using System;
using UnityEngine;

namespace Axe4Unity.Op.MemKit {

  [Serializable]
  public struct Load : IOp {

    public void Execute(ref MachineStateNative machine) {
      machine.MemKitIndex = 0;
    }
  }

  [Serializable]
  public struct Next : IOp {

    public void Execute(ref MachineStateNative machine) {
      machine.MemKitIndex++;



      machine.HL = (ushort)((machine.MemKitIndex >= machine.FileMetadata.Length) ? 0 : 1);

      Debug.Log(machine.MemKitIndex + " : " + machine.FileMetadata.Length + " : " + machine.HL);
    }
  }

  [Serializable]
  public struct Dim : IOp, IOpRModifier {

    [field: SerializeField]
    public int RMode { get; set; }

    public void Execute(ref MachineStateNative machine) {
      if (machine.MemKitIndex >= machine.FileMetadata.Length) {
        machine.HL = 0;
        return;
      }

      var file = machine.FileMetadata[machine.MemKitIndex];

      switch (RMode) {
        case 0:
          //TODO: hard-coded for only appvars for now
          machine.HL = 21;
          break;
        case 1:
          machine.HL = file.Address;
          break;
        case 2:
          machine.HL = (ushort)(file.IsArchived ? 1 : 0);
          break;
      }
    }
  }

  [Serializable]
  public struct Print : IOp_Function {

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      int buffer = machine.PopArg();

      if (machine.MemKitIndex >= machine.FileMetadata.Length) {
        return;
      }

      var file = machine.FileMetadata[machine.MemKitIndex];

      for (int i = 1; i < file.Name.Length; i++) {
        machine.Write_U8(buffer++, file.Name[i]);
      }
      machine.Write_U8(buffer++, 0);
    }
  }

  [Serializable]
  public struct New : IOp_Function {

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      int size = machine.PopArg();
      int offset = machine.PopArg();
      int addr = machine.PopArg();

      int fileIndex = -1;
      for (int i = 0; i < machine.FileMetadata.Length; i++) {
        if (machine.FileMetadata[i].Address == addr) {
          fileIndex = i;
          break;
        }
      }

      if (fileIndex == -1) {
        machine.HL = 0;
        return;
      }

      var file = machine.FileMetadata[fileIndex];

      if (!machine.TryCreateRAMFile(file.Name, file.Size + size, out var newAddr)) {
        machine.HL = 0;
        return;
      }

      for (int i = addr; i < offset; i++) {
        machine.Write_U8(newAddr++, machine.Read_U8(addr++));
      }

      for (int i = 0; i < size; i++) {
        machine.Write_U8(newAddr++, 0);
      }

      for (int i = offset; i < file.Size; i++) {
        machine.Write_U8(newAddr++, machine.Read_U8(addr++));
      }

      machine.DeleteFile(file.Name);

      machine.HL = 1;
    }
  }

  [Serializable]
  public struct Delete : IOp_Function {

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      int size = machine.PopArg();
      int offset = machine.PopArg();
      int addr = machine.PopArg();

      int fileIndex = -1;
      for (int i = 0; i < machine.FileMetadata.Length; i++) {
        if (machine.FileMetadata[i].Address == addr) {
          fileIndex = i;
          break;
        }
      }

      if (fileIndex == -1) {
        machine.HL = 0;
        return;
      }

      var file = machine.FileMetadata[fileIndex];

      int remaining = file.Size - offset - size;
      for (int i = 0; i < remaining; i++) {
        machine.Write_U8(offset, machine.Read_U8(offset + size));
        offset++;
      }

      machine.HL = 1;
    }
  }
}
