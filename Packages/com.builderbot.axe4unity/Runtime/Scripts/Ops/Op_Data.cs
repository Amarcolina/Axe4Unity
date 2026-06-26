using System;
using UnityEngine;
using Unity.Collections;

namespace Axe4Unity.Op {
  using static Constants;

  [Serializable]
  public struct StoreMemory : IOp, IOpRModifier {

    [field: SerializeField]
    public int RMode { get; set; }

    public void Execute(ref MachineStateNative machine) {
      var value = machine.PopArg();
      var addr = machine.HL;

      switch (RMode) {
        case 0:
          machine.Write_U8(addr, value);
          break;
        case 1:
          machine.Write_U16(addr, value);
          machine.HL = (ushort)(addr + 1);
          break;
      }
    }
  }

  [Serializable]
  public struct StoreAddress : IOp, IOpRModifier {

    [field: SerializeField]
    public int RMode { get; set; }

    public int Address;

    public void Execute(ref MachineStateNative machine) {
      var value = machine.HL;

      switch (RMode) {
        case 0:
          machine.Write_U8(Address, value);
          break;
        case 1:
          machine.Write_U16(Address, value);
          break;
      }
    }
  }

  [Serializable]
  public struct ReadMemory : IOp, IOpRModifier {

    [field: SerializeField]
    public int RMode { get; set; }

    public void Execute(ref MachineStateNative machine) {
      var addr = machine.HL;

      switch (RMode) {
        case 0:
          machine.HL = machine.Read_U8(addr);
          break;
        case 1:
          machine.HL = machine.Read_U16(addr);
          break;
        default:
          throw new Exception();
      }
    }
  }

  [Serializable]
  public struct ReadMemorySignedByte : IOp {

    public void Execute(ref MachineStateNative machine) {
      var addr = machine.HL;
      machine.HL = (ushort)machine.Read_S8(addr);
    }
  }

  [Serializable]
  public struct ReadAddress : IOp {

    public ushort VarAddress;

    public void Execute(ref MachineStateNative machine) {
      machine.HL = machine.Read_U16(VarAddress);
    }
  }

  [Serializable]
  public struct ReadFile : IOp, IOpRModifier {

    [field: SerializeField]
    public int RMode { get; set; }

    public ushort VarAddress;

    public void Execute(ref MachineStateNative machine) {
      var fileOffset = machine.HL;
      var fileId = machine.Read_U16(VarAddress + 2);

      switch (RMode) {
        case 0:
          machine.HL = machine.ReadFile_U8(fileId, fileOffset);
          break;
        case 1:
          machine.HL = machine.ReadFile_U16(fileId, fileOffset);
          break;
        default:
          throw new Exception();
      }
    }
  }

  [Serializable]
  public struct FileHandle : IOp {

    public ushort VarAddress;

    public void Execute(ref MachineStateNative machine) {
      machine.HL = machine.Read_U16(VarAddress);
    }
  }

  [Serializable]
  public struct Const : IOp {

    public ushort Value;

    public void Execute(ref MachineStateNative machine) {
      machine.HL = Value;
    }
  }

  [Serializable]
  public struct Fill : IOp_Function {

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      int ptr, size;
      byte val;

      switch (ArgCount) {
        default:
          throw new InvalidOperationException();
        case 2:
          size = machine.PopArg();
          ptr = machine.PopArg();
          val = (byte)machine.Read_U8(ptr);
          break;
        case 3:
          val = (byte)(machine.PopArg() & 0xFF);
          size = machine.PopArg();
          ptr = machine.PopArg();
          break;
      }

      var buffer = machine.GetBuffer(ptr, size);
      for (int i = 0; i < size; i++) {
        buffer[i] = val;
      }
    }
  }

  [Serializable]
  public struct Copy : IOp_Function, IOpRModifier {

    [field: SerializeField]
    public int RMode { get; set; }

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      //TODO: have this work for files as well
      int addrFrom, addrTo, count;
      switch (ArgCount) {
        default: throw new InvalidOperationException();
        case 1:
          addrFrom = machine.PopArg();
          addrTo = Machine.ADDR_L6;
          count = SCREEN_BYTES;
          break;
        case 2:
          addrTo = machine.PopArg();
          addrFrom = machine.PopArg();
          count = SCREEN_BYTES;
          break;
        case 3:
          count = machine.PopArg();
          addrTo = machine.PopArg();
          addrFrom = machine.PopArg();
          break;
      }

      switch (RMode) {
        case 0: {
          var bufferTo = machine.GetBuffer(addrTo, count);
          var bufferFrom = machine.GetBuffer(addrFrom, count);
          for (int i = 0; i < count; i++) {
            bufferTo[i] = bufferFrom[i];
          }
          break;
        }
        case 1: {
          var bufferTo = machine.GetBuffer(addrTo - count + 1, count);
          var bufferFrom = machine.GetBuffer(addrFrom - count + 1, count);
          for (int i = count - 1; i >= 0; i--) {
            bufferTo[i] = bufferFrom[i];
          }
          break;
        }
      }
    }
  }

  [Serializable]
  public struct Exch : IOp_Function {

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      int size = machine.PopArg();
      var buffA = machine.GetBuffer(machine.PopArg(), size);
      var buffB = machine.GetBuffer(machine.PopArg(), size);

      for (int i = 0; i < size; i++) {
        (buffA[i], buffB[i]) = (buffB[i], buffA[i]);
      }
    }
  }

  [Serializable]
  public struct InData : IOp_Function {

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      ushort size = ushort.MaxValue;
      if (ArgCount == 3) {
        size = machine.PopArg();
      }

      var ptr = machine.PopArg();
      var val = machine.PopArg();

      for (int i = 0; i < size; i++) {
        if (machine.Read_U8(ptr++) == val) {
          machine.HL = (ushort)(i + 1);
          return;
        }
      }
      machine.HL = 0;
    }
  }

  [Serializable]
  public struct Length : IOp_Function {

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      var ptr = machine.PopArg();
      ushort length = 0;
      while (machine.Memory[ptr] != 0) {
        length++;
        ptr++;
      }
      machine.HL = length;
    }
  }

  [Serializable]
  public struct StrGet : IOp_Function {

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      var n = machine.PopArg();
      var ptr = machine.PopArg();

      for (; n != 0; n--) {
        while (machine.Read_U8(ptr++) != 0) { }
      }

      machine.HL = ptr;
    }
  }

  [Serializable]
  public struct StrEq : IOp_Function {

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      var str2 = machine.PopArg();
      var str1 = machine.PopArg();

      while (true) {
        var c1 = machine.Read_U8(str1++);
        var c2 = machine.Read_U8(str2++);
        if (c1 != c2) {
          machine.HL = 0;
          return;
        }

        if (c1 == 0) {
          machine.HL = 1;
          return;
        }
      }
    }
  }

  [Serializable]
  public struct Sort : IOp_Function {

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      var size = machine.PopArg();
      var ptr = machine.PopArg();

      machine.GetBuffer(ptr, size).Sort();
    }
  }

  [Serializable]
  public struct CheckSum : IOp_Function {

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      var size = machine.PopArg();
      var ptr = machine.PopArg();

      ushort sum = 0;
      for (; size != 0; size--) {
        sum += machine.Read_U8(ptr++);
      }

      machine.HL = sum;
    }
  }

}
