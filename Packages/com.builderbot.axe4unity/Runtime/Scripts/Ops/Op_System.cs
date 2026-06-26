using System;
using UnityEngine;

namespace Axe4Unity.Op {

  [Serializable]
  public struct Fix : IOp_Function {

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      var arg = machine.PopArg();
      switch (arg) {
        case 0: machine.LargeSizeFont = false; break;
        case 1: machine.LargeSizeFont = true; break;

        case 2: machine.DrawTextInvert = false; break;
        case 3: machine.DrawTextInvert = true; break;

        case 4: machine.TextToBuffer = false; break;
        case 5: machine.TextToBuffer = true; break;

        case 6: machine.TextNoScroll = false; break;
        case 7: machine.TextNoScroll = true; break;

        case 8: machine.LowercaseEnabled = false; break;
        case 9: machine.LowercaseEnabled = true; break;
      }
    }
  }

  [Serializable]
  public struct Full : IOp {

    public void Execute(ref MachineStateNative machine) {
      machine.IsFullSpeed = true;
      machine.HL = 1;
    }
  }

  [Serializable]
  public struct Normal : IOp {

    public void Execute(ref MachineStateNative machine) {
      machine.IsFullSpeed = false;
    }
  }

  [Serializable]
  public struct Pause : IOp_Function, IOpLoopExit {

    [field: SerializeField]
    public int ArgCount { get; set; }

    bool IOpLoopExit.ShouldExit => true;

    public void Execute(ref MachineStateNative machine) {
      machine.HL = machine.PopArg();
    }
  }

  [Serializable]
  public struct GetKey : IOp_Function, IOpRModifier, IOpLoopExit {

    [field: SerializeField]
    public int RMode { get; set; }

    [field: SerializeField]
    public int ArgCount { get; set; }

    bool IOpLoopExit.ShouldExit => RMode == 1;

    public void Execute(ref MachineStateNative machine) {
      if (RMode == 1) {
        //Handled by the runner, not implemented on machine level
        return;
      }

      switch (ArgCount) {
        default:
          throw new();
        case 0: {
          machine.HL = (ushort)machine.LastKeyPressed;
          machine.LastKeyPressed = 0;
          break;
        }
        case 1: {
          var key = machine.PopArg();

          if (key == 0) {
            machine.HL = 0;
            for (int i = 0; i < machine.PressedKeys.Length; i++) {
              if (machine.PressedKeys[i]) {
                machine.HL = 1;
                break;
              }
            }
          } else {
            machine.HL = (ushort)(machine.GetKey(key) ? 1 : 0);
          }
          break;
        }
      }
    }
  }
}
