using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Axe4Unity {

  [CreateAssetMenu]
  [PreferBinarySerialization]
  public class MachineStateAsset : ScriptableObject {

    public MachineState State;

#if UNITY_EDITOR
    [CustomEditor(typeof(MachineStateAsset))]
    public class MachineStateAssetEditor : Editor {

      public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        bool TryGetActiveRunner(out AxeRunner runner) {
          runner = FindObjectsByType<AxeRunner>(FindObjectsInactive.Exclude).FirstOrDefault();
          if (runner == null) {
            Debug.LogWarning("No active runner in the scene!");
            return false;
          } else {
            return true;
          }
        }

        AxeRunner runner;
        if (GUILayout.Button("Save State") && TryGetActiveRunner(out runner)) {
          Undo.RecordObject(runner, "Save Axe State");
          (target as MachineStateAsset).State.CopyFrom(runner.Machine.State);
          EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("Load State") && TryGetActiveRunner(out runner)) {
          (target as MachineStateAsset).State.CopyTo(runner.Machine.State);
        }
      }
    }
#endif
  }
}
