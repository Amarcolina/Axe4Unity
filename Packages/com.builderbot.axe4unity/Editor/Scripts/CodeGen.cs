using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Axe4Unity {

  public static class CodeGen {

    public static void Generate(string asset, Program program) {
      string programName = Path.GetFileNameWithoutExtension(asset);

      using (var writer = File.CreateText(asset)) {
        writer.WriteLine("using System;");
        writer.WriteLine("using UnityEngine;");
        writer.WriteLine("using Unity.Burst;");
        writer.WriteLine("using Unity.Collections.LowLevel.Unsafe;");
        writer.WriteLine("using Axe4Unity;");
        writer.WriteLine("using Axe4Unity.Op;");
        writer.WriteLine();

        writer.WriteLine("[BurstCompile]");
        writer.WriteLine($"public class {programName} : NativeRunner {{");
        writer.WriteLine();

        writer.WriteLine("  public override OpAndMetaData Step(Machine machine, int maxSteps) {");
        writer.WriteLine("    unsafe {");
        writer.WriteLine("      var statePtr = UnsafeUtility.AddressOf(ref machine.State);");
        writer.WriteLine("      var lastExecuted = Execute(statePtr, maxSteps);");
        writer.WriteLine("      int lineIndex = (int)(lastExecuted / 10000);");
        writer.WriteLine("      int opIndex = (int)(lastExecuted % 10000);");
        writer.WriteLine("      return machine.Program.Lines[lineIndex].Ops[opIndex];");
        writer.WriteLine("    }");
        writer.WriteLine("  }");
        writer.WriteLine();

        writer.WriteLine("  [BurstCompile]");
        writer.WriteLine("  public static unsafe ulong Execute(void* ptr, int maxSteps) {");
        writer.WriteLine("    ref MachineStateNative machine = ref UnsafeUtility.AsRef<MachineStateNative>(ptr);");
        writer.WriteLine("    return Execute(ref machine, maxSteps);");
        writer.WriteLine("  }");
        writer.WriteLine();

        writer.WriteLine("  public static ulong Execute(ref MachineStateNative machine, int maxSteps) {");
        writer.WriteLine("    ulong lastExecuted = default;");
        writer.WriteLine("    while (maxSteps > 0) {");
        writer.WriteLine("    switch(machine.PC.GetLongCode()) {");
        writer.WriteLine("      default:");
        writer.WriteLine("        throw new InvalidOperationException($\"Tried to jump to line {machine.PC.LineIndex} and op {machine.PC.OpIndex} with code {machine.PC.GetLongCode()}\");");

        HashSet<(int line, int op)> jumpLocations = new() {
          (0, 0)
        };

        //Find jump locations
        for (int i = 0; i < program.Lines.Count; i++) {
          var line = program.Lines[i];
          for (int j = 0; j < line.Ops.Count; j++) {
            var op = line.Ops[j].Op;

            int nextLine, nextOp;
            if (j == (line.Ops.Count - 1)) {
              nextLine = i + 1;
              nextOp = 0;
            } else {
              nextLine = i;
              nextOp = j + 1;
            }

            if (op is IOpControl controlOp) {
              jumpLocations.Add((controlOp.JumpLine, controlOp.JumpOp));
              jumpLocations.Add((nextLine, nextOp));
            }

            if (op is Op.Label) {
              jumpLocations.Add((i, j));
            }

            if (op is Op.CallAddr or Op.Text) {
              jumpLocations.Add((nextLine, nextOp));
            }

            if (op is IOpLoopExit exitOp && exitOp.ShouldExit) {
              jumpLocations.Add((nextLine, nextOp));
            }
          }
        }

        //Build op blocks
        List<List<OpAndMetaData>> opBlocks = new();
        {
          List<OpAndMetaData> currBlock = new();
          for (int i = 0; i < program.Lines.Count; i++) {
            var line = program.Lines[i];
            for (int j = 0; j < line.Ops.Count; j++) {
              var item = line.Ops[j];

              if (jumpLocations.Contains((i, j)) && currBlock.Count != 0) {
                opBlocks.Add(currBlock);
                currBlock = new();
              }

              currBlock.Add(item);
            }
          }

          if (currBlock.Count != 0) {
            opBlocks.Add(currBlock);
          }
        }

        //Output ops
        int prevLine = -1;
        for (int blockI = 0; blockI < opBlocks.Count; blockI++) {
          var block = opBlocks[blockI];
          int firstLine = block[0].LineIndex;
          int firstOp = block[0].OpIndex;
          int lastLine = block[block.Count - 1].LineIndex;
          int lastOp = block[block.Count - 1].OpIndex;

          int nextBlockLine = 0;
          int nextBlockOp = 0;
          if (blockI != opBlocks.Count - 1) {
            nextBlockLine = opBlocks[blockI + 1][0].LineIndex;
            nextBlockOp = opBlocks[blockI + 1][0].OpIndex;
          }

          writer.WriteLine($"      case {firstLine}_{firstOp:D4}:");
          writer.WriteLine($"        machine.PC = new ProgramCounter({nextBlockLine}, {nextBlockOp});");
          writer.WriteLine($"        lastExecuted = {lastLine}_{lastOp:D4};");
          writer.WriteLine($"        maxSteps -= {block.Count};");

          foreach (var item in block) {
            if (item.LineIndex != prevLine) {
              writer.WriteLine();
              writer.WriteLine($"        //{program.Lines[item.LineIndex].Text}");
            }

            List<(string name, object val)> args = new();
            foreach (var field in item.Op.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)) {
              args.Add((field.Name, field.GetValue(item.Op)));
            }
            foreach (var prop in item.Op.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
              args.Add((prop.Name, prop.GetValue(item.Op)));
            }

            writer.Write($"        new {ToNaturalString(item.Op.GetType())}() {{ ");
            writer.Write(string.Join(", ", args.Select(a => $"{a.name} = {ToValueString(a.val)}")));
            writer.Write(" }.Execute(ref machine);");
            writer.WriteLine();

            prevLine = item.LineIndex;
          }

          if (block[block.Count - 1].Op is IOpLoopExit loopExit && loopExit.ShouldExit) {
            writer.WriteLine($"        return lastExecuted;");
          } else {
            writer.WriteLine($"        break;");
          }
        }

        writer.WriteLine("    }");
        writer.WriteLine("    }");
        writer.WriteLine("  return lastExecuted;");
        writer.WriteLine("  }");
        writer.WriteLine("}");
      }
    }

    private static string ToNaturalString(Type type) {
      if (!type.IsGenericType) {
        return type.Name;
      }

      var str = type.Name;
      var name = str.Substring(0, str.IndexOf('`'));
      return $"{name}<{string.Join(", ", type.GenericTypeArguments.Select(ToNaturalString))}>";
    }

    private static string ToValueString(object val) {
      if (val is bool b) {
        return b ? "true" : "false";
      } else {
        return val.ToString();
      }
    }
  }
}
