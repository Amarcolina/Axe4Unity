using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEditor;
using Unity.Jobs;
using Unity.Collections;

namespace Axe4Unity {
  using static Constants;

  public class Debugger : EditorWindow {

    public static Debugger Instance;

    public const int CONTROL_BAR_HEIGHT = 20;

    public const int PROGRAM_LINE_NUM_WIDTH = 46;
    public const int PROGRAM_LINE_HEIGHT = 24;

    public static readonly Color BreakpointColor = new Color(0.5f, 0.1f, 0.1f, 1f);
    public static readonly Color ParentHighlightColor = new Color(0.02f, 0.02f, 0.02f, 1f);
    public static readonly Color HighlightColor = new Color(0.4f, 0.4f, 0.5f, 1f);

    public GUISkin DebuggerSkin;

    public AxeRunner Runner;
    public Machine Machine;
    public Program Program;

    public List<int> Breakpoints = new();

    public Vector2 ProgramScroll;

    private Dictionary<List<Token>, LineContent> _styledLineCache = new();

    [NonSerialized]
    private AxeRunner _subscribedRunner;

    private PauseOn PauseCondition;

    private int _stepLevel;
    private bool _anyWatching;

    [MenuItem("Axe/Debugger")]
    private static void Init() {
      GetWindow<Debugger>().Show();
    }

    private void OnEnable() {
      Instance = this;
      DebuggerSkin = AssetDatabase.LoadAssetAtPath<GUISkin>("Packages/com.builderbot.axe4unity/Editor/GUI/DebuggerSkin.guiskin");
    }

    private void OnDisable() {
      Instance = null;

      if (_subscribedRunner != null) {
        _subscribedRunner.OnStepExecution -= OnStepExecution;
      }
    }

    private void OnGUI() {
      Profiler.BeginSample("Axe.Debugger");
      Runner = FindAnyObjectByType<AxeRunner>(FindObjectsInactive.Exclude);

      _anyWatching = DebuggerAddonVarViewer.All.
                     SelectMany(v => v.ExpressionViews).
                     Any(e => e.IsWatched);

      Machine = Runner.Machine;
      Program = Runner.Program.Program;

      if (Application.isPlaying) {
        if (Runner != _subscribedRunner) {
          if (_subscribedRunner != null) {
            _subscribedRunner.OnStepExecution -= OnStepExecution;
          }
          _subscribedRunner = Runner;
          if (_subscribedRunner != null) {
            _subscribedRunner.OnStepExecution += OnStepExecution;
          }
        }
      }

      Rect controlRect = new Rect(0, 0, position.width, CONTROL_BAR_HEIGHT);
      Rect programViewRect = new Rect(0, CONTROL_BAR_HEIGHT, position.width, position.height - CONTROL_BAR_HEIGHT);

      Profiler.BeginSample("DoControlBar");
      DoControlBar(controlRect);
      Profiler.EndSample();

      Profiler.BeginSample("DoProgramView");
      DoProgramView(programViewRect);
      Profiler.EndSample();

      Repaint();
      Profiler.EndSample();
    }

    private void OnStepExecution(OpAndMetaData executed) {
      if (Machine.NextOp == null) {
        return;
      }

      var nextOp = Machine.NextOp;
      var prevOp = executed;

      bool shouldPause = false;
      switch (PauseCondition) {
        case PauseOn.Op:
          shouldPause = true;
          break;
        case PauseOn.StepIn:
          shouldPause = nextOp.Row != prevOp.Row;
          break;
        case PauseOn.StepOut:
          shouldPause = Machine.State.CallStackTop < _stepLevel;
          break;
        case PauseOn.StepOver:
          shouldPause = Machine.State.CallStackTop <= _stepLevel &&
                        nextOp.Row != prevOp.Row;
          break;
        case PauseOn.Graphic:
          shouldPause = prevOp.Op is IOpGraphic;
          break;
        case PauseOn.Frame:
          shouldPause = prevOp.Op is Op.DispGraph;
          break;
      }

      if (Machine.NextOp != null &&
          Machine.NextOp.Row != prevOp.Row &&
          Breakpoints.Contains(Machine.NextOp.Row)) {
        shouldPause = true;
      }

      if (_anyWatching) {
        foreach (var varViewer in DebuggerAddonVarViewer.All) {
          foreach (var expr in varViewer.ExpressionViews) {
            if (!expr.IsWatched) {
              continue;
            }

            ushort prevValue = expr.CurrValue;
            expr.Execute(Machine);
            ushort newValue = expr.CurrValue;

            if (expr.Result == ExpressionResult.Success &&
                newValue != prevValue) {
              shouldPause = true;
              Debug.Log($"Expression {expr.Expr} changed from {prevValue} to {newValue} on op {executed.Op.GetType().Name} on line {executed.LineIndex}");
            }
          }
        }
      }

      if (shouldPause) {
        PauseCondition = PauseOn.None;
        Runner.Running = false;
        ProgramScroll.y = Mathf.Max(0, Machine.NextOp.Row * PROGRAM_LINE_HEIGHT - position.height / 2);
      }

      ProgramScroll.y = Mathf.Max(0, Machine.NextOp.Row * PROGRAM_LINE_HEIGHT - position.height / 2);
    }

