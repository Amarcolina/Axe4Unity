using System;
using UnityEngine;
using Unity.Collections;

namespace Axe4Unity.Op {
  using static Constants;

  [Serializable]
  public struct ClrHome : IOp, IOpLoopExit {

    bool IOpLoopExit.ShouldExit => true;

    public void Execute(ref MachineStateNative machine) {
      machine.GetBuffer(Machine.ADDR_SCREEN_FRONT, SCREEN_BYTES).Clear();
      machine.TextCursorX = 0;
      machine.TextCursorY = 0;
    }
  }

  [Serializable]
  public struct Disp : IOp, IOpLoopExit {

    bool IOpLoopExit.ShouldExit => true;

    public void Execute(ref MachineStateNative machine) {
      var buffer = machine.GetBuffer(Machine.ADDR_L2, HOME_SCREEN_BYTES);
      int addr = machine.HL;

      while (machine.Memory[addr] != 0) {
        var code = machine.Memory[addr++];
        if (code == 0) {
          break;
        }

        bool isNewline = code == '\n';

        if (!isNewline) {
          buffer[machine.DispCursorX + machine.DispCursorY * HOME_SCREEN_WIDTH] = code;
          machine.DispCursorX++;
        }

        if (machine.DispCursorX == HOME_SCREEN_WIDTH || isNewline) {
          machine.DispCursorX = 0;
          machine.DispCursorY++;

          if (machine.DispCursorY == HOME_SCREEN_HEIGHT) {
            machine.DispCursorX = 0;
            machine.DispCursorY = HOME_SCREEN_HEIGHT - 1;

            Utils.ShiftUpHomeScreen(ref machine);
          }
        }
      }

      Utils.PrintHomeScreen(ref machine);
    }
  }

  [Serializable]
  public struct Output : IOp_Function, IOpLoopExit {

    [field: SerializeField]
    public int ArgCount { get; set; }

    bool IOpLoopExit.ShouldExit => true;

    public void Execute(ref MachineStateNative machine) {
      switch (ArgCount) {
        default:
          throw new InvalidOperationException();
        case 1: {
          var pos = machine.PopArg();
          machine.TextCursorX = pos % 16;
          machine.TextCursorY = pos / 16;
          break;
        }
        case 2: {
          machine.TextCursorY = machine.PopArg();
          machine.TextCursorX = machine.PopArg();
          break;
        }
        case 3: {
          var buffer = machine.GetBuffer(Machine.ADDR_L2, HOME_SCREEN_BYTES);
          var str = machine.PopArg();
          machine.TextCursorY = machine.PopArg();
          machine.TextCursorX = machine.PopArg();

          var addr = str;
          while (true) {
            var code = machine.Memory[addr++];
            if (code == 0) {
              break;
            }

            int index = machine.DispCursorX + machine.DispCursorY * HOME_SCREEN_WIDTH;
            if (index >= 0 && index < HOME_SCREEN_BYTES) {
              buffer[index] = code;
            }

            machine.DispCursorX++;
            if (machine.DispCursorX == HOME_SCREEN_WIDTH) {
              machine.DispCursorX = 0;
              machine.DispCursorY++;
            }
          }
          break;
        }
      }

      Utils.PrintHomeScreen(ref machine);
    }
  }

  [Serializable]
  public struct Text : IOp_Function, IOpGraphic {

    [field: SerializeField]
    public int ArgCount { get; set; }

    public bool IsTextOnly;

    public void Execute(ref MachineStateNative machine) {
      int addr = 0;
      if (ArgCount == 3) {
        addr = machine.PopArg();
        machine.TextCursorY = machine.PopArg();
        machine.TextCursorX = machine.PopArg();
      } else if (ArgCount == 2) {
        machine.TextCursorY = machine.PopArg();
        machine.TextCursorX = machine.PopArg();
        return;
      } else if (ArgCount == 1) {
        if (IsTextOnly) {
          addr = machine.PopArg();
        } else {
          int pos = machine.PopArg();
          machine.TextCursorX = pos % 256;
          machine.TextCursorY = pos / 256;
          return;
        }
      }

      NativeSlice<byte> buffer;
      if (machine.TextToBuffer) {
        buffer = machine.GetBuffer(Machine.ADDR_L6, SCREEN_BYTES);
      } else {
        buffer = machine.GetBuffer(Machine.ADDR_SCREEN_FRONT, SCREEN_BYTES);
      }

      var font = machine.SmallFont;

      while (machine.Memory[addr] != 0) {
        var code = machine.Memory[addr++];
        var width = Utils.DrawGlyph(buffer, font, code, machine.TextCursorX, machine.TextCursorY, machine.DrawTextInvert);
        machine.TextCursorX += width;
      }
    }
  }

  [Serializable]
  public struct ToStringNumber : IOp {

    public void Execute(ref MachineStateNative machine) {
      var addr = Machine.ADDR_TEXT_TMP;
      int value = machine.HL;

      Span<byte> digits = stackalloc byte[8];
      int digitCount = 0;
      do {
        byte c = (byte)(0x30 + value % 10);
        digits[digitCount++] = c;
        value /= 10;
      } while (value != 0);

      for (int i = 0; i < digitCount; i++) {
        machine.Write_U8(addr + i, digits[digitCount - 1 - i]);
      }
      for (int i = digitCount; i < 5; i++) {
        machine.Write_U8(addr + i, 0x20);
      }
      machine.Write_U8(addr + 5, 0);

      machine.HL = (ushort)addr;
    }
  }

  [Serializable]
  public struct ToStringToken : IOp {

    public void Execute(ref MachineStateNative machine) {
      throw new NotImplementedException();
    }
  }

  [Serializable]
  public struct ToStringChar : IOp {

    public void Execute(ref MachineStateNative machine) {
      var addr = Machine.ADDR_TEXT_TMP;
      var value = machine.HL;

      machine.Write_U8(addr + 0, value);
      machine.Write_U8(addr + 1, 0);

      machine.HL = (ushort)addr;
    }
  }

  [Serializable]
  public struct ToStringHex : IOp {

    public void Execute(ref MachineStateNative machine) {
      var addr = Machine.ADDR_TEXT_TMP;
      var value = machine.HL;

      for (int i = 3; i >= 0; i--) {
        int digit = value & 0x0F;
        if (digit < 10) {
          machine.Write_U8(addr + i, (byte)('0' + value));
        } else {
          machine.Write_U8(addr + i, (byte)('A' + (value - 10)));
        }
        value /= 16;
      }

      machine.Write_U8(addr + 4, 0);

      machine.HL = (ushort)addr;
    }
  }

  [Serializable]
  public struct ToStringNewline : IOp {

    public void Execute(ref MachineStateNative machine) {
      var addr = Machine.ADDR_TEXT_TMP;

      machine.Write_U8(addr + 0, '\n');
      machine.Write_U8(addr + 1, 0);

      machine.HL = (ushort)addr;
    }
  }
}
