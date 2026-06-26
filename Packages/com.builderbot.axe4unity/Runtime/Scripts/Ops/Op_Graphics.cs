using System;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

namespace Axe4Unity.Op {
  using static Constants;

  [Serializable]
  public struct DispGraph : IOp_Function, IOpRModifier, IOpGraphic, IOpLoopExit {

    [field: SerializeField]
    public int RMode { get; set; }

    [field: SerializeField]
    public int ArgCount { get; set; }

    bool IOpLoopExit.ShouldExit => true;

    public bool DoClrDraw;
    public bool DoRecalPic;

    public void Execute(ref MachineStateNative machine) {
      NativeSlice<byte> frontBuffer;
      NativeSlice<byte> backBuffer;

      switch (ArgCount) {
        case 0:
          frontBuffer = machine.GetBuffer(Machine.ADDR_L6, 768);
          backBuffer = machine.GetBuffer(Machine.ADDR_L3, SCREEN_BYTES);
          break;
        case 1:
          frontBuffer = machine.GetBuffer(machine.PopArg(), SCREEN_BYTES);
          backBuffer = machine.GetBuffer(Machine.ADDR_L3, SCREEN_BYTES);
          break;
        case 2:
          backBuffer = machine.GetBuffer(machine.PopArg(), SCREEN_BYTES);
          frontBuffer = machine.GetBuffer(machine.PopArg(), SCREEN_BYTES);
          break;
        default:
          throw new Exception();
      }

      switch (RMode) {
        case 0:
          machine.GetBuffer(Machine.ADDR_SCREEN_FRONT, SCREEN_BYTES).CopyFrom(frontBuffer);
          break;
        case 1:
        case 2:
          machine.GetBuffer(Machine.ADDR_SCREEN_FRONT, SCREEN_BYTES).CopyFrom(frontBuffer);
          machine.GetBuffer(Machine.ADDR_SCREEN_BACK, SCREEN_BYTES).CopyFrom(backBuffer);
          break;
      }

      if (DoClrDraw) {
        switch (RMode) {
          case 0:
            frontBuffer.Clear();
            break;
          case 1:
          case 2:
            frontBuffer.Clear();
            backBuffer.Clear();
            break;
        }
      }

      if (DoRecalPic) {
        frontBuffer.CopyFrom(backBuffer);
      }
    }
  }

  [Serializable]
  public struct StorePic : IOp {

    public void Execute(ref MachineStateNative machine) {
      var front = machine.GetBuffer(Machine.ADDR_L6, SCREEN_BYTES);
      var back = machine.GetBuffer(Machine.ADDR_L3, SCREEN_BYTES);
      back.CopyFrom(front);
    }
  }

  [Serializable]
  public struct RecallPic : IOp {

    public void Execute(ref MachineStateNative machine) {
      var front = machine.GetBuffer(Machine.ADDR_L6, SCREEN_BYTES);
      var back = machine.GetBuffer(Machine.ADDR_L3, SCREEN_BYTES);
      front.CopyFrom(back);
    }
  }

  [Serializable]
  public struct StoreGDB : IOp {

    public void Execute(ref MachineStateNative machine) {
      var screen = machine.GetBuffer(Machine.ADDR_SCREEN_FRONT, SCREEN_BYTES);
      var buffer = machine.GetBuffer(Machine.ADDR_L6, SCREEN_BYTES);
      buffer.CopyFrom(screen);
    }
  }

  [Serializable]
  public struct DrawInv : IOp_Function, IOpRModifier {

    [field: SerializeField]
    public int RMode { get; set; }

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      var buff = Utils.GetGraphicBufferFromArgs(this, ref machine, argIndex: 0);

