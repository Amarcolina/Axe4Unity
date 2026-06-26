using Unity.Collections;
using UnityEngine;

namespace Axe4Unity {

  [CreateAssetMenu]
  public class BitFont : ScriptableObject {

    public AppVarAsset Asset;
    public int Height;

    public int GetGlyphWidth(int code) {
      return Asset.Data[code * (Height + 1)];
    }

    public int GetGlyphHeight(int code) {
      return Height;
    }

    public bool GetGlyphPixel(int code, int dx, int dy) {
      return (Asset.Data[code * (Height + 1) + 1 + dy] & (1 << (7 - dx))) != 0;
    }

    public Native ToNative(Allocator allocator = Allocator.Persistent) {
      return new Native() {
        Data = new NativeArray<byte>(Asset.Data.ToArray(), allocator),
        Height = Height
      };
    }

    public struct Native {

      public NativeArray<byte> Data;
      public int Height;

      public void Dispose() {
        if (Data.IsCreated) {
          Data.Dispose();
        }
      }

      public int GetGlyphWidth(int code) {
        return Data[code * (Height + 1)];
      }

      public int GetGlyphHeight(int code) {
        return Height;
      }

      public bool GetGlyphPixel(int code, int dx, int dy) {
        return (Data[code * (Height + 1) + 1 + dy] & (1 << (7 - dx))) != 0;
      }
    }
  }
}
