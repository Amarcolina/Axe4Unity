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
        int indent = 0;
        bool startOfLine = true;

        void Write(string str) {
          if (startOfLine) {
            for (int i = 0; i < indent; i++) {
              writer.Write(' ');
              writer.Write(' ');
            }
            startOfLine = false;
          }
          writer.Write(str);
        }

        void WriteLine(string str = null) {
          if (str != null) {
            Write(str);
          }
          writer.WriteLine();
          startOfLine = true;
        }

        WriteLine("using System;");
        WriteLine("using UnityEngine;");
        WriteLine("using Unity.Burst;");
        WriteLine("using Unity.Collections.LowLevel.Unsafe;");
        WriteLine("using Axe4Unity;");
        WriteLine("using Axe4Unity.Op;");
        WriteLine();

        WriteLine("[BurstCompile]");
        WriteLine($"public class {programName} : NativeRunner {{");
        indent++;

        WriteLine();

        WriteLine("public override Results Step(Machine machine, int maxSteps, int maxGetKeys) {");
        WriteLine("  unsafe {");
        WriteLine("    var statePtr = UnsafeUtility.AddressOf(ref machine.State);");
        WriteLine("    Execute(statePtr, maxSteps, maxGetKeys, out var lastOpIndex, out var stepsCompleted, out var getKeysCompleted);");
        WriteLine("    int lineIndex = (int)(lastOpIndex / 10000);");
        WriteLine("    int opIndex = (int)(lastOpIndex % 10000);");
        WriteLine("    var lastOp = machine.Program.Lines[lineIndex].Ops[opIndex];");
        WriteLine("    return new Results() {");
        WriteLine("      LastOpExecuted = lastOp,");
        WriteLine("      StepsCompleted = stepsCompleted,");
        WriteLine("      IsGetKeyTimeout = getKeysCompleted >= maxGetKeys,");
        WriteLine("    };");
        WriteLine("  }");
        WriteLine("}");
        WriteLine();

        WriteLine("[BurstCompile]");
        WriteLine("public static unsafe void Execute(");
        WriteLine("  void* ptr,");
        WriteLine("  int maxSteps,");
        WriteLine("  int maxGetKey,");
        WriteLine("  out ulong lastOpIndex,");
        WriteLine("  out int stepsCompleted,");
        WriteLine("  out int getKeysCompleted) {");
        WriteLine("  ref MachineStateNative machine = ref UnsafeUtility.AsRef<MachineStateNative>(ptr);");
        WriteLine("  Execute(ref machine, maxSteps, maxGetKey, out lastOpIndex, out stepsCompleted, out getKeysCompleted);");
        WriteLine("}");
        WriteLine();

        WriteLine("public static void Execute(");
        WriteLine("  ref MachineStateNative machine,");
        WriteLine("  int maxSteps,");
        WriteLine("  int maxGetKeys,");
        WriteLine("  out ulong lastOpIndex,");
        WriteLine("  out int stepsCompleted,");
        WriteLine("  out int getKeysCompleted) {");
        indent++;

        WriteLine("lastOpIndex = 0;");
        WriteLine("stepsCompleted = 0;");
        WriteLine("getKeysCompleted = 0;");
        WriteLine();

        WriteLine("while (stepsCompleted < maxSteps && getKeysCompleted < maxGetKeys) {");
        indent++;

        WriteLine("switch(machine.PC.GetLongCode()) {");
        indent++;

        WriteLine("default:");
        WriteLine("  throw new InvalidOperationException($\"Tried to jump to line {machine.PC.LineIndex} and op {machine.PC.OpIndex} with code {machine.PC.GetLongCode()}\");");

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

            if (op is Op.CallAddr or Op.Text or Op.Return) {
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

          WriteLine($"case {firstLine}_{firstOp:D4}:");
          indent++;

          WriteLine($"machine.PC = new ProgramCounter({nextBlockLine}, {nextBlockOp});");
          WriteLine($"lastOpIndex = {lastLine}_{lastOp:D4};");
          WriteLine($"stepsCompleted += {block.Count};");

          int getKeyCount = block.Count(o => o.Op is Op.GetKey);
          if (getKeyCount != 0) {
            WriteLine($"getKeysCompleted += {getKeyCount};");
          }

          foreach (var item in block) {
            if (item.LineIndex != prevLine) {
              WriteLine();
              WriteLine($"//{program.Lines[item.LineIndex].Text}");
            }

            List<(string name, object val)> args = new();
            foreach (var field in item.Op.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)) {
              args.Add((field.Name, field.GetValue(item.Op)));
            }
            foreach (var prop in item.Op.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
              args.Add((prop.Name, prop.GetValue(item.Op)));
            }

            Write($"new {ToNaturalString(item.Op.GetType())}()");
            if (args.Count != 0) {
              Write(" { ");
              Write(string.Join(", ", args.Select(a => $"{a.name} = {ToValueString(a.val)}")));
              Write(" }");
            }
            Write(".Execute(ref machine);");
            WriteLine();

            prevLine = item.LineIndex;
          }

          if (block[block.Count - 1].Op is IOpLoopExit loopExit && loopExit.ShouldExit) {
            WriteLine($"return;");
          } else {
            WriteLine($"break;");
          }

          indent--;
        }

        indent--; //switch
        WriteLine("}");

        indent--; //while
        WriteLine("}");
        WriteLine("return;");

        indent--; //method
        WriteLine("}");

        indent--; //class
        WriteLine("}");
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
