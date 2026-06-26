using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Axe4Unity {
  using static Constants;

  [BurstCompile]
  public struct BufferToTexture3ColorGreyscaleJob : IJob {

    public NativeSlice<byte> Memory;
    public int AddrFront, AddrBack;
    public NativeSlice<Color32> Pixels;

    public void Execute() {
      for (int x = 0; x < SCREEN_WIDTH_BYTES; x++) {
        for (int y = 0; y < SCREEN_HEIGHT; y++) {
          byte bFront = Memory[AddrFront + x + y * SCREEN_WIDTH_BYTES];
          byte bBack = Memory[AddrBack + x + y * SCREEN_WIDTH_BYTES];

          for (int dx = 0; dx < 8; dx++) {
            bool pxFront = (bFront & (1 << (7 - dx))) != 0;
            bool pxBack = (bBack & (1 << (7 - dx))) != 0;

            Color32 color;
            if (pxFront) {
              color = new Color32(0, 0, 0, 255);
            } else if (pxBack) {
              color = new Color32(128, 128, 128, 255);
            } else {
              color = new Color32(255, 255, 255, 255);
            }

            Pixels[x * 8 + dx + (SCREEN_HEIGHT - 1 - y) * SCREEN_WIDTH] = color;
          }
        }
      }
    }
  }
}
