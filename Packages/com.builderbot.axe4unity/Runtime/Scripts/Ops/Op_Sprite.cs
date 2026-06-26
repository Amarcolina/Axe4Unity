using System;
using UnityEngine;
using Unity.Collections;

namespace Axe4Unity.Op {
  using static Constants;

  [Serializable]
  public struct PtSprite<T> : IOp_Function, IOpRModifier, IOpGraphic where T : IPlotter {

    [field: SerializeField]
    public int RMode { get; set; }

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      var buffer = Utils.GetGraphicBufferFromArgs(this, ref machine, 3);
      var sprite = machine.GetBuffer(machine.PopArg(), 8);
      int y = Utils.GetDrawingPoint(machine.PopArg());
      int x = Utils.GetDrawingPoint(machine.PopArg());

      T plotter = default;

      for (int dy = 0; dy < 8; dy++) {
        int ny = y + dy;
        if (ny < 0 || ny >= SCREEN_HEIGHT) {
          continue;
        }

        for (int dx = 0; dx < 8; dx++) {
          int nx = x + dx;
          if (nx < 0 || nx >= SCREEN_WIDTH) {
            continue;
          }

          bool px = (sprite[dy] & (1 << (7 - dx))) != 0;
          plotter.Plot(buffer, nx, ny, px);
        }
      }
    }
  }

  [Serializable]
  public struct PtMask : IOp_Function, IOpRModifier, IOpGraphic {

    [field: SerializeField]
    public int RMode { get; set; }

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      if (RMode == 0) {
        DoGreyScaleSprite(ref machine);
      } else {
        DoMaskedSprite(ref machine);
      }
    }

    private void DoGreyScaleSprite(ref MachineStateNative machine) {
      var front = machine.GetBuffer(Machine.ADDR_L6, SCREEN_BYTES);
      var back = machine.GetBuffer(Machine.ADDR_L3, SCREEN_BYTES);

      var sprite = machine.GetBuffer(machine.PopArg(), 16);
      var y = machine.PopArg();
      var x = machine.PopArg();

      PlotterOverwrite plotter = default;

      for (int dy = 0; dy < 8; dy++) {
        int ny = y + dy;
        if (ny < 0 || ny >= SCREEN_HEIGHT) {
          continue;
        }

        for (int dx = 0; dx < 8; dx++) {
          int nx = x + dx;
          if (nx < 0 || nx >= SCREEN_WIDTH) {
            continue;
          }

          bool px_0 = (sprite[dy] & (1 << (7 - dx))) != 0;
          bool px_1 = (sprite[dy + 8] & (1 << (7 - dx))) != 0;

          if (px_0 || px_1) {
            plotter.Plot(front, nx, ny, px_0);
            plotter.Plot(back, nx, ny, px_1);
          }
        }
      }
    }

    private void DoMaskedSprite(ref MachineStateNative machine) {
      NativeSlice<byte> buffer;
      if (ArgCount == 4) {
        buffer = machine.GetBuffer(machine.PopArg(), SCREEN_BYTES);
      } else {
        buffer = machine.GetBuffer(Machine.ADDR_L6, SCREEN_BYTES);
      }

      var sprite = machine.GetBuffer(machine.PopArg(), 16);
      var y = machine.PopArg();
      var x = machine.PopArg();

      PlotterOverwrite plotterOverwrite = default;
      PlotterInvert plotterInvert = default;

      for (int dy = 0; dy < 8; dy++) {
        int ny = y + dy;
        if (ny < 0 || ny >= SCREEN_HEIGHT) {
          continue;
        }

        for (int dx = 0; dx < 8; dx++) {
          int nx = x + dx;
          if (nx < 0 || nx >= SCREEN_WIDTH) {
            continue;
          }

          bool px_0 = (sprite[dy] & (1 << (7 - dx))) != 0;
          bool px_1 = (sprite[dy + 8] & (1 << (7 - dx))) != 0;
          int mode = (px_0 ? 1 : 0) + (px_1 ? 2 : 0);

          switch (mode) {
            case 0: break;
            case 1: plotterOverwrite.Plot(buffer, nx, ny, false); break;
            case 2: plotterInvert.Plot(buffer, nx, ny, true); break;
            case 3: plotterOverwrite.Plot(buffer, nx, ny, true); break;
          }
        }
      }
    }
  }

  [Serializable]
  public struct PtGet : IOp_Function, IOpRModifier {

    [field: SerializeField]
    public int RMode { get; set; }

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      ushort dstAddr;
      if (ArgCount == 4) {
        dstAddr = machine.PopArg();
      } else {
        dstAddr = Machine.ADDR_SPRITE_TMP;
      }

      var dst = machine.Memory.Slice(dstAddr, 8);
      var buffer = Utils.GetGraphicBufferFromArgs(this, ref machine, 2);
      var y = Utils.GetDrawingPoint(machine.PopArg());
      var x = Utils.GetDrawingPoint(machine.PopArg());

      for (int dy = 0; dy < 8; dy++) {
        byte line = 0;
        for (int dx = 0; dx < 8; dx++) {
          if (Utils.TryGetPixelOffsetAndMask(x + dx, y + dy, out var offset, out var mask)) {
            if ((buffer[offset] & mask) != 0) {
              line = (byte)(line | (1 << (7 - dx)));
            }
          }
        }
        dst[dy] = line;
      }

      machine.HL = dstAddr;
    }
  }

  [Serializable]
  public struct SpriteTransform<TransformT> : IOp_Function where TransformT : ISpriteTransform {

    [field: SerializeField]
    public int ArgCount { get; set; }

    public void Execute(ref MachineStateNative machine) {
      var sprite = machine.GetBuffer(machine.PopArg(), 8);
      var resultAddr = Machine.ADDR_SPRITE_TMP;
      var result = machine.GetBuffer(resultAddr, 8);

      for (int i = 0; i < 8; i++) {
        result[i] = 0;
      }

      TransformT t = default;

      for (int x = 0; x < 8; x++) {
        for (int y = 0; y < 8; y++) {
          t.Transform(x, y, out var tx, out var ty);

          int srcMask = 1 << x;
          int srcOffset = y;

          int dstMask = 1 << tx;
          int dstOffset = ty;

          if ((sprite[srcOffset] & srcMask) != 0) {
            result[dstOffset] = (byte)(result[dstOffset] | dstMask);
          }
        }
      }

      machine.HL = (ushort)resultAddr;
    }
  }

  [Serializable]
  public struct RotC : ISpriteTransform {

    public void Transform(int x, int y, out int tx, out int ty) {
      tx = y;
      ty = 7 - x;
    }
  }

  [Serializable]
  public struct RotCC : ISpriteTransform {

    public void Transform(int x, int y, out int tx, out int ty) {
      tx = 7 - y;
      ty = x;
    }
  }

  [Serializable]
  public struct FlipV : ISpriteTransform {

    public void Transform(int x, int y, out int tx, out int ty) {
      tx = x;
      ty = 7 - y;
    }
  }

  [Serializable]
  public struct FlipH : ISpriteTransform {

    public void Transform(int x, int y, out int tx, out int ty) {
      tx = 7 - x;
      ty = y;
    }
  }

  public interface ISpriteTransform {
    void Transform(int x, int y, out int tx, out int ty);
  }
}
