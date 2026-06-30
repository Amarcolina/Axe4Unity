using UnityEngine;
using System.Collections.Generic;
using System;

namespace Axe4Unity {

  public class Machine {

    public const int ADDR_PROGRAM_MEMORY = 0x4000;

    public const int ADDR_SCREEN_FRONT = 0x2000;
    public const int ADDR_SCREEN_BACK = 0x2300;

    public const int ADDR_LETTER_VARS = 0x0000;

    public const int ADDR_L1 = 0x0900;
    public const int ADDR_L2 = 0x0C00;
    public const int ADDR_L3 = 0x1000;
    public const int ADDR_L4 = 0x1300;
    public const int ADDR_L5 = 0x1600;
    public const int ADDR_L6 = 0x1900;

    public const int ADDR_CALLING_ARGS = 0x0300;
    public const int ADDR_SPRITE_TMP = 0x0180;
    public const int ADDR_TEXT_TMP = 0x0600;
    public const int ADDR_MEMKIT_CURR = 0x06100;

    public const int ADDR_FILE_HANDLE = 0x0700;

    public const int ADDR_RANDOM_STATE = 0x8700;

    public const int ADDR_FLAG_LARGE_FONT = 0x8800;
    public const int ADDR_FLAG_INVERT_TEXT = 0x8801;
    public const int ADDR_FLAG_TEXT_TO_BUFFER = 0x8802;
    public const int ADDR_FLAG_TEXT_NO_SCROLL = 0x8803;
    public const int ADDR_FLAG_LOWERCASE_ENABLED = 0x8804;
    public const int ADDR_FLAG_IS_FULL_SPEED = 0x8805;

    public const int ADDR_PEN_X = 0x86D7;
    public const int ADDR_PEN_Y = 0x86D8;
    public const int ADDR_DISP_X = 0x86D9;
    public const int ADDR_DISP_Y = 0x86DA;

    public const int ADDR_FREE_RAM = 0xA000;

    public const int FILE_INDEX_NONE = -1;

