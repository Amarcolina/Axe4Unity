using Unity.Collections;

namespace Axe4Unity {

  public interface IPlotter {
    void Plot(NativeSlice<byte> buffer, int x, int y, bool on);
  }

  public struct PlotterOverwrite : IPlotter {

    public void Plot(NativeSlice<byte> buffer, int x, int y, bool on) {
      if (Utils.TryGetPixelOffsetAndMask(x, y, out var offset, out var mask)) {
        buffer[offset] = (byte)((buffer[offset] & ~mask) | (on ? mask : 0));
      }
    }
  }

  public struct PlotterErase : IPlotter {

    public void Plot(NativeSlice<byte> buffer, int x, int y, bool on) {
      if (Utils.TryGetPixelOffsetAndMask(x, y, out var offset, out var mask)) {
        buffer[offset] = (byte)(buffer[offset] & ~mask);
      }
    }
  }

  public struct PlotterOr : IPlotter {

    public void Plot(NativeSlice<byte> buffer, int x, int y, bool on) {
      if (Utils.TryGetPixelOffsetAndMask(x, y, out var offset, out var mask)) {
        buffer[offset] = (byte)(buffer[offset] | (on ? mask : 0));
      }
    }
  }

  public struct PlotterAnd : IPlotter {

    public void Plot(NativeSlice<byte> buffer, int x, int y, bool on) {
      if (Utils.TryGetPixelOffsetAndMask(x, y, out var offset, out var mask)) {
        buffer[offset] = (byte)(buffer[offset] & (on ? mask : 0));
      }
    }
  }

  public struct PlotterInvert : IPlotter {

    public void Plot(NativeSlice<byte> buffer, int x, int y, bool on) {
      if (Utils.TryGetPixelOffsetAndMask(x, y, out var offset, out var mask)) {
        buffer[offset] = (byte)(buffer[offset] ^ (on ? mask : 0));
      }
    }
  }
}
