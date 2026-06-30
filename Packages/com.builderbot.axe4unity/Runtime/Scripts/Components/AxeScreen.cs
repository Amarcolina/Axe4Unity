using Unity.Jobs;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Axe4Unity {
  using static Constants;

  public class AxeScreen : MonoBehaviour {

    [Tooltip("The renderer supply the screen texture to.  _MainTex will be driven via MaterialProperly block modification.")]
    public Renderer Renderer;

    [Tooltip("The UI Image to apply the texture to.")]
    public RawImage UIImage;

    [Tooltip("Should pixel response-time be simulated?  If true, a rough sim of LCD hardware is done to improve feel.")]
    public bool UseResponseTime = true;

    [Tooltip("Should the generated texture generate mip-maps?  Only useful if you expect to be mapping the texture onto 3D geometry or scaling it")]
    public bool UseMips = false;

    [Tooltip("How frequently the texture is updated.  Affects the pixel response time simulation.")]
    public int UpdateFPS = 60;

    [Tooltip("Scales the texture up from the native 96x64 resolution.  Easily gives a pleasant bilinear smoothness between pixels.")]
    public int Scale = 4;

    [Tooltip("Pixels to pad the screen before scaling the image.")]
    public int Padding = 1;

    [Range(0, 1)]
    [Tooltip("How quickly a pixel will turn on if response time simulation is enabled.")]
    public float TurnOnLerp = 0.5f;

    [Range(0, 1)]
    [Tooltip("How quickly a pixel will turn off if response time simulation is enabled.")]
    public float TurnOffLerp = 0.5f;

    private MaterialPropertyBlock _mpb;
    private Texture2D _screen;

    private float _residual;

    private NativeArray<Color32> _rawScreen;
    private NativeArray<Color> _interpolatedScreen;

    private bool _hasCreated;

    private void Start() {
      CreateIfNeeded();
    }

    private void OnDestroy() {
      _rawScreen.Dispose();
      _interpolatedScreen.Dispose();
    }

    private void LateUpdate() {
      _residual -= Time.deltaTime;
      if (_residual < 0) {
        int framesToSim = 0;
        while (_residual < 0) {
          _residual += 1f / UpdateFPS;
          framesToSim++;
        }

        new BufferResponseTimeJob() {
          Src = _rawScreen,
          Dst = _interpolatedScreen,
          TurnOnLerp = UseResponseTime ? TurnOnLerp : 1,
          TurnOffLerp = UseResponseTime ? TurnOffLerp : 1,
          Frames = framesToSim
        }.Run();
        new BufferScaleUpJob() {
          Src = _interpolatedScreen,
          Dst = _screen.GetPixelData<Color32>(mipLevel: 0),
          Scale = Scale,
          Padding = Padding
        }.Run();
        _screen.Apply(updateMipmaps: UseMips, makeNoLongerReadable: false);
      }
    }

    public void UpdateScreen(AxeRunner runner, int rMode) {
      CreateIfNeeded();

      switch (rMode) {
        case 0:
          new BufferToTextureJob() {
            Buffer = runner.Machine.State.GetBuffer(Machine.ADDR_SCREEN_FRONT, SCREEN_BYTES),
            Pixels = _rawScreen,
          }.Run();
          break;
        case 1:
          new BufferToTexture3ColorGreyscaleJob() {
            Memory = runner.Machine.State.Memory,
            AddrFront = Machine.ADDR_SCREEN_FRONT,
            AddrBack = Machine.ADDR_SCREEN_BACK,
            Pixels = _rawScreen,
          }.Run();
          break;
        case 2:
          new BufferToTexture4ColorGreyscaleJob() {
            Memory = runner.Machine.State.Memory,
            AddrFront = Machine.ADDR_SCREEN_FRONT,
            AddrBack = Machine.ADDR_SCREEN_BACK,
            Pixels = _rawScreen,
          }.Run();
          break;
      }
    }

    private void CreateIfNeeded() {
      if (_hasCreated) {
        return;
      }
      _hasCreated = true;

      _mpb = new();

      _screen = new Texture2D((SCREEN_WIDTH + Padding * 2) * Scale, (SCREEN_HEIGHT + Padding * 2) * Scale, TextureFormat.RGBA32, mipChain: UseMips);
      _screen.filterMode = FilterMode.Bilinear;
      _screen.wrapMode = TextureWrapMode.Clamp;

      var pxData = _screen.GetPixelData<Color32>(0);
      for (int i = 0; i < pxData.Length; i++) {
        pxData[i] = new Color32(255, 255, 255, 0);
      }
      _screen.Apply();

      _rawScreen = new NativeArray<Color32>(SCREEN_WIDTH * SCREEN_HEIGHT, Allocator.Persistent);
      _interpolatedScreen = new NativeArray<Color>(SCREEN_WIDTH * SCREEN_HEIGHT, Allocator.Persistent);

      for (int i = 0; i < _rawScreen.Length; i++) {
        _rawScreen[i] = Color.white;
        _interpolatedScreen[i] = Color.white;
      }

      if (Renderer != null) {
        Renderer.GetPropertyBlock(_mpb);
        _mpb.SetTexture("_MainTex", _screen);
        Renderer.SetPropertyBlock(_mpb);
      }

      if (UIImage != null) {
        UIImage.texture = _screen;
      }
    }
  }
}
