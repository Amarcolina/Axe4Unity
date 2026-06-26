using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
#if NEWTONSOFT_INSTALLED
using Newtonsoft.Json.Linq;
#endif

namespace Axe4Unity {

  public static partial class TokenLookup {

    public static Dictionary<string, Token> StringToToken;
    public static Dictionary<Token, string> TokenToString;
    public static Dictionary<char, List<(string str, Token tok)>> CharToTokens;

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Build Token Lookup")]
    static void BuildLookupCodegen() {
      using (var writer = File.CreateText("Assets/TokenLookupTable.cs")) {
        writer.WriteLine("public static partial class TokenLookup {");

        writer.WriteLine("public static Dictionary<string, Token> StringToToken2 = new() {");

        foreach ((var str, var tok) in StringToToken.OrderBy(t => t.Key)) {
          var escaped = str.Replace("\"", "\\\"").
                            Replace("\n", "\\n").
                            Replace("\\", "\\\\");


          writer.Write($"[\"{escaped}\"] = new Token(");
          if (tok.IsTwoByte) {
            writer.WriteLine($"0x{tok.Value >> 8:X2}, 0x{tok.Value & 0xFF:X2}),");
          } else {
            writer.WriteLine($"0x{tok.Value:X2}),");
          }
        }

        writer.WriteLine("};");

        writer.WriteLine("}");
      }
    }
#endif

    static TokenLookup() {
      StringToToken = new();
      TokenToString = new();
      CharToTokens = new();

#if NEWTONSOFT_INSTALLED
      LoadFromJson();
#else
      StringToToken = StringToToken_Generated;
      foreach ((var str, var tok) in StringToToken) {
        TokenToString[tok] = str;
      }
#endif

      foreach ((var str, var tok) in StringToToken) {
        if (!CharToTokens.TryGetValue(str[0], out var list)) {
          list = new();
          CharToTokens[str[0]] = list;
        }
        list.Add((str, tok));
      }
    }

#if NEWTONSOFT_INSTALLED
    private static void LoadFromJson() {
      var tokens = JObject.Parse(File.ReadAllText("Packages/com.builderbot.axe4unity/Runtime/Tokens/8X.json"));

      foreach (var pair in tokens) {
        var byte1 = ParseHex(pair.Key);

        if (pair.Value is JObject subObj) {
          foreach (var pair2 in subObj) {
            var byte2 = ParseHex(pair2.Key);
            var name = pair2.Value[0]["langs"]["en"]["accessible"].ToString();
            TokenToString[new Token() {
              IsTwoByte = true,
              Value = byte2 + (byte1 << 8)
            }] = name;
          }
        } else {
          var name = pair.Value[0]["langs"]["en"]["accessible"].ToString();
          TokenToString[new Token() {
            Value = byte1
          }] = name;
        }
      }

      foreach (var pair in TokenToString) {
        if (StringToToken.ContainsKey(pair.Value)) {
          throw new Exception();
        }

        StringToToken[pair.Value] = pair.Key;
      }
    }

    private static int ParseHex(string hex) {
      return int.Parse(hex.Substring(1), System.Globalization.NumberStyles.HexNumber);
    }
#endif
  }
}
