using System;
using UnityEngine;

namespace Axe4Unity.Op {

  [Serializable]
  public struct Label : IOp {
    public void Execute(ref MachineStateNative machine) { }
  }

  [Serializable]
  public struct Goto : IOp, IOpControl {

    public int LabelAddress;

    int IOpControl.JumpLine { get => LabelAddress; set => throw new NotImplementedException(); }
    int IOpControl.JumpOp { get => 0; set => throw new NotImplementedException(); }

    public void Execute(ref MachineStateNative machine) {
      machine.Jump(LabelAddress);
    }
  }

  [Serializable]
  public struct GotoExpr : IOp {

    public void Execute(ref MachineStateNative machine) {
      machine.Jump(machine.HL);
    }
  }

  [Serializable]
  public struct GotoIfEq : IOp {

    public int LabelAddress;
    public ushort Value;

    public void Execute(ref MachineStateNative machine) {
      if (machine.HL == Value) {
        machine.Jump(LabelAddress);
      }
    }
  }

  [Serializable]
  public struct Return : IOp, IOpRModifier {

    [field: SerializeField]
    public int RMode { get; set; }

    public void Execute(ref MachineStateNative machine) {
      switch (RMode) {
        case 0:
          machine.CallStackTop--;
          break;
        case 1:
          machine.CallStackTop = 0;
          break;
      }
    }
  }

  [Serializable]
  public struct Call : IOp_Function, IOpControl {

    [field: SerializeField]
    public int ArgCount { get; set; }

    public int LabelAddress;

    int IOpControl.JumpLine { get => LabelAddress; set => throw new NotImplementedException(); }
    int IOpControl.JumpOp { get => 0; set => throw new NotImplementedException(); }

    public void Execute(ref MachineStateNative machine) {
      var argAddr = Machine.ADDR_CALLING_ARGS + ArgCount * 2;
      for (int i = 0; i < ArgCount; i++) {
        argAddr -= 2;
        machine.Write_U16(argAddr, machine.PopArg());
      }

      machine.CallStack[machine.CallStackTop++] = new ProgramCounter() {
        LineIndex = LabelAddress,
        OpIndex = 0
      };
    }
  }

  [Serializable]
  public struct CallAddr : IOp_Function {

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      var argAddr = Machine.ADDR_CALLING_ARGS + ArgCount * 2;
      for (int i = 0; i < ArgCount; i++) {
        argAddr -= 2;
        machine.Write_U16(argAddr, machine.PopArg());
      }

      machine.CallStack[machine.CallStackTop++] = new ProgramCounter() {
        LineIndex = machine.HL,
        OpIndex = 0
      };
    }
  }

  [Serializable]
  public struct If : IOp, IOpControl {

    [field: SerializeField]
    public int JumpLine { get; set; }

    [field: SerializeField]
    public int JumpOp { get; set; }

    public bool Negated;

    public void Execute(ref MachineStateNative machine) {
      if ((machine.HL == 0) != Negated) {
        machine.Jump(JumpLine, JumpOp);
      }
    }
  }

  [Serializable]
  public struct Else : IOp, IOpControl {

    [field: SerializeField]
    public int JumpLine { get; set; }

    [field: SerializeField]
    public int JumpOp { get; set; }

    public bool IsElseIf;

    public void Execute(ref MachineStateNative machine) {
      machine.Jump(JumpLine, JumpOp);
    }
  }

  [Serializable]
  public struct DS : IOp, IOpControl {

    [field: SerializeField]
    public int JumpLine { get; set; }

    [field: SerializeField]
    public int JumpOp { get; set; }

    public ushort VarAddress;

    public void Execute(ref MachineStateNative machine) {
      var maxValue = machine.HL;

      var value = machine.Read_U16(VarAddress);
      value--;
      machine.Write_U16(VarAddress, value);

      if (value == 0) {
        machine.Write_U16(VarAddress, maxValue);
      } else {
        machine.Jump(JumpLine, JumpOp);
      }
    }
  }

  [Serializable]
  public struct For : IOp, IOpControl {

    [field: SerializeField]
    public int JumpLine { get; set; }

    [field: SerializeField]
    public int JumpOp { get; set; }

    public ushort VarAddress;

    public void Execute(ref MachineStateNative machine) {
      ushort compValue = machine.HL;
      ushort varValue = machine.Read_U16(VarAddress);

      if (varValue > compValue) {
        machine.Jump(JumpLine, JumpOp);
      }
    }
  }

  [Serializable]
  public struct ForStack : IOp, IOpControl, IOpRModifier {

    [field: SerializeField]
    public int JumpLine { get; set; }

    [field: SerializeField]
    public int JumpOp { get; set; }

    [field: SerializeField]
    public int RMode { get; set; }

    public void Execute(ref MachineStateNative machine) {
      var counter = machine.PopArg();

      if (RMode == 1) {
        counter = (ushort)(counter & 0xFF);
      }

      if (counter == 0) {
        machine.Jump(JumpLine, JumpOp);
      } else {
        machine.HL = (ushort)(counter - 1);
        machine.PushArg();
      }
    }
  }

  [Serializable]
  public struct WhileTrue : IOp, IOpControl {

    [field: SerializeField]
    public int JumpLine { get; set; }

    [field: SerializeField]
    public int JumpOp { get; set; }

    public void Execute(ref MachineStateNative machine) { }

  }

  [Serializable]
  public struct While : IOp, IOpControl {

    [field: SerializeField]
    public int JumpLine { get; set; }

    [field: SerializeField]
    public int JumpOp { get; set; }

    public void Execute(ref MachineStateNative machine) {
      if (machine.HL == 0) {
        machine.Jump(JumpLine, JumpOp);
      }
    }
  }

  [Serializable]
  public struct Repeat : IOp, IOpControl {

    [field: SerializeField]
    public int JumpLine { get; set; }

    [field: SerializeField]
    public int JumpOp { get; set; }

    public void Execute(ref MachineStateNative machine) {
      if (machine.HL != 0) {
        machine.Jump(JumpLine, JumpOp);
      }
    }
  }

  [Serializable]
  public struct End : IOp {
    public void Execute(ref MachineStateNative machine) { }
  }

  [Serializable]
  public struct EndLoop : IOp, IOpControl {

    [field: SerializeField]
    public int JumpLine { get; set; }

    [field: SerializeField]
    public int JumpOp { get; set; }

    public void Execute(ref MachineStateNative machine) {
      machine.Jump(JumpLine, JumpOp);
    }
  }

  [Serializable]
  public struct EndFor : IOp, IOpControl {

    [field: SerializeField]
    public int JumpLine { get; set; }

    [field: SerializeField]
    public int JumpOp { get; set; }

    public ushort VarAddress;

    public void Execute(ref MachineStateNative machine) {
      machine.Write_U16(VarAddress, (ushort)(1 + machine.Read_U16(VarAddress)));
      machine.Jump(JumpLine, JumpOp);
    }
  }
}
