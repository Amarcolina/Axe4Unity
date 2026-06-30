using System;
using UnityEngine;

namespace Axe4Unity.Op {
  using static Constants;

  [Serializable]
  public struct UnArchive : IOp_Function {

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      var name = Utils.PtrToString(machine, machine.PopArg());

      if (!machine.ArchiveFiles.TryGetValue(name, out var file)) {
        Debug.Log($"Tried to un-archive file {name} but it was not found!");
        machine.HL = 0;
        return;
      }

      if (!machine.TryCreateRAMFile(name, file.Length, out var addr)) {
        Debug.LogError($"Tried to un-archive file {name} but there was not enough free RAM!");
        machine.HL = 0;
        return;
      }

      for (int i = 0; i < file.Length; i++) {
        machine.Write_U8(addr + i, file[i]);
      }

      machine.DeleteFile(name);

      machine.HL = 1;
    }
  }

  [Serializable]
  public struct Archive : IOp_Function {

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      var name = Utils.PtrToString(machine, machine.PopArg());

      int index = machine.IndexOfFile(name);
      if (index < 0) {
        Debug.Log($"Tried to archive file {name} but it was not found!");
        machine.HL = 0;
      }

      var file = machine.FileMetadata[index];
      if (file.IsArchived) {
        Debug.Log($"Tried to archive file {name} but it was already in the archive");
        machine.HL = 0;
      }

      var data = machine.CreateArchiveFile(name, file.Size);

      for (int i = 0; i < data.Length; i++) {
        data[i] = (byte)machine.Read_U8(file.Address + i);
      }
      
      machine.DeleteFile(name);

      machine.HL = 1;
    }
  }

  [Serializable]
  public struct DelVar : IOp_Function {

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      var name = Utils.PtrToString(machine, machine.PopArg());
      machine.DeleteFile(name);
    }
  }

  [Serializable]
  public struct GetCalcFromRam : IOp {

    public void Execute(ref MachineStateNative machine) {
      var name = Utils.PtrToString(machine, machine.HL);

      int index = machine.IndexOfFile(name);
      if (index < 0) {
        Debug.Log($"Could not find file {name} on system");
        machine.HL = 0;
      }

      var file = machine.FileMetadata[index];
      if (file.IsArchived) {
        Debug.Log($"Tried to load file {name} but it was archived");
        machine.HL = 0;
      }

      Debug.Log($"Loaded file {name} from ram");

      machine.HL = file.Address;
    }
  }

  [Serializable]
  public struct GetCalcFromFileSystem : IOp {

    public ushort VarAddress;

    public void Execute(ref MachineStateNative machine) {
      var name = Utils.PtrToString(machine, machine.HL);

      var index = machine.IndexOfFile(name);
      if (index < 0) {
        Debug.Log($"Could not mount file with name {name} because it was not found");
        machine.Write_U16(VarAddress + 2, FILE_HANDLE_ID_INVALID);
        machine.HL = 0;
        return;
      }

      var file = machine.FileMetadata[index];

      ushort id = 1;
      while (machine.MountedFiles.ContainsKey(id)) {
        id++;
      }

      Debug.Log($"Mounted file with name {name} into file variable with id {id}");
      machine.MountedFiles[id] = name;
      machine.Write_U16(VarAddress - 2, file.Size);
      machine.Write_U16(VarAddress, file.Address);
      machine.Write_U16(VarAddress + 2, id);
      machine.HL = 1;
    }
  }

  [Serializable]
  public struct GetCalcCreate : IOp {

    public void Execute(ref MachineStateNative machine) {
      var size = machine.HL;
      var namePtr = machine.PopArg();

      var name = Utils.PtrToString(machine, namePtr);

      if (!machine.TryCreateRAMFile(name, size, out var addr)) {
        Debug.LogError($"Not enough RAM to create file {name}!");
        machine.HL = 0;
        return;
      }

      Debug.Log($"File {name} was created at address {addr}");

      machine.HL = (ushort)addr;
    }
  }
}
