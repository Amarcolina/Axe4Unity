using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Axe4Unity {
  using static Constants;

  [BurstCompile]
  public struct BufferToTextureJob : IJob {

    public NativeSlice<byte> Buffer;
    public NativeSlice<Color32> Pixels;

    public void Execute() {
      for (int x = 0; x < SCREEN_WIDTH_BYTES; x++) {
        for (int y = 0; y < SCREEN_HEIGHT; y++) {
          byte b = Buffer[x + y * SCREEN_WIDTH_BYTES];

          for (int dx = 0; dx < 8; dx++) {
            Pixels[x * 8 + dx + (SCREEN_HEIGHT - 1 - y) * SCREEN_WIDTH] = (b & (1 << (7 - dx))) != 0 ? new Color32(0, 0, 0, 255) : new Color32(255, 255, 255, 255);
          }
        }
      }
    }
  }
}
