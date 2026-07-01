using Unity.Collections;

namespace Axe4Unity {
  using static Constants;

  public static class Utils {

    public static FixedString32Bytes PtrToString(MachineStateNative machine, ushort ptr) {
      int length = 0;
      while (machine.Memory[ptr + length] != 0) {
        length++;
      }

      FixedString32Bytes result = new FixedString32Bytes(default, length);

      for (int i = 0; i < length; i++) {
        result[i] = machine.Memory[ptr + i];
      }

      return result;
    }

    public static void Clear<T>(this NativeArray<T> array) where T : struct {
      for (int i = 0; i < array.Length; i++) {
        array[i] = default;
      }
    }

    public static void Clear<T>(this NativeSlice<T> array) where T : struct {
      for (int i = 0; i < array.Length; i++) {
        array[i] = default;
      }
    }

    public static NativeSlice<byte> GetGraphicBufferFromArgs<OpT>(in OpT op, ref MachineStateNative machine, int argIndex) where OpT : IOp_Function, IOpRModifier {
      if (argIndex < op.ArgCount) {
        return machine.GetBuffer(machine.PopArg(), SCREEN_BYTES);
      } else {
        if (op.RMode == 1) {
          return machine.GetBuffer(Machine.ADDR_L3, SCREEN_BYTES);
        } else {
          return machine.GetBuffer(Machine.ADDR_L6, SCREEN_BYTES);
        }
      }
    }

    public static bool TryGetPixelOffsetAndMask(int x, int y, out ushort offset, out byte mask) {
      if (x < 0 || x >= SCREEN_WIDTH || y < 0 || y >= SCREEN_HEIGHT) {
        offset = 0;
        mask = 0;
        return false;
      }

      int index = x + y * SCREEN_WIDTH;
      offset = (ushort)(index >> 3);
      mask = (byte)(1 << (7 - index & 0x07));
      return true;
    }

    public static int DrawGlyph(NativeSlice<byte> buffer, BitFont.Native font, int code, int x, int y, bool invert = false) {
      int width = font.GetGlyphWidth(code);
      int height = font.Height;

      for (int dx = 0; dx < width; dx++) {
        for (int dy = 0; dy < height; dy++) {
          if (TryGetPixelOffsetAndMask(x + dx, y + dy, out var offset, out var mask)) {
            bool px = font.GetGlyphPixel(code, dx, dy) != invert;
            buffer[offset] = (byte)((buffer[offset] & ~mask) | (px ? mask : 0));
          }
        }
      }

      return width;
    }

    public static void PrintHomeScreen(ref MachineStateNative machine) {
      var bufferAddr = Machine.ADDR_L2;
      var screen = machine.GetBuffer(Machine.ADDR_SCREEN_FRONT, SCREEN_BYTES);

      for (int y = 0; y < HOME_SCREEN_HEIGHT; y++) {
        for (int x = 0; x < HOME_SCREEN_WIDTH; x++) {
          char c = (char)machine.Read_U8(bufferAddr++);
          if (!char.IsControl(c)) {
            DrawGlyph(screen, machine.LargeFont, c, x * 6, y * 8);
          } else {
            DrawGlyph(screen, machine.LargeFont, ' ', x * 6, y * 8);
          }
        }
      }
    }

    public static void ShiftUpHomeScreen(ref MachineStateNative machine) {
      var bufferAddr = Machine.ADDR_L2;
      for (int i = 0; i < HOME_SCREEN_BYTES - HOME_SCREEN_WIDTH; i++) {
        machine.Write_U8(bufferAddr, machine.Read_U8(bufferAddr + HOME_SCREEN_WIDTH));
        bufferAddr++;
      }
      for (int i = 0; i < HOME_SCREEN_WIDTH; i++) {
        machine.Write_U8(bufferAddr, ' ');
        bufferAddr++;
      }
    }

    public static int GetDrawingPoint(ushort coordinate) {
      return (sbyte)(coordinate & 0xFF);
    }
  }
}
