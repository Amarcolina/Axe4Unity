using UnityEngine;
using System.Collections.Generic;
using Axe4Unity;

public class PortalSpeedRunManager : MonoBehaviour {

  public AxeRunner Runner;
  public CalcKeyboard Keyboard;
  public AxeSpeedRunRecording[] Recordings;
  public MachineStateAsset[] States;
  public float LevelFPS;
  public float TransitionFPS;
  public float MaintenanceFPS;
  public int MaintenanceStart, MaintenanceEnd;
  public float PauseTimeScale;
  public float PlaybackSpeed = 1;
  public List<int> WinLines = new();
  public List<int> LevelStartLines = new();

  [Header("Runtime")]
  public int CurrRecording;
  public int CurrFrame;
  public bool WaitingForWin;
  public bool WaitingForLevelStart;

  private float _stepTime;
  private int _prevKey;

  private void Start() {
    foreach (var b in Keyboard.Buttons) {
      if (Runner.Controls.Map.ContainsKey(b.Code)) {
        b.Button.image.color = b.Button.colors.pressedColor;
        b.Button.image.enabled = false;
        b.Button.enabled = false;
      }
    }
  }

  private void Update() {
    _stepTime -= Time.deltaTime * PlaybackSpeed;
    while (_stepTime < 0) {
      StepPlayback();
    }
  }

  private void StepPlayback() {
    if (WaitingForLevelStart) {
      if (CurrRecording >= MaintenanceStart && CurrRecording <= MaintenanceEnd) {
        _stepTime += 1f / MaintenanceFPS;
      } else {
        _stepTime += 1f / TransitionFPS;
      }

      SetKey(0);
      SetKey(15);

      int lineIndex = StepFrame();
      if (LevelStartLines.Contains(lineIndex)) {
        WaitingForLevelStart = false;
      }
    } else {
      _stepTime += 1f / LevelFPS;

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

      SetKey(pressedKey);

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
      _stepTime += results.PauseTime * PauseTimeScale;
    }

    return Runner.Machine.State.PC.LineIndex;
  }

  private void SetKey(int code) {
    if (Keyboard.CodeToButton.TryGetValue(_prevKey, out var prevButton)) {
      prevButton.Button.image.enabled = false;
    }

    for (int i = 0; i < Runner.Machine.State.PressedKeys.Length; i++) {
      Runner.Machine.SetKeyIsPressed(i, i == code);
    }

    if (Keyboard.CodeToButton.TryGetValue(code, out var button)) {
      button.Button.image.enabled = true;
    }

    _prevKey = code;
  }

}
