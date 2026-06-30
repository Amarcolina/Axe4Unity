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
    public int Frames;

    public void Execute() {
      float onLerp = 1f - Mathf.Pow(1f - TurnOnLerp, Frames);
      float offLerp = 1f - Mathf.Pow(1f - TurnOffLerp, Frames);

      for (int i = 0; i < Src.Length; i++) {
        Color src = Src[i];
        Color dst = Dst[i];
        if (dst.r < src.r) {
          dst = Color.Lerp(dst, src, onLerp);
        } else {
          dst = Color.Lerp(dst, src, offLerp);
        }
        Dst[i] = dst;
      }
    }
  }
}
