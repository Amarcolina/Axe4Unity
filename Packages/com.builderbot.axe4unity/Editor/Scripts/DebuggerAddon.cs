using UnityEditor;

namespace Axe4Unity {

  public abstract class DebuggerAddon : EditorWindow {

    public static AxeRunner Runner => Debugger.Instance.Runner;
    public static Machine Machine => Debugger.Instance.Machine;

    public void OnGUI() {
      if (Debugger.Instance == null) {
        EditorGUILayout.HelpBox("You must open a Debugger instance to use this window!", MessageType.Info);
        return;
      }

      if (Debugger.Instance.Machine == null) {
        EditorGUILayout.HelpBox("Waiting for machine to start...", MessageType.Info);
        return;
      }

      DrawAddon();

      Repaint();
    }

    protected abstract void DrawAddon();

  }
}
