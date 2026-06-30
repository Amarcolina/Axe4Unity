using UnityEngine;
using UnityEditor;
using System.Text;
using System.Collections.Generic;

namespace Axe4Unity {

  public class DebuggerAddonHexViewer : DebuggerAddon {

    public const int BLOCK_SIZE = 8;
    public const int CHAR_WIDTH = 8;

    public const string HEX_COLOR_ZERO = "#BB2222";
    public const string HEX_COLOR_ASCII = "#55BB55";
    public const string HEX_COLOR_FF = "#BB22BB";
    public const string HEX_COLOR_OTHER = "#666666";

    public Debugger.ExpressionView Address = new() {
      Expr = "|E4000"
    };
    public int Offset = 0;
    public int BlockCount = 1;

    private StringBuilder _str = new();

    private List<LineContentCache> _lineCache = new();
    private byte[] _lineValues = new byte[8];

    [MenuItem("Axe/Hex Viewer", priority = 102)]
    private static void Init() {
      GetWindow<DebuggerAddonHexViewer>().Show();
    }

    protected override void DrawAddon() {
      if (Address.OnGUI()) {
        Offset = 0;
      }
      Address.Parse(0, Debugger.Instance.Machine);
      Address.Execute(Debugger.Instance.Machine);

      Offset = EditorGUILayout.IntField("Address Offset", Offset);
      BlockCount = EditorGUILayout.IntField("Block Count:", BlockCount);

      ushort address = (ushort)(Address.CurrValue + Offset);
      Vector2 pos = Vector2.zero;
      int line = 0;
      pos.y += 70;

      var prevSkin = GUI.skin;
      GUI.skin = Debugger.Instance.DebuggerSkin;

      if (Event.current.type == EventType.ScrollWheel) {
        if (Event.current.delta.y > 0) {
          Offset += BlockCount * 8;
        } else {
          Offset -= BlockCount * 8;
        }
        Event.current.Use();
      }

      int lineWidth = BlockCount * BLOCK_SIZE;
      if (_lineValues.Length != lineWidth) {
        _lineValues = new byte[lineWidth];
      }

      while (true) {
        if (line >= _lineCache.Count) {
          _lineCache.Add(new LineContentCache());
        }
        var lineCache = _lineCache[line];

        for (int i = 0; i < lineWidth; i++) {
          _lineValues[i] = (byte)Machine.State.Read_U8((ushort)(address + i));
        }

        if (!lineCache.IsFresh(address, _lineValues)) {
          _str.Clear();

          string currColor = HEX_COLOR_OTHER;
          _str.Append($"<color={HEX_COLOR_OTHER}>");
          _str.Append($"0x{address:X4}");
          _str.Append("  ");

          int offset = 0;
          for (int j = 0; j < BlockCount; j++) {
            for (int i = 0; i < BLOCK_SIZE; i++) {
              byte val = _lineValues[offset++];
              string valCol = GetColorForByte(val);

              if (valCol != currColor) {
                _str.Append("</color>");
                _str.Append($"<color={valCol}>");
                currColor = valCol;
              }

              _str.Append($"{val:X2}");
              _str.Append(" ");
            }

            _str.Append(" ");
          }

          offset = 0;
          for (int j = 0; j < BlockCount; j++) {
            for (int i = 0; i < BLOCK_SIZE; i++) {
              byte val = _lineValues[offset++];
              char c = (char)val;

              string valCol = GetColorForByte(val);
              if (valCol != currColor) {
                _str.Append("</color>");
                _str.Append($"<color={valCol}>");
                currColor = valCol;
              }

              if (char.IsControl(c) || (c > 128)) {
                _str.Append('.');
              } else {
                _str.Append(c);
              }
            }
          }

          _str.Append("</color>  ");

          lineCache.Address = address;
          lineCache.Content = new GUIContent(_str.ToString());
          lineCache.UpdateTexture(_lineValues);
          lineCache.MarkFresh(_lineValues);
        }

        var lineRect = new Rect(pos, new Vector2(position.width, Debugger.PROGRAM_LINE_HEIGHT));
        GUI.Label(lineRect, lineCache.Content);

        Vector2 endOfLinePos = GUI.skin.label.GetCursorPixelPosition(lineRect, lineCache.Content, lineCache.Content.text.Length);
        GUI.DrawTexture(new Rect(new Vector2(endOfLinePos.x, pos.y), new Vector2(24 * BlockCount, 24)), lineCache.TextureContent);

        pos.y += Debugger.PROGRAM_LINE_HEIGHT;
        address += (ushort)lineWidth;
        line += 1;

        if (pos.y > position.height) {
          break;
        }
      }

      GUI.skin = prevSkin;
    }

    private string GetColorForByte(byte val) {
      string valCol;
      if (val == 0) {
        valCol = HEX_COLOR_ZERO;
      } else if (val == 0xFF) {
        valCol = HEX_COLOR_FF;
      } else if (char.IsWhiteSpace((char)val)) {
        valCol = HEX_COLOR_OTHER;
      } else if (val >= 0x20 && val <= 0x7E) {
        valCol = HEX_COLOR_ASCII;
      } else {
        valCol = HEX_COLOR_OTHER;
      }
      return valCol;
    }

    public class LineContentCache {

      public byte[] Values = null;

      public ushort Address;
      public GUIContent Content;
      public Texture2D TextureContent;

      public bool IsFresh(ushort address, byte[] values) {
        if (Address != address) {
          return false;
        }

        if (Values == null || Values.Length != values.Length) {
          return false;
        }

        for (int i = 0; i < values.Length; i++) {
          if (values[i] != Values[i]) {
            return false;
          }
        }
        return true;
      }

      public void UpdateTexture(byte[] values) {
        if (TextureContent == null || TextureContent.width != values.Length) {
          if (TextureContent != null) {
            DestroyImmediate(TextureContent);
          }
          TextureContent = new Texture2D(values.Length, 8, TextureFormat.RGBA32, mipChain: false);
          TextureContent.filterMode = FilterMode.Point;
          TextureContent.wrapMode = TextureWrapMode.Clamp;
        }

        int sprites = values.Length / 8;
        for (int i = 0; i < sprites; i++) {
          for (int j = 0; j < 8; j++) {
            byte line = values[j + i * 8];
            for (int k = 0; k < 8; k++) {
              if ((line & 0b1) != 0) {
                TextureContent.SetPixel(7 - k + i * 8, 7 - j, Color.black);
              } else {
                TextureContent.SetPixel(7 - k + i * 8, 7 - j, Color.white);
              }
              line /= 2;
            }
          }
        }

        TextureContent.Apply(updateMipmaps: false, makeNoLongerReadable: false);
      }

      public void MarkFresh(byte[] values) {
        if (Values == null || Values.Length != values.Length) {
          Values = new byte[values.Length];
        }

        values.CopyTo(Values, 0);
      }
    }

  }
}
