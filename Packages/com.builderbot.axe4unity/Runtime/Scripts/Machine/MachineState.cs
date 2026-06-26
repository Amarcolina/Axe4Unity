using System;
using System.Collections.Generic;
using Unity.Collections;

namespace Axe4Unity {

  [Serializable]
  public struct MachineState {

    public byte[] Memory;
    public List<MountedFile> MountedFiles;
    public List<RamFile> RamFiles;
    public List<ProgramCounter> CallStack;
    public List<ushort> ArgStack;
    public ushort HL;

    public void CopyTo(MachineStateNative machine) {
      machine.Memory.CopyFrom(Memory);

      machine.MountedFiles.Clear();
      foreach (var item in MountedFiles) {
        machine.MountedFiles.Add(item.Id, item.Name);
      }

      machine.RamFiles.Clear();
      foreach (var item in RamFiles) {
        machine.RamFiles.Add(item.Name, (item.Addr, item.Size));
      }

      machine.CallStackTop = CallStack.Count;
      for (int i = 0; i < CallStack.Count; i++) {
        machine.CallStack[i] = CallStack[i];
      }

      machine.ArgStackTop = ArgStack.Count;
      for (int i = 0; i < ArgStack.Count; i++) {
        machine.ArgStack[i] = ArgStack[i];
      }

      machine.HL = HL;
    }

    public void CopyFrom(MachineStateNative machine) {
      if (Memory == null || Memory.Length != machine.Memory.Length) {
        Memory = new byte[machine.Memory.Length];
      }
      machine.Memory.CopyTo(Memory);

      MountedFiles = new();
      foreach (var pair in machine.MountedFiles) {
        MountedFiles.Add(new MountedFile() {
          Id = pair.Key,
          Name = pair.Value.ToString()
        });
      }

      RamFiles = new();
      foreach (var pair in machine.RamFiles) {
        RamFiles.Add(new RamFile() {
          Name = pair.Key.ToString(),
          Addr = pair.Value.ptr,
          Size = pair.Value.size
        });
      }

      CallStack = new List<ProgramCounter>(machine.CallStack.Slice(0, machine.CallStackTop));
      ArgStack = new List<ushort>(machine.ArgStack.Slice(0, machine.ArgStackTop));
      HL = machine.HL;
    }

    [Serializable]
    public struct MountedFile {
      public ushort Id;
      public string Name;
    }

    [Serializable]
    public struct RamFile {
      public string Name;
      public ushort Addr;
      public ushort Size;
    }
  }
}
