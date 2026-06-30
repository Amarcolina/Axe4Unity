using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Axe4Unity {

  public class DebuggerAddonVarViewer : DebuggerAddon {

    public static HashSet<DebuggerAddonVarViewer> All = new();

    public Debugger.ExpressionView LetterOffsetView;
    public List<Debugger.ExpressionView> ExpressionViews = new();
    public string CustomAction;

    [MenuItem("Axe/Var Viewer", priority = 100)]
    private static void Init() {
      GetWindow<DebuggerAddonVarViewer>().Show();
    }

    private void OnEnable() {
      All.Add(this);

      LetterOffsetView = new();

      ExpressionViews.Clear();
      ExpressionViews.Add(new Debugger.ExpressionView() {
        Expr = "HL"
      });
      for (int i = 0; i < 26; i++) {
        ExpressionViews.Add(new Debugger.ExpressionView() {
          Expr = ((char)('A' + i)).ToString()
        });
      }
      ExpressionViews.Add(new Debugger.ExpressionView() {
        Expr = "theta",
      });
    }

    private void OnDisable() {
      All.Remove(this);
    }

    protected override void DrawAddon() {
      GUILayout.Label("Arg Stack: (" + Machine.State.ArgStackTop + " items)");
      for (int i = 0; i < Machine.State.ArgStackTop; i++) {
        var item = Machine.State.ArgStack[i];
        using (new GUILayout.HorizontalScope()) {
          GUILayout.TextField($"{item:X4}");
          GUILayout.TextField(item.ToString());
          GUILayout.TextField(((short)item).ToString());
        }
      }

      GUILayout.Space(10);

      int letterOffset = 0;

      using (new GUILayout.HorizontalScope()) {
        GUILayout.Label("Letter Offset:");
        if (LetterOffsetView.OnGUI()) {
          foreach (var expr in ExpressionViews) {
            expr.UpToDate = false;
          }
        }

        LetterOffsetView.Parse(0, Machine);
        LetterOffsetView.Execute(Machine);
        letterOffset = LetterOffsetView.CurrValue;
      }

      foreach (var expr in ExpressionViews) {
        using (new GUILayout.HorizontalScope()) {
          expr.OnGUI();
          expr.Parse(letterOffset, Machine);
          expr.Execute(Machine);

          string hexResult = "-";
          string unsignedResult = "-";
          string signedResult = "-";

          switch (expr.Result) {
            case Debugger.ExpressionResult.Success:
              hexResult = $"0x{expr.CurrValue:X4}";
              unsignedResult = $"{expr.CurrValue}";
              signedResult = $"{(short)expr.CurrValue}";
              break;
            case Debugger.ExpressionResult.Warn:
              GUI.color = Color.yellow;
              break;
            case Debugger.ExpressionResult.Error:
              GUI.color = Color.red;
              break;
          }

          EditorGUILayout.TextField(hexResult, GUILayout.Width(60));
          EditorGUILayout.TextField(unsignedResult, GUILayout.Width(60));
          EditorGUILayout.TextField(signedResult, GUILayout.Width(60));

          GUI.color = expr.IsWatched ? Color.green : Color.white;
          if (GUILayout.Button("Watch", GUILayout.Width(48))) {
            expr.IsWatched = !expr.IsWatched;
          }
          GUI.color = Color.white;

          GUI.color = Color.white;
        }
      }

      GUILayout.Space(10);

      using (new GUILayout.HorizontalScope()) {
        CustomAction = EditorGUILayout.TextField(CustomAction);

        if (GUILayout.Button("Execute")) {
          var tokens = Token.ParseLine(CustomAction);
          var lines = Parser.ParseLines(tokens);
          var program = Compiler.Compile(lines);
          program.Lines.RemoveAt(program.Lines.Count - 1);
          Machine.State.HL = 0;
          foreach (var line in program.Lines) {
            foreach (var item in line.Ops) {
              item.Op.Execute(ref Machine.State);
            }
          }
        }
      }
    }
  }
}
