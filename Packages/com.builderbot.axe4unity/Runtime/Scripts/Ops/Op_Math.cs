using System;
using UnityEngine;

namespace Axe4Unity.Op {

  [Serializable]
  public struct Negate : IOp {

    public void Execute(ref MachineStateNative machine) {
      machine.HL = (ushort)(-(short)machine.HL);
    }
  }

  [Serializable]
  public struct Inc : IOp {

    public void Execute(ref MachineStateNative machine) {
      machine.HL++;
    }
  }

  [Serializable]
  public struct Dec : IOp {

    public void Execute(ref MachineStateNative machine) {
      machine.HL--;
    }
  }

  [Serializable]
  public struct Add : IOp_Binary_U16 {
    public ushort Execute(ref MachineStateNative machine, ushort lhs, ushort rhs) => (ushort)(lhs + rhs);
  }

  [Serializable]
  public struct Sub : IOp_Binary_U16 {
    public ushort Execute(ref MachineStateNative machine, ushort lhs, ushort rhs) => (ushort)(lhs - rhs);
  }

  [Serializable]
  public struct Mul : IOp_Binary_U16 {
    public ushort Execute(ref MachineStateNative machine, ushort lhs, ushort rhs) => (ushort)(lhs * rhs);
  }

  [Serializable]
  public struct MulFixed : IOp_Binary_U16 {
    public ushort Execute(ref MachineStateNative machine, ushort lhs, ushort rhs) {
      return (ushort)(((short)lhs * (short)rhs) / 256);
    }
  }

  [Serializable]
  public struct MulHigh : IOp_Binary_U16 {
    public ushort Execute(ref MachineStateNative machine, ushort lhs, ushort rhs) {
      return (ushort)((lhs * rhs) >> 16);
    }
  }

  [Serializable]
  public struct Div : IOp_Binary_U16 {
    public ushort Execute(ref MachineStateNative machine, ushort lhs, ushort rhs) {
      if (rhs == 0) {
        return ushort.MaxValue;
      } else {
        return (ushort)(lhs / rhs);
      }
    }
  }

  [Serializable]
  public struct DivS : IOp_Binary_S16 {

    public short Execute(ref MachineStateNative machine, short lhs, short rhs) {
      if (rhs == 0) {
        if (lhs >= 0) {
          return short.MaxValue;
        } else {
          unchecked {
            return short.MinValue;
          }
        }
      } else if (rhs == -32768) {
        return (short)(lhs >= 0 ? 0 : -1);
      } else {
        float f = lhs / (float)rhs;
        return (short)Mathf.FloorToInt(f);
      }
    }
  }

  [Serializable]
  public struct DivFixed : IOp_Binary_U16 {
    public ushort Execute(ref MachineStateNative machine, ushort lhs, ushort rhs) {
      return (ushort)(((short)lhs / (short)rhs) / 256);
    }
  }

  [Serializable]
  public struct Mod : IOp_Binary_U16 {
    public ushort Execute(ref MachineStateNative machine, ushort lhs, ushort rhs) {
      if (rhs == 0) {
        return rhs;
      } else {
        return (ushort)(lhs % rhs);
      }
    }
  }

  [Serializable]
  public struct Square : IOp, IOpRModifier {

    [field: SerializeField]
    public int RMode { get; set; }

    public void Execute(ref MachineStateNative machine) {
      switch (RMode) {
        case 0:
          machine.HL *= machine.HL;
          break;
        case 1:
          machine.HL = (ushort)(machine.HL * machine.HL / 256);
          break;
      }
    }
  }

  [Serializable]
  public struct Recip : IOp {

    public void Execute(ref MachineStateNative machine) {
      float v = machine.HL / 256f;
      if (v == 0) {
        machine.HL = 0xFFFF;
        return;
      }

      v = 1f / v;

      machine.HL = (ushort)Mathf.RoundToInt(v * 256);
    }
  }

  [Serializable]
  public struct Eq : IOp_Binary_U16 {
    public ushort Execute(ref MachineStateNative machine, ushort lhs, ushort rhs) => (ushort)(lhs == rhs ? 1 : 0);
  }

  [Serializable]
  public struct NEq : IOp_Binary_U16 {
    public ushort Execute(ref MachineStateNative machine, ushort lhs, ushort rhs) => (ushort)(lhs != rhs ? 1 : 0);
  }

  [Serializable]
  public struct Greater : IOp_Binary_U16 {
    public ushort Execute(ref MachineStateNative machine, ushort lhs, ushort rhs) => (ushort)(lhs > rhs ? 1 : 0);
  }

  [Serializable]
  public struct GreaterS : IOp_Binary_S16 {
    public short Execute(ref MachineStateNative machine, short lhs, short rhs) => (short)(lhs > rhs ? 1 : 0);
  }

