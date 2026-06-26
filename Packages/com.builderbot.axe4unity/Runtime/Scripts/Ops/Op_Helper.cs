using System;
using UnityEngine;

namespace Axe4Unity.Op
{

  [Serializable]
  public struct Nop : IOp {

    public void Execute(ref MachineStateNative machine) { }
  }

  [Serializable]
  public struct PushArg : IOp {

    public void Execute(ref MachineStateNative machine) {
      machine.PushArg();
    }
  }

  [Serializable]
  public struct SwapStack : IOp {

    public void Execute(ref MachineStateNative machine) {
      var value = machine.HL;
      machine.HL = machine.PopArg();
      machine.PushArg(value);
    }
  }

  [Serializable]
  public struct Binary_U16<OpT> : IOp, IOp_Binary_U16 where OpT : IOp_Binary_U16 {

    public void Execute(ref MachineStateNative machine) {
      var lhs = machine.PopArg();
      var rhs = machine.HL;

      machine.HL = default(OpT).Execute(ref machine, lhs, rhs);
    }

    public ushort Execute(ref MachineStateNative machine, ushort lhs, ushort rhs) {
      return default(OpT).Execute(ref machine, lhs, rhs);
    }
  }

  [Serializable]
  public struct Binary_S16<OpT> : IOp, IOp_Binary_S16 where OpT : IOp_Binary_S16 {

    public void Execute(ref MachineStateNative machine) {
      short lhs = (short)machine.PopArg();
      short rhs = (short)machine.HL;

      machine.HL = (ushort)default(OpT).Execute(ref machine, lhs, rhs);
    }

    public short Execute(ref MachineStateNative machine, short lhs, short rhs) {
      return default(OpT).Execute(ref machine, lhs, rhs);
    }
  }

  [Serializable]
  public struct Binary_U8<OpT> : IOp, IOp_Binary_U8 where OpT : IOp_Binary_U8 {

    public void Execute(ref MachineStateNative machine) {
      ushort lhs = machine.PopArg();
      ushort rhs = machine.HL;

      machine.HL = default(OpT).Execute(ref machine, (byte)(lhs & 0xFF), (byte)(rhs & 0xFF));
    }

    public byte Execute(ref MachineStateNative machine, byte lhs, byte rhs) {
      return default(OpT).Execute(ref machine, lhs, rhs);
    }
  }
}
