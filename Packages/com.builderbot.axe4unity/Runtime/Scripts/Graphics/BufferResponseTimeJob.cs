using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;

namespace Axe4Unity {

  [BurstCompile]
  public struct BufferResponseTimeJob : IJob {

    [ReadOnly]
    public NativeArray<Color32> Src;
    public NativeArray<Color> Dst;

    public float TurnOnLerp;
    public float TurnOffLerp;

    public void Execute() {
      for (int i = 0; i < Src.Length; i++) {
        Color src = Src[i];
        Color dst = Dst[i];
        if (dst.r < src.r) {
          dst = Color.Lerp(dst, src, TurnOnLerp);
        } else {
          dst = Color.Lerp(dst, src, TurnOffLerp);
        }
        Dst[i] = dst;
      }
    }
  }
}