  [Serializable]
  public struct GreaterEq : IOp_Binary_U16 {
    public ushort Execute(ref MachineStateNative machine, ushort lhs, ushort rhs) => (ushort)(lhs >= rhs ? 1 : 0);
  }

  [Serializable]
  public struct GreaterEqS : IOp_Binary_S16 {
    public short Execute(ref MachineStateNative machine, short lhs, short rhs) => (short)(lhs >= rhs ? 1 : 0);
  }

  [Serializable]
  public struct Less : IOp_Binary_U16 {
    public ushort Execute(ref MachineStateNative machine, ushort lhs, ushort rhs) => (ushort)(lhs < rhs ? 1 : 0);
  }

  [Serializable]
  public struct LessS : IOp_Binary_S16 {
    public short Execute(ref MachineStateNative machine, short lhs, short rhs) => (short)(lhs < rhs ? 1 : 0);
  }

  [Serializable]
  public struct LessEq : IOp_Binary_U16 {
    public ushort Execute(ref MachineStateNative machine, ushort lhs, ushort rhs) => (ushort)(lhs <= rhs ? 1 : 0);
  }

  [Serializable]
  public struct LessEqS : IOp_Binary_S16 {
    public short Execute(ref MachineStateNative machine, short lhs, short rhs) => (short)(lhs <= rhs ? 1 : 0);
  }

  [Serializable]
  public struct And_U8 : IOp_Binary_U8 {
    public byte Execute(ref MachineStateNative machine, byte lhs, byte rhs) => (byte)(lhs & rhs);
  }

  [Serializable]
  public struct And_U16 : IOp_Binary_U16 {
    public ushort Execute(ref MachineStateNative machine, ushort lhs, ushort rhs) => (ushort)(lhs & rhs);
  }

  [Serializable]
  public struct Or_U8 : IOp_Binary_U8 {
    public byte Execute(ref MachineStateNative machine, byte lhs, byte rhs) => (byte)(lhs | rhs);
  }

  [Serializable]
  public struct Or_U16 : IOp_Binary_U16 {
    public ushort Execute(ref MachineStateNative machine, ushort lhs, ushort rhs) => (ushort)(lhs | rhs);
  }

  [Serializable]
  public struct Xor_U8 : IOp_Binary_U8 {
    public byte Execute(ref MachineStateNative machine, byte lhs, byte rhs) => (byte)(lhs ^ rhs);
  }

  [Serializable]
  public struct Xor_U16 : IOp_Binary_U16 {
    public ushort Execute(ref MachineStateNative machine, ushort lhs, ushort rhs) => (ushort)(lhs ^ rhs);
  }

  [Serializable]
  public struct Not : IOp_Function, IOpRModifier {

    [field: SerializeField]
    public int RMode { get; set; }

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      var arg = machine.PopArg();

      switch (RMode) {
        case 0:
          machine.HL = (ushort)(~(arg & 0xFF));
          break;
        case 1:
          machine.HL = (ushort)(~arg);
          break;
      }
    }
  }

  [Serializable]
  public struct Max : IOp_Function {

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      ushort lhs = machine.PopArg();
      ushort rhs = machine.PopArg();
      machine.HL = lhs > rhs ? lhs : rhs;
    }
  }

  [Serializable]
  public struct Abs : IOp_Function {

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      var val = (short)machine.PopArg();
      if (val < 0) {
        var negated = -val;
        if (negated < ushort.MinValue) {
          negated = ushort.MinValue;
        } else if (negated > ushort.MaxValue) {
          negated = ushort.MaxValue;
        }
        val = (short)negated;
      }
      machine.HL = (ushort)val;
    }
  }

  [Serializable]
  public struct Min : IOp_Function {

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      ushort lhs = machine.PopArg();
      ushort rhs = machine.PopArg();
      machine.HL = lhs < rhs ? lhs : rhs;
    }
  }

  [Serializable]
  public struct Rand : IOp {

    public void Execute(ref MachineStateNative machine) {
      var r = machine.Random;
      machine.HL = (ushort)r.NextUInt(0, ushort.MaxValue);
      machine.Random = r;
    }
  }

  [Serializable]
  public struct Sin : IOp_Function {

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      ushort arg = machine.PopArg();
      machine.HL = (ushort)Mathf.RoundToInt(127 * Mathf.Sin(arg / 256f * Mathf.PI * 2));
    }
  }

  [Serializable]
  public struct Cos : IOp_Function {

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      ushort arg = machine.PopArg();
      machine.HL = (ushort)Mathf.RoundToInt(127 * Mathf.Cos(arg / 256f * Mathf.PI * 2));
    }
  }

  [Serializable]
  public struct SquareRoot : IOp_Function {

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      var arg = machine.PopArg();
      machine.HL = (ushort)Mathf.FloorToInt(Mathf.Sqrt(arg));
    }
  }
}
