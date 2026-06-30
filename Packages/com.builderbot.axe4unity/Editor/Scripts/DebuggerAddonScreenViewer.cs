using UnityEngine;
using UnityEditor;
using Unity.Collections;
using Unity.Jobs;

namespace Axe4Unity {
  using static Constants;

  public class DebuggerAddonScreenViewer : DebuggerAddon {

    [MenuItem("Axe/Screen Viewer", priority = 101)]
    private static void Init() {
      GetWindow<DebuggerAddonScreenViewer>().Show();
    }

    private GreyPreview _greyPreview;
    private Texture2D _screenPreview;
    private Texture2D _frontBuffer;
    private Texture2D _backBuffer;

    public Debugger.ExpressionView FrontExpr = new Debugger.ExpressionView() {
      Expr = "L6"
    };
    public Debugger.ExpressionView BackExpr = new Debugger.ExpressionView() {
      Expr = "L3"
    };

    private void OnEnable() {
      Texture2D CreateBuffer() {
        var buffer = new Texture2D(SCREEN_WIDTH, SCREEN_HEIGHT, TextureFormat.RGBA32, mipChain: false);
        buffer.filterMode = FilterMode.Point;
        return buffer;
      }

      _screenPreview = CreateBuffer();
      _frontBuffer = CreateBuffer();
      _backBuffer = CreateBuffer();
    }

    private void OnDisable() {
      DestroyImmediate(_screenPreview);
      DestroyImmediate(_frontBuffer);
      DestroyImmediate(_backBuffer);
    }

    protected override void DrawAddon() {
      if (Event.current.type == EventType.Repaint) {
        switch (_greyPreview) {
          case GreyPreview.Grey3:
            new BufferToTexture3ColorGreyscaleJob() {
              Memory = Machine.State.Memory,
              AddrBack = Machine.ADDR_L3,
              AddrFront = Machine.ADDR_L6,
              Pixels = _screenPreview.GetPixelData<Color32>(0)
            }.Run();
            break;
          case GreyPreview.Grey4:
            new BufferToTexture4ColorGreyscaleJob() {
              Memory = Machine.State.Memory,
              AddrBack = Machine.ADDR_L3,
              AddrFront = Machine.ADDR_L6,
              Pixels = _screenPreview.GetPixelData<Color32>(0)
            }.Run();
            break;
        }

        _frontBuffer.Apply();
        _backBuffer.Apply();
        _screenPreview.Apply();
      }

      DrawTexLayout("Front Buffer: ", FrontExpr, _frontBuffer);
      DrawTexLayout("Back Buffer: ", BackExpr, _backBuffer);

      if (Event.current.type == EventType.Repaint) {
        switch (_greyPreview) {
          case GreyPreview.Grey3:
            new BufferToTexture3ColorGreyscaleJob() {
              Memory = Machine.State.Memory,
              AddrBack = BackExpr.CurrValue,
              AddrFront = FrontExpr.CurrValue,
              Pixels = _screenPreview.GetPixelData<Color32>(0)
            }.Run();
            break;
          case GreyPreview.Grey4:
            new BufferToTexture4ColorGreyscaleJob() {
              Memory = Machine.State.Memory,
              AddrBack = BackExpr.CurrValue,
              AddrFront = FrontExpr.CurrValue,
              Pixels = _screenPreview.GetPixelData<Color32>(0)
            }.Run();
            break;
        }

        _screenPreview.Apply();
      }

      using (new GUILayout.HorizontalScope()) {
        GUILayout.Label("Grey");
        if (GUILayout.Button("Off")) {
          _greyPreview = GreyPreview.None;
        }
        if (GUILayout.Button("3")) {
          _greyPreview = GreyPreview.Grey3;
        }
        if (GUILayout.Button("4")) {
          _greyPreview = GreyPreview.Grey4;
        }
      }

      if (_greyPreview != GreyPreview.None) {
        DrawTexLayout(_screenPreview);
      }
    }

    private void DrawTexLayout(Texture2D tex) {
      float width = position.width;
      float scale = Mathf.Floor(width / 96f);

      var r = GUILayoutUtility.GetRect(tex.width * scale, tex.height * scale);
      r.width = tex.width * scale;
      r.height = tex.height * scale;
      GUI.DrawTexture(r, tex);
    }

    private void DrawTexLayout(string label, Debugger.ExpressionView expr, Texture2D tex) {
      expr.OnGUI(label);

      if (Event.current.type == EventType.Repaint) {
        expr.Parse(0, Machine);
        expr.Execute(Machine);

        new BufferToTextureJob() {
          Buffer = Machine.State.Memory.Slice(expr.CurrValue, SCREEN_BYTES),
          Pixels = tex.GetPixelData<Color32>(0)
        }.Run();
        tex.Apply();
      }

      DrawTexLayout(tex);
    }

    private enum GreyPreview {
      None,
      Grey3,
      Grey4
    }
  }
}