    public static readonly List<(int addr, int size, string name)> BuiltInMemoryLocations = new() {
      (ADDR_LETTER_VARS, 27 * 2, "__Alpha"),
      (ADDR_SPRITE_TMP, 8, "__SpriteTmp"),
      (ADDR_CALLING_ARGS, 6 * 2, "__SubArgs"),
      (ADDR_TEXT_TMP, 8, "__TmpText"),

      (ADDR_MEMKIT_CURR, 2, "__MemKitIndex"),

      (ADDR_FILE_HANDLE + 6 * 0 + 0, 2, "__Y1Size"),
      (ADDR_FILE_HANDLE + 6 * 0 + 2, 2, "`Y1"),
      (ADDR_FILE_HANDLE + 6 * 0 + 4, 2, "__Y1ID"),

      (ADDR_FILE_HANDLE + 6 * 1 + 0, 2, "__Y2Size"),
      (ADDR_FILE_HANDLE + 6 * 1 + 2, 2, "`Y2"),
      (ADDR_FILE_HANDLE + 6 * 1 + 4, 2, "__Y2ID"),

      (ADDR_FILE_HANDLE + 6 * 2 + 0, 2, "__Y3Size"),
      (ADDR_FILE_HANDLE + 6 * 2 + 2, 2, "`Y3"),
      (ADDR_FILE_HANDLE + 6 * 2 + 4, 2, "__Y3ID"),

      (ADDR_FILE_HANDLE + 6 * 3 + 0, 2, "__Y4Size"),
      (ADDR_FILE_HANDLE + 6 * 3 + 2, 2, "`Y4"),
      (ADDR_FILE_HANDLE + 6 * 3 + 4, 2, "__Y4ID"),

      (ADDR_FILE_HANDLE + 6 * 4 + 0, 2, "__Y5Size"),
      (ADDR_FILE_HANDLE + 6 * 4 + 2, 2, "`Y5"),
      (ADDR_FILE_HANDLE + 6 * 4 + 4, 2, "__Y5ID"),

      (ADDR_FILE_HANDLE + 6 * 5 + 0, 2, "__Y6Size"),
      (ADDR_FILE_HANDLE + 6 * 5 + 2, 2, "`Y6"),
      (ADDR_FILE_HANDLE + 6 * 5 + 4, 2, "__Y6ID"),

      (ADDR_FILE_HANDLE + 6 * 6 + 0, 2, "__Y7Size"),
      (ADDR_FILE_HANDLE + 6 * 6 + 2, 2, "`Y7"),
      (ADDR_FILE_HANDLE + 6 * 6 + 4, 2, "__Y7ID"),

      (ADDR_FILE_HANDLE + 6 * 7 + 0, 2, "__Y8Size"),
      (ADDR_FILE_HANDLE + 6 * 7 + 2, 2, "`Y8"),
      (ADDR_FILE_HANDLE + 6 * 7 + 4, 2, "__Y8ID"),

      (ADDR_FILE_HANDLE + 6 * 8 + 0, 2, "__Y9Size"),
      (ADDR_FILE_HANDLE + 6 * 8 + 2, 2, "`Y9"),
      (ADDR_FILE_HANDLE + 6 * 8 + 4, 2, "__Y9ID"),

      (ADDR_FILE_HANDLE + 6 * 9 + 0, 2, "__Y1Size"),
      (ADDR_FILE_HANDLE + 6 * 9 + 2, 2, "`Y0"),
      (ADDR_FILE_HANDLE + 6 * 9 + 4, 2, "__Y1ID"),

      (ADDR_L1, 768, "L1"),
      (ADDR_L2, 531, "L2"),
      (ADDR_L3, 768, "L3"),
      (ADDR_L4, 256, "L4"),
      (ADDR_L5, 128, "L5"),
      (ADDR_L6, 768, "L6"),

      (ADDR_SCREEN_FRONT, 768, "__ScreenFront"),
      (ADDR_SCREEN_BACK, 768, "__ScreenBack"),

      (0x4000, 0x4000, "__ProgramMemory"),

      (ADDR_PEN_X, 1, "__PenX"),
      (ADDR_PEN_Y, 1, "__PenY"),

      (ADDR_RANDOM_STATE, 4, "__RandomState"),
      (ADDR_FLAG_LARGE_FONT, 1, "__Flag_LargeFont"),
      (ADDR_FLAG_INVERT_TEXT, 1, "__Flag_InvertText"),
      (ADDR_FLAG_TEXT_TO_BUFFER, 1, "__Flag_TextToBuffer"),
      (ADDR_FLAG_TEXT_NO_SCROLL, 1, "__Flag_TextNoScroll"),
      (ADDR_FLAG_LOWERCASE_ENABLED, 1, "__Flag_LowercaseEnabled"),
      (ADDR_FLAG_IS_FULL_SPEED, 1, "__Flag_IsFullSpeed"),

      (ADDR_FREE_RAM, 1024, "__FreeRam"),
    };

    public static readonly List<(string name, string memoryLocation, int offset)> BuiltInVariables = new() {
      ("{r1}", "__SubArgs", 0),
      ("{r2}", "__SubArgs", 2),
      ("{r3}", "__SubArgs", 4),
      ("{r4}", "__SubArgs", 6),
      ("{r5}", "__SubArgs", 8),
      ("{r6}", "__SubArgs", 10),

      ("A", "__Alpha", 0),
      ("B", "__Alpha", 2),
      ("C", "__Alpha", 4),
      ("D", "__Alpha", 6),
      ("E", "__Alpha", 8),
      ("F", "__Alpha", 10),
      ("G", "__Alpha", 12),
      ("H", "__Alpha", 14),
      ("I", "__Alpha", 16),
      ("J", "__Alpha", 18),
      ("K", "__Alpha", 20),
      ("L", "__Alpha", 22),
      ("M", "__Alpha", 24),
      ("N", "__Alpha", 26),
      ("O", "__Alpha", 28),
      ("P", "__Alpha", 30),
      ("Q", "__Alpha", 32),
      ("R", "__Alpha", 34),
      ("S", "__Alpha", 36),
      ("T", "__Alpha", 38),
      ("U", "__Alpha", 40),
      ("V", "__Alpha", 42),
      ("W", "__Alpha", 44),
      ("X", "__Alpha", 46),
      ("Y", "__Alpha", 48),
      ("Z", "__Alpha", 50),
      ("theta", "__Alpha", 52),
    };

