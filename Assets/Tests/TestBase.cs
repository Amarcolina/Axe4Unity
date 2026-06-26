using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;

namespace Axe4Unity {

  [Timeout(1000)]
  public class TestBase {

    public Machine Machine;

    [SetUp]
    public void SetUp() { }

    [TearDown]
    public void TearDown() {
      if (Machine != null) {
        Machine.Dispose();
        Machine = null;
      }
    }

    public Program Execute(params string[] lines) {
      if (Machine != null) {
        Machine.Dispose();
        Machine = null;
      }

      List<List<Token>> parsedLines = new();
      foreach (var line in lines) {
        parsedLines.Add(Token.ParseLine(line));
      }

      var programAssetA = ScriptableObject.CreateInstance<ProgramAsset>();
      var programAssetB = ScriptableObject.CreateInstance<ProgramAsset>();

      programAssetA.Program = Compiler.Compile(parsedLines);

      EditorUtility.CopySerializedManagedFieldsOnly(programAssetA, programAssetB);

      Machine = new Machine(programAssetB.Program);

      int maxIt = 10_000;
      while (Machine.Step() != null) {
        maxIt--;
        if (maxIt <= 0) {
          Assert.Fail("Program took too long to terminate");
        }
      }

      Assert.That(Machine.State.ArgStackTop, Is.Zero);
      Assert.That(Machine.State.CallStackTop, Is.Zero);

      return programAssetB.Program;
    }

    public int Addr(string name) => Machine.AddressOfName(name);

    public ushort U16(string name) => Machine.State.Read_U16(Machine.AddressOfName(name));
    public short S16(string name) => Machine.State.Read_S16(Machine.AddressOfName(name));
    public ushort U8(string name) => Machine.State.Read_U8(Machine.AddressOfName(name));

    public ushort U16(int addr) => Machine.State.Read_U16(addr);
    public short S16(int addr) => Machine.State.Read_S16(addr);
    public ushort U8(int addr) => Machine.State.Read_U8(addr);
  }
}
