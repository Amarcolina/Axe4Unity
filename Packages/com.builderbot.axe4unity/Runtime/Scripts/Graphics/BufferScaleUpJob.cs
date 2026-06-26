using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;

namespace Axe4Unity {
  using static Constants;

  [BurstCompile]
  public struct BufferScaleUpJob : IJob {

    [ReadOnly]
    public NativeArray<Color> Src;
    public NativeArray<Color32> Dst;

    public int Scale;
    public int Padding;

    public void Execute() {
      int readI = 0;
      for (int y = 0; y < SCREEN_HEIGHT; y++) {
        for (int x = 0; x < SCREEN_WIDTH; x++) {
          Color32 px = Src[readI++];

          for (int dy = 0; dy < Scale; dy++) {
            for (int dx = 0; dx < Scale; dx++) {
              int nx = (x + Padding) * Scale + dx;
              int ny = (y + Padding) * Scale + dy;
              Dst[nx + ny * Scale * (96 + Padding * 2)] = px;
            }
          }
        }
      }
    }
  }
}
