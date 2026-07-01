using UnityEngine;
using UnityEditor;

namespace Axe4Unity {

  public class DebuggerAddonOpViewer : DebuggerAddon {

    private SerializedObject _programObj;
    private SerializedProperty _cachedProp;
    private OpAndMetaData _cachedOp;
    private GUIContent _nextOpContent;

    [MenuItem("Axe/Op Viewer", priority = 104)]
    private static void Init() {
      GetWindow<DebuggerAddonOpViewer>().Show();
    }

    private void OnEnable() {
      _nextOpContent = new GUIContent("Next Operation:");

      name = "Op Viewer";
      titleContent = new GUIContent("Op Viewer");
    }

    protected override void DrawAddon() {
      if (_programObj == null || _programObj.targetObject != Runner.Program) {
        if (_programObj != null) {
          _programObj.Dispose();
        }
        _programObj = new SerializedObject(Runner.Program);
      }

      GUILayout.Label($"Call Stack: ({Machine.State.CallStackTop})");
      for (int i = 0; i < Machine.State.CallStackTop; i++) {
        var pc = Machine.State.CallStack[i];
        GUILayout.Label($"   Line: {pc.LineIndex}  Op: {pc.OpIndex}");
      }

      GUILayout.Space(20);

      if (_cachedProp == null || _cachedOp != Machine.NextOp) {
        if (Machine.NextOp == null) {
          _cachedOp = null;
          _cachedProp = null;
        } else {
          var programProp = _programObj.FindProperty("Program");
          var lines = programProp.FindPropertyRelative("Lines");
          var line = lines.GetArrayElementAtIndex(Machine.State.PC.LineIndex);
          var ops = line.FindPropertyRelative("Ops");
          _cachedProp = ops.GetArrayElementAtIndex(Machine.State.PC.OpIndex);
          _cachedOp = Machine.NextOp;
          _cachedProp.isExpanded = true;
          _cachedProp.FindPropertyRelative("Op").isExpanded = true;
          _nextOpContent = new GUIContent($"Next Op: {_cachedOp.Op.GetType().Name}");
        }
      }

      if (_cachedProp == null) {
        EditorGUILayout.LabelField("Next Operation: None");
      } else {
        EditorGUILayout.PropertyField(_cachedProp, _nextOpContent, includeChildren: true);
      }
    }
  }
}