    public MachineStateNative State;

    [NonSerialized]
    public Program Program;

    public OpAndMetaData NextOp {
      get {
        if (State.CallStackTop == 0) {
          return null;
        } else {
          var pc = State.PC;
          return Program.Lines[pc.LineIndex].Ops[pc.OpIndex];
        }
      }
    }

    public static bool TryGetAddressOfBuiltInStaticVariable(string name, out int addr) {
      foreach ((var a, _, var n) in BuiltInMemoryLocations) {
        if (n == name) {
          addr = a;
          return true;
        }
      }
      addr = 0;
      return false;
    }

    public static bool TryGetAddressOfBuiltInVariable(string name, out int addr) {
      foreach ((var n, var m, var o) in BuiltInVariables) {
        if (name == n && TryGetAddressOfBuiltInStaticVariable(m, out var memAddr)) {
          addr = memAddr + o;
          return true;
        }
      }
      addr = 0;
      return false;
    }

    public Machine(Program program, BitFont largeFont = null, BitFont smallFont = null) {
      Program = program;

      State.Init(largeFont, smallFont);

      State.Reset(this);
    }

    public void Dispose() {
      State.Dispose();
    }

    public void Reset() {
      State.Reset(this);
    }

    public void ResetAllFiles() {
      State.ResetAllFiles();
    }

    public void AddToArchive(string name, List<byte> data) {
      var file = State.CreateArchiveFile(name, data.Count);

      for (int i = 0; i < data.Count; i++) {
        file[i] = data[i];
      }
    }

    public bool TryAddToRAM(string name, List<byte> data) {
      if (!State.TryCreateRAMFile(name, data.Count, out var addr)) {
        return false;
      }

      for (int i = 0; i < data.Count; i++) {
        State.Write_U8(addr + i, data[i]);
      }

      return true;
    }

    public OpAndMetaData Step() {
      if (State.CallStackTop == 0) {
        return null;
      }

      var pc = State.PC;

      if (pc.LineIndex < 0 || pc.LineIndex >= Program.Lines.Count) {
        throw new InvalidOperationException($"Tried to execute invalid line index {pc.LineIndex}");
      }
      var line = Program.Lines[pc.LineIndex];

      if (pc.OpIndex < 0 || pc.OpIndex >= line.Ops.Count) {
        throw new InvalidOperationException($"Tried to execute invalid op index {pc.OpIndex} on line {pc.LineIndex}");
      }

      var toExecute = line.Ops[pc.OpIndex];

      pc.OpIndex++;
      if (pc.OpIndex >= line.Ops.Count) {
        pc.OpIndex = 0;
        pc.LineIndex++;
      }

      State.PC = pc;

      try {
        toExecute.Op.Execute(ref State);
      } catch (Exception) {
        Debug.LogError($"Encountered error when trying to execute op {toExecute.Op.GetType().FullName}\n" +
                       $"Line: {toExecute.Row}\n" +
                       $"Expr: {Program.Lines[toExecute.Row].Text}");
        throw;
      }

      return toExecute;
    }

    public void SetKeyIsPressed(int code, bool pressed) {
      if (pressed && !State.PressedKeys[code]) {
        State.LastKeyPressed = code;
      }
      State.PressedKeys[code] = pressed;
    }

    public int AddressOfName(string name) {
      int addr;
      if (TryGetAddressOfBuiltInStaticVariable(name, out addr) ||
         TryGetAddressOfBuiltInVariable(name, out addr) ||
         Program.TryGetVarAddress(name, out addr)) {
        return addr;
      } else {
        throw new InvalidOperationException($"Could not find variable with name {name}");
      }
    }
  }
}