      for (int i = 0; i < buff.Length; i++) {
        buff[i] = (byte)(buff[i] ^ 0xFF);
      }
    }
  }

  [Serializable]
  public struct ClrDraw : IOp_Function, IOpRModifier, IOpGraphic {

    [field: SerializeField]
    public int RMode { get; set; }

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      if (RMode == 2) {
        var buf0 = machine.GetBuffer(Machine.ADDR_L3, SCREEN_BYTES);
        var buf1 = machine.GetBuffer(Machine.ADDR_L6, SCREEN_BYTES);
        for (int i = 0; i < SCREEN_BYTES; i++) {
          buf0[i] = 0;
          buf1[i] = 0;
        }
      } else {
        var buf = Utils.GetGraphicBufferFromArgs(this, ref machine, argIndex: 0);
        for (int i = 0; i < SCREEN_BYTES; i++) {
          buf[i] = 0;
        }
      }
    }
  }

  [Serializable]
  public struct PxlPlot<T> : IOp_Function, IOpRModifier, IOpGraphic where T : IPlotter {

    [field: SerializeField]
    public int RMode { get; set; }

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      var buffer = Utils.GetGraphicBufferFromArgs(this, ref machine, argIndex: 2);
      int y = Utils.GetDrawingPoint(machine.PopArg());
      int x = Utils.GetDrawingPoint(machine.PopArg());

      T plotter = default;
      plotter.Plot(buffer, x, y, true);
    }
  }

  [Serializable]
  public struct PxlTest : IOp_Function, IOpRModifier {

    [field: SerializeField]
    public int RMode { get; set; }

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      var buffer = Utils.GetGraphicBufferFromArgs(this, ref machine, argIndex: 2);
      int y = Utils.GetDrawingPoint(machine.PopArg());
      int x = Utils.GetDrawingPoint(machine.PopArg());

      if (Utils.TryGetPixelOffsetAndMask(x, y, out var offset, out var mask)) {
        machine.HL = (ushort)((buffer[offset] & mask) != 0 ? 1 : 0);
      } else {
        machine.HL = 0;
      }
    }
  }

  [Serializable]
  public struct Line<T> : IOp_Function, IOpRModifier, IOpGraphic where T : IPlotter {

    [field: SerializeField]
    public int RMode { get; set; }

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      var buffer = Utils.GetGraphicBufferFromArgs(this, ref machine, argIndex: 4);
      int y1 = machine.PopArg();
      int x1 = machine.PopArg();
      int y0 = machine.PopArg();
      int x0 = machine.PopArg();

      if (x0 >= SCREEN_WIDTH || x1 >= SCREEN_WIDTH || y0 >= SCREEN_HEIGHT || y1 >= SCREEN_HEIGHT) {
        return;
      }

      int dy = y1 - y0;
      int dx = x1 - x0;
      int stepx, stepy;

      if (dy < 0) { dy = -dy; stepy = -1; } else { stepy = 1; }
      if (dx < 0) { dx = -dx; stepx = -1; } else { stepx = 1; }
      dy <<= 1;
      dx <<= 1;

      int fraction;
      T plotter = default;

      plotter.Plot(buffer, x0, y0, true);
      if (dx > dy) {
        fraction = dy - (dx >> 1);
        while (Mathf.Abs(x0 - x1) > 0) {
          if (fraction >= 0) {
            y0 += stepy;
            fraction -= dx;
          }
          x0 += stepx;
          fraction += dy;
          plotter.Plot(buffer, x0, y0, true);
        }
      } else {
        fraction = dx - (dy >> 1);
        while (Mathf.Abs(y0 - y1) > 0) {
          if (fraction >= 0) {
            x0 += stepx;
            fraction -= dy;
          }
          y0 += stepy;
          fraction += dx;
          plotter.Plot(buffer, x0, y0, true);
        }
      }
    }
  }

  [Serializable]
  public struct StraightLine<T> : IOp_Function, IOpRModifier, IOpGraphic where T : IPlotter {

    [field: SerializeField]
    public int RMode { get; set; }

    [field: SerializeField]
    public int ArgCount { get; set; }

    public bool Vertical;

    public void Execute(ref MachineStateNative machine) {
      T plotter = default;

      NativeSlice<byte> buffer;
      if (ArgCount == 2) {
        buffer = machine.GetBuffer(machine.PopArg(), SCREEN_BYTES);
      } else {
        buffer = Utils.GetGraphicBufferFromArgs(this, ref machine, argIndex: 5);
      }

      int x0 = 0;
      int x1 = Vertical ? (SCREEN_WIDTH - 1) : (SCREEN_HEIGHT - 1);

      if (ArgCount >= 3) {
        x1 = machine.PopArg();
        x0 = machine.PopArg();
      }

      int y = machine.PopArg();

      for (int x = x0; x <= x1; x++) {
        if (Vertical) {
          plotter.Plot(buffer, x, y, true);
        } else {
          plotter.Plot(buffer, y, x, true);
        }
      }
    }
  }

  [Serializable]
  public struct Circle<T> : IOp_Function, IOpRModifier, IOpGraphic where T : IPlotter {

    [field: SerializeField]
    public int RMode { get; set; }

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      T plotter = default;

      var buffer = Utils.GetGraphicBufferFromArgs(this, ref machine, 3);
      int radius = machine.PopArg();
      int centerY = machine.PopArg();
      int centerX = machine.PopArg();

      var center = new int2(centerX, centerY);
      var extent = new int2(radius, radius);

      if (extent.x == 0 || extent.y == 0) {
        return;
      }

      int a2 = extent.x * extent.x;
      int b2 = extent.y * extent.y;
      int fa2 = 4 * a2, fb2 = 4 * b2;
      int x, y, sigma;

      //first half
      for (x = 0, y = extent.y, sigma = 2 * b2 + a2 * (1 - 2 * extent.y); b2 * x <= a2 * y; x++) {
        int x0 = center.x + x;
        int x1 = center.x - x;
        int y0 = center.y + y;
        int y1 = center.y - y;

        plotter.Plot(buffer, x0, y0, true);
        plotter.Plot(buffer, x1, y0, true);
        plotter.Plot(buffer, x0, y1, true);
        plotter.Plot(buffer, x1, y1, true);

        if (sigma >= 0) {
          sigma += fa2 * (1 - y);
          y--;
        }
        sigma += b2 * ((4 * x) + 6);
      }

      //second half
      for (x = extent.x, y = 0, sigma = 2 * a2 + b2 * (1 - 2 * extent.x); a2 * y <= b2 * x; y++) {
        int x0 = center.x + x;
        int x1 = center.x - x;
        int y0 = center.y + y;
        int y1 = center.y - y;

        plotter.Plot(buffer, x0, y0, true);
        plotter.Plot(buffer, x1, y0, true);
        plotter.Plot(buffer, x0, y1, true);
        plotter.Plot(buffer, x1, y1, true);

        if (sigma >= 0) {
          sigma += fb2 * (1 - x);
          x--;
        }
        sigma += a2 * ((4 * y) + 6);
      }
    }
  }

  [Serializable]
  public struct Bitmap : IOp_Function, IOpRModifier, IOpGraphic {

    [field: SerializeField]
    public int RMode { get; set; }

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      int mode = 0;
      if (ArgCount == 5) {
        mode = machine.PopArg();
      }

      var buffer = Utils.GetGraphicBufferFromArgs(this, ref machine, 3);
      var bmp = machine.PopArg();
      var y = machine.PopArg();
      var x = machine.PopArg();

      int width = machine.Read_U8(bmp++);
      int height = machine.Read_U8(bmp++);
      int bytesWide = 1 + (width - 1) / 8;

      for (int dx = 0; dx < width; dx++) {
        for (int dy = 0; dy < height; dy++) {
          int bmpOffset = (dx >> 3) + dy * bytesWide;
          int bmpMask = 1 << (dx & 0b111);
          bool bmpPixel = (machine.Read_U8(bmp + bmpOffset) & bmpMask) != 0;

          switch (mode) {
            case 0:
              new PlotterOr().Plot(buffer, x + dx, y + dy, bmpPixel);
              break;
            case 1:
              new PlotterInvert().Plot(buffer, x + dx, y + dy, bmpPixel);
              break;
          }
        }
      }
    }
  }

  [Serializable]
  public struct Vertical : IOp_Function, IOpRModifier, IOpGraphic {

    [field: SerializeField]
    public int RMode { get; set; }

    [field: SerializeField]
    public int ArgCount { get; set; }

    public bool Positive;

    public void Execute(ref MachineStateNative machine) {
      var buff = Utils.GetGraphicBufferFromArgs(this, ref machine, argIndex: 0);

      if (Positive) {
        for (int r = 63; r >= 1; r--) {
          for (int i = 0; i < 12; i++) {
            buff[r * 12 + i] = buff[r * 12 + i - 12];
          }
        }
      } else {
        for (int r = 0; r <= 62; r++) {
          for (int i = 0; i < 12; i++) {
            buff[r * 12 + i] = buff[r * 12 + i + 12];
          }
        }
      }
    }
  }

  [Serializable]
  public struct Horizontal : IOp_Function, IOpRModifier, IOpGraphic {

    [field: SerializeField]
    public int RMode { get; set; }

    [field: SerializeField]
    public int ArgCount { get; set; }

    public bool Positive;

    public void Execute(ref MachineStateNative machine) {
      var buff = Utils.GetGraphicBufferFromArgs(this, ref machine, argIndex: 0);

      if (Positive) {
        for (int r = 0; r < SCREEN_HEIGHT; r++) {
          bool carry = false;
          for (int c = 0; c < 12; c++) {
            bool nextCarry = (buff[r * 12 + c] & 0b00000001) != 0;
            buff[r * 12 + c] = (byte)((buff[r * 12 + c] >> 1) |
                                      (carry ? 0b10000000 : 0));
            carry = nextCarry;
          }
        }
      } else {
        for (int r = 0; r < SCREEN_HEIGHT; r++) {
          bool carry = false;
          for (int c = 11; c >= 0; c--) {
            bool nextCarry = (buff[r * 12 + c] & 0b10000000) != 0;
            buff[r * 12 + c] = (byte)((buff[r * 12 + c] << 1) |
                                      (carry ? 0b00000001 : 0));
            carry = nextCarry;
          }
        }
      }
    }
  }

  [Serializable]
  public struct Rect<T> : IOp_Function, IOpRModifier, IOpGraphic where T : IPlotter {

    [field: SerializeField]
    public int RMode { get; set; }

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      var buffer = Utils.GetGraphicBufferFromArgs(this, ref machine, argIndex: 4);
      int h = machine.PopArg() & 0xFF;
      int w = machine.PopArg() & 0xFF;
      int y = Utils.GetDrawingPoint(machine.PopArg());
      int x = Utils.GetDrawingPoint(machine.PopArg());

      T plotter = default;

      for (int dx = 0; dx < w; dx++) {
        for (int dy = 0; dy < h; dy++) {
          plotter.Plot(buffer, x + dx, y + dy, true);
        }
      }
    }
  }
}
