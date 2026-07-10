using UnityEngine;
using System.Collections.Generic;
using Axe4Unity;

public class PortalSpeedRunManager : MonoBehaviour {

  public AxeRunner Runner;
  public AxeSpeedRunRecording[] Recordings;
  public MachineStateAsset[] States;
  public float PlaybackFPS;
  public float PlaybackSpeed = 1;
  public List<int> WinLines = new();
  public List<int> LevelStartLines = new();

  [Header("Runtime")]
  public int CurrRecording;
  public int CurrFrame;
  public bool WaitingForWin;
  public bool WaitingForLevelStart;

  private float _stepTime;

  private void Update() {
    _stepTime -= Time.deltaTime * PlaybackSpeed;
    while (_stepTime < 0) {
      _stepTime += 1f / PlaybackFPS;
      StepPlayback();
    }
  }

  private void StepPlayback() {
    if (WaitingForLevelStart) {
      for (int i = 0; i < Runner.Machine.State.PressedKeys.Length; i++) {
        Runner.Machine.SetKeyIsPressed(i, false);
      }
      Runner.Machine.SetKeyIsPressed(15, true);

      int lineIndex = StepFrame();
      if (LevelStartLines.Contains(lineIndex)) {
        WaitingForLevelStart = false;
      }
    } else {
      var rec = Recordings[CurrRecording];

      int pressedKey;
      if (WaitingForWin) {
        pressedKey = 3;
      } else {
        pressedKey = rec.Frames[CurrFrame];
        CurrFrame++;
        if (CurrFrame >= rec.Frames.Count) {
          WaitingForWin = true;
        }
      }

      for (int i = 0; i < Runner.Machine.State.PressedKeys.Length; i++) {
        Runner.Machine.SetKeyIsPressed(i, i == pressedKey);
      }

      int lineIndex = StepFrame();
      if (WinLines.Contains(lineIndex)) {
        WaitingForWin = false;
        WaitingForLevelStart = true;
        CurrFrame = 0; 
        CurrRecording++;
      }
    }
  }

  private int StepFrame() {
    var results = Runner.SimulateFrame();

    if (results.DidDisplay) {
      Runner.Screen.UpdateScreen(Runner, results.DisplayMode);
    }

    if (results.PauseTime > 0) {
      _stepTime += results.PauseTime;
    }

    return Runner.Machine.State.PC.LineIndex;
  }

}
