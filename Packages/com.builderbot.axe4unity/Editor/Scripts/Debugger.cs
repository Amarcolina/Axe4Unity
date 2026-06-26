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

    public const int CONTROL_BAR_HEIGHT = 20;
    public const int DEBUGGER_BAR_WIDTH = SCREEN_WIDTH * 4;

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

    public ExpressionView LetterOffsetView;
    public List<ExpressionView> ExpressionViews = new();
    public string CustomAction;

    public Vector2 ProgramScroll;

    private Dictionary<List<Token>, LineContent> _styledLineCache = new();

    [NonSerialized]
    private AxeRunner _subscribedRunner;

    private PauseOn PauseCondition;
    private GreyPreview _greyPreview;

    private Texture2D _screenPreview;
    private Texture2D _frontBuffer;
    private Texture2D _backBuffer;

    private SerializedObject _sObj;

    private int _stepLevel;
    private bool _anyWatching;

    [MenuItem("Axe/Debugger")]
    private static void Init() {
      GetWindow<Debugger>().Show();
    }

    private void OnEnable() {
      Texture2D CreateBuffer() {
        var buffer = new Texture2D(SCREEN_WIDTH, SCREEN_HEIGHT, TextureFormat.RGBA32, mipChain: false);
        buffer.filterMode = FilterMode.Point;
        return buffer;
      }

      _screenPreview = CreateBuffer();
      _frontBuffer = CreateBuffer();
      _backBuffer = CreateBuffer();

      _sObj = new SerializedObject(this);

      LetterOffsetView = new();

      ExpressionViews.Clear();
      ExpressionViews.Add(new ExpressionView() {
        Expr = "HL"
      });
      for (int i = 0; i < 26; i++) {
        ExpressionViews.Add(new ExpressionView() {
          Expr = ((char)('A' + i)).ToString()
        });
      }
      ExpressionViews.Add(new ExpressionView() {
        Expr = "theta",
      });

      DebuggerSkin = AssetDatabase.LoadAssetAtPath<GUISkin>("Packages/com.builderbot.axe4unity/Editor/GUI/DebuggerSkin.guiskin");
    }

    private void OnDisable() {
      DestroyImmediate(_screenPreview);
      DestroyImmediate(_frontBuffer);
      DestroyImmediate(_backBuffer);

      if (_subscribedRunner != null) {
        _subscribedRunner.OnStepExecution -= OnStepExecution;
      }
    }

    private void OnGUI() {
      Profiler.BeginSample("Axe.Debugger");
      Runner = FindAnyObjectByType<AxeRunner>(FindObjectsInactive.Exclude);

      _anyWatching = ExpressionViews.Any(e => e.IsWatched);

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
      Rect programViewRect = new Rect(0, CONTROL_BAR_HEIGHT, position.width - DEBUGGER_BAR_WIDTH * 2, position.height - CONTROL_BAR_HEIGHT);
      Rect debuggerRect = new Rect(position.width - DEBUGGER_BAR_WIDTH * 2, CONTROL_BAR_HEIGHT, DEBUGGER_BAR_WIDTH, position.height - CONTROL_BAR_HEIGHT);
      Rect screenViewRect = new Rect(position.width - DEBUGGER_BAR_WIDTH, CONTROL_BAR_HEIGHT, DEBUGGER_BAR_WIDTH, position.height - CONTROL_BAR_HEIGHT);

      Profiler.BeginSample("DoControlBar");
      DoControlBar(controlRect);
      Profiler.EndSample();

      Profiler.BeginSample("DoScreenView");
      DoScreenView(screenViewRect);
      Profiler.EndSample();

      Profiler.BeginSample("DoProgramView");
      DoProgramView(programViewRect);
      Profiler.EndSample();

      Profiler.BeginSample("DoDebuggerView");
      DoDebuggerView(debuggerRect);
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
        var prevHL = Machine.State.HL;
        foreach (var expr in ExpressionViews) {
          if (!expr.IsWatched) {
            continue;
          }

          Machine.State.HL = 0;
          try {
            foreach (var item in expr.Ops) {
              item.Op.Execute(ref Machine.State);
            }

            ushort newValue = Machine.State.HL;
            if (newValue != expr.CurrValue) {
              shouldPause = true;
              Debug.Log($"Expression {expr.Expr} changed from {expr.CurrValue} to {newValue} on op {executed.Op.GetType().Name} on line {executed.LineIndex}");
            }

            expr.CurrValue = newValue;
          } catch (Exception e) {
            Debug.LogException(e);
          }
        }
        Machine.State.HL = prevHL;
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

    private void DoScreenView(Rect rect) {
      if (Machine == null || Machine.Program == null) {
        return;
      }

      if (Event.current.type == EventType.Repaint) {
        new BufferToTextureJob() {
          Buffer = Machine.State.Memory.Slice(Machine.ADDR_L6, SCREEN_BYTES),
          Pixels = _frontBuffer.GetPixelData<Color32>(0)
        }.Run();

        new BufferToTextureJob() {
          Buffer = Machine.State.Memory.Slice(Machine.ADDR_L3, SCREEN_BYTES),
          Pixels = _backBuffer.GetPixelData<Color32>(0)
        }.Run();

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

      using (new GUILayout.AreaScope(rect)) {
        void DrawTexLayout(Texture2D tex) {
          var r = GUILayoutUtility.GetRect(tex.width * 4, tex.height * 4);
          GUI.DrawTexture(r, tex);
        }

        GUILayout.Label("Front Buffer");
        DrawTexLayout(_frontBuffer);

        GUILayout.Label("Back Buffer");
        DrawTexLayout(_backBuffer);

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
    }

    private void DoDebuggerView(Rect rect) {
      if (Machine == null || Machine.Program == null) {
        return;
      }

      using (new GUILayout.AreaScope(rect)) {
        if (Machine.NextOp != null) {
          EditorGUILayout.LabelField("Next Op: " + Machine.NextOp.Type);
        }

        GUILayout.FlexibleSpace();

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

        ushort prevHL = Machine.State.HL;
        try {
          int letterOffset = 0;

          using (new GUILayout.HorizontalScope()) {
            GUILayout.Label("Letter Offset:");
            EditorGUI.BeginChangeCheck();
            LetterOffsetView.Expr = GUILayout.TextField(LetterOffsetView.Expr);
            if (EditorGUI.EndChangeCheck()) {
              LetterOffsetView.UpToDate = false;
              foreach (var expr in ExpressionViews) {
                expr.UpToDate = false;
              }
            }

            UpdateExpressionView(LetterOffsetView, 0);

            Machine.State.HL = 0;
            if (LetterOffsetView.Ops != null) {
              try {
                foreach (var item in LetterOffsetView.Ops) {
                  item.Op.Execute(ref Machine.State);
                }
                letterOffset = Machine.State.HL;
              } catch { }
            }
          }

          foreach (var expr in ExpressionViews) {
            using (new GUILayout.HorizontalScope()) {
              EditorGUI.BeginChangeCheck();
              expr.Expr = GUILayout.TextField(expr.Expr);
              if (EditorGUI.EndChangeCheck()) {
                expr.UpToDate = false;
              }

              UpdateExpressionView(expr, letterOffset);

              string hexResult = "-";
              string unsignedResult = "-";
              string signedResult = "-";

              if (expr.Ops != null) {
                Machine.State.HL = (ushort)(expr.Expr == "HL" ? prevHL : 0);
                try {
                  foreach (var item in expr.Ops) {
                    item.Op.Execute(ref Machine.State);
                  }

                  hexResult = $"0x{Machine.State.HL:X4}";
                  unsignedResult = $"{Machine.State.HL}";
                  signedResult = $"{(short)Machine.State.HL}";

                  expr.CurrValue = Machine.State.HL;
                } catch (Exception e) {
                  Debug.LogException(e);
                  GUI.color = Color.red;
                }
              } else {
                GUI.color = Color.yellow;
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
        } finally {
          Machine.State.HL = prevHL;
        }
      }
    }

    private void UpdateExpressionView(ExpressionView expr, int letterOffset) {
      if (!expr.UpToDate) {
        try {
          if (expr.Expr == "HL") {
            expr.Ops = new List<OpAndMetaData>();
          } else {
            var program = Compiler.Compile(new List<List<Token>>() {
              Token.ParseLine($"real({letterOffset})"),
              Token.ParseLine(expr.Expr)
            });

            expr.Ops = program.Lines.SelectMany(l => l.Ops).Where(o => o.Op is not Op.Return).ToList();
          }

          expr.UpToDate = true;
        } catch (Exception) {
          expr.UpToDate = false;
          expr.Ops = null;
        }
      }
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

      [NonSerialized]
      public bool UpToDate = false;
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

    private enum GreyPreview {
      None,
      Grey3,
      Grey4
    }
  }
}
