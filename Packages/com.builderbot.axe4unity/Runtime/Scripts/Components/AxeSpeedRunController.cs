using UnityEngine;
using UnityEngine.InputSystem;

namespace Axe4Unity {



  public class AxeSpeedRunController : MonoBehaviour {

    public AxeRunner Runner;
    public AxeKeyboardControl Controls;
    public AxeSpeedRunRecording Recording;
    public MachineStateAsset StartingState;
    public bool Record;
    public float PlaybackFPS = 60;
    public bool Restart;

    private int _currFrame;
    private float _frameResidual;

    private void OnEnable() {
      Runner.Running = false;
      if (Record) {
        Recording.Frames.Clear();
      }
      _currFrame = 0;
    }

    private void Update() {
      if (Record) {
        if (Restart) {
          Restart = false;
          Recording.Frames.Clear();
          StartingState.State.CopyTo(Runner.Machine.State);
        }

        if (Keyboard.current.anyKey.wasPressedThisFrame) {
          Runner.UpdateKeyControls(out var key);
          Recording.Frames.Add(key);
          UnityEditor.EditorUtility.SetDirty(Recording);
          SimulateFrame();
        }
      } else {
        if (Restart) {
          Restart = false;
          StartingState.State.CopyTo(Runner.Machine.State);
          _currFrame = 0;
          _frameResidual = 0;
        }

        _frameResidual -= Time.deltaTime;
        while (_frameResidual < 0 && _currFrame < Recording.Frames.Count) {
          _frameResidual += 1f / PlaybackFPS;
          for (int i = 0; i < Runner.Machine.State.PressedKeys.Length; i++) {
            Runner.Machine.State.PressedKeys[i] = false;
          }
          Runner.Machine.State.PressedKeys[Recording.Frames[_currFrame]] = true;
          SimulateFrame();

          _currFrame++;
        }
      }
    }

    private void SimulateFrame() {
      var results = Runner.SimulateFrame();

      if (results.DidDisplay) {
        Runner.Screen.UpdateScreen(Runner, results.DisplayMode);
      }
    }
  }
}