    private void DoControlBar(Rect rect) {
      using (new GUILayout.AreaScope(rect)) {
        using (new GUILayout.HorizontalScope()) {
          GUILayout.Label("", EditorStyles.toolbar, GUILayout.ExpandWidth(true));

          if (GUILayout.Button("Restart", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
            Runner.Machine.Reset();
          }

          GUILayout.Space(50);

          if (GUILayout.Button(Runner.Running ? "Pause" : "Resume", EditorStyles.toolbarButton, GUILayout.Width(100))) {
            Runner.Running = !Runner.Running;
          }

          if (GUILayout.Button("Step In", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
            PauseCondition = PauseOn.StepIn;
            Runner.Running = true;
          }

          if (GUILayout.Button("Step Over", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
            PauseCondition = PauseOn.StepOver;
            Runner.Running = true;
            _stepLevel = Machine.State.CallStackTop;
          }

          if (GUILayout.Button("Step Out", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
            PauseCondition = PauseOn.StepOut;
            Runner.Running = true;
            _stepLevel = Machine.State.CallStackTop;
          }

          GUILayout.Space(50);

          if (GUILayout.Button("Step Op", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
            PauseCondition = PauseOn.Op;
            Runner.Running = true;
          }

          if (GUILayout.Button("Next Graphic", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
            PauseCondition = PauseOn.Graphic;
            Runner.Running = true;
          }

          if (GUILayout.Button("Next Frame", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
            PauseCondition = PauseOn.Frame;
            Runner.Running = true;
          }

          GUILayout.Label("", EditorStyles.toolbar, GUILayout.ExpandWidth(true));
        }
      }
    }

    private void DoProgramView(Rect rect) {
      if (Program == null) {
        return;
      }

      var viewRect = new Rect(0, 0, rect.width - 20, Program.Lines.Count * PROGRAM_LINE_HEIGHT);

      ProgramScroll = GUI.BeginScrollView(rect, ProgramScroll, viewRect, alwaysShowHorizontal: false, alwaysShowVertical: true);

      var prevSkin = GUI.skin;
      GUI.skin = DebuggerSkin;
      for (int i = 0; i < Program.Lines.Count; i++) {
        if (i * PROGRAM_LINE_HEIGHT < ProgramScroll.y ||
            i * PROGRAM_LINE_HEIGHT > ProgramScroll.y + position.height) {
          continue;
        }

        var numRect = new Rect(0, i * PROGRAM_LINE_HEIGHT, PROGRAM_LINE_NUM_WIDTH, PROGRAM_LINE_HEIGHT);
        var lineRect = new Rect(PROGRAM_LINE_NUM_WIDTH, i * PROGRAM_LINE_HEIGHT, rect.width - PROGRAM_LINE_NUM_WIDTH, PROGRAM_LINE_HEIGHT);

        Profiler.BeginSample("GetStyledLine");
        var lineContent = GetStyledLine(Program.Lines[i]);
        Profiler.EndSample();

        if (Machine != null) {
          for (int j = 0; j < Machine.State.CallStackTop; j++) {
            var pc = Machine.State.CallStack[j];
            var op = Program.Lines[pc.LineIndex].Ops[pc.OpIndex];

            if (i == op.Row) {
              var rMin = DebuggerSkin.label.GetCursorPixelPosition(lineRect, lineContent.GUIContent, lineContent.TokenToCharLookup[op.ColStart]);
              var rMax = DebuggerSkin.label.GetCursorPixelPosition(lineRect, lineContent.GUIContent, lineContent.TokenToCharLookup[op.ColEnd]);
              Rect highlightRect = new Rect(rMin.x, i * PROGRAM_LINE_HEIGHT, rMax.x - rMin.x, PROGRAM_LINE_HEIGHT);

              GUI.color = j == Machine.State.CallStackTop - 1 ? HighlightColor : ParentHighlightColor;
              GUI.DrawTexture(highlightRect, Texture2D.whiteTexture);
              GUI.color = Color.white;
            }
          }
        }

        if (Breakpoints.Contains(i)) {
          GUI.color = BreakpointColor;
          GUI.DrawTexture(numRect, Texture2D.whiteTexture);
          GUI.color = Color.white;
        }

        GUI.color = (Machine != null && Machine.NextOp != null && Machine.NextOp.Row == i) ?
                    Color.white :
                    new Color(0.5f, 0.5f, 0.5f, 1f);

        if (GUI.Button(numRect, $"{i:D4}:", DebuggerSkin.label)) {
          if (Breakpoints.Contains(i)) {
            Breakpoints.Remove(i);
          } else {
            Breakpoints.Add(i);
          }
        }
        GUI.color = Color.white;

        GUI.Label(lineRect, lineContent.GUIContent);
      }
      GUI.skin = prevSkin;

      GUI.EndScrollView();
    }

    private LineContent GetStyledLine(Program.Line line) {
      if (_styledLineCache.TryGetValue(line.Tokens, out var content)) {
        return content;
      }

      string text = "".PadLeft(line.Indent * 2);
      int[] tokenToChar = new int[line.Tokens.Count + 1];
      for (int i = 0; i < line.Tokens.Count; i++) {
        tokenToChar[i] = text.Length;
        text = text + line.Tokens[i].ToString();
      }

      tokenToChar[tokenToChar.Length - 1] = text.Length;

      content = new LineContent() {
        GUIContent = new GUIContent(text),
        TokenToCharLookup = tokenToChar
      };

      _styledLineCache[line.Tokens] = content;

      return content;
    }

    private struct LineContent {
      public GUIContent GUIContent;
      public int[] TokenToCharLookup;
    }

    [Serializable]
    public class ExpressionView {

      public string Expr;
      public List<OpAndMetaData> Ops;

      public bool IsWatched;
      public ushort CurrValue;
      public ExpressionResult Result;

      [NonSerialized]
      public bool UpToDate = false;

      public bool OnGUI(string label = null) {
        EditorGUI.BeginChangeCheck();
        if (label == null) {
          Expr = GUILayout.TextField(Expr);
        } else {
          Expr = EditorGUILayout.TextField(label, Expr);
        }
        if (EditorGUI.EndChangeCheck()) {
          UpToDate = false;
          return true;
        }
        return false;
      }

      public void Parse(int letterAddress, Machine machine) {
        if (!UpToDate) {
          try {
            if (Expr == "HL") {
              Ops = new List<OpAndMetaData>();
            } else {
              var program = Compiler.Compile(new List<List<Token>>() {
                Token.ParseLine($"real({letterAddress})"),
                Token.ParseLine(Expr)
              }, context: machine.Program);

              Ops = program.Lines.SelectMany(l => l.Ops).Where(o => o.Op is not Op.Return).ToList();
            }

            UpToDate = true;
          } catch (Exception) {
            UpToDate = false;
            Ops = null;
          }
        }
      }

      public void Execute(Machine machine) {
        ushort prevHL = machine.State.HL;
        machine.State.HL = 0;
        try {
          if (Ops != null) {
            machine.State.HL = (ushort)(Expr == "HL" ? prevHL : 0);
            try {
              foreach (var item in Ops) {
                item.Op.Execute(ref machine.State);
              }

              CurrValue = machine.State.HL;
              Result = ExpressionResult.Success;
            } catch (Exception e) {
              Debug.LogException(e);
              CurrValue = 0;
              Result = ExpressionResult.Error;
            }
          } else {
            CurrValue = 0;
            Result = ExpressionResult.Warn;
          }
        } finally {
          machine.State.HL = prevHL;
        }
      }
    }

    private enum PauseOn {
      None,
      Op,
      StepOver,
      StepIn,
      StepOut,
      Graphic,
      Frame
    }

    public enum ExpressionResult {
      Success,
      Warn,
      Error
    }
  }
}
