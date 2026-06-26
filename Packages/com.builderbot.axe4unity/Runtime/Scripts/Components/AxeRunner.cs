using System;
using UnityEngine;

namespace Axe4Unity {

  public class AxeRunner : MonoBehaviour {

    public Action<OpAndMetaData> OnStepExecution;

    [Tooltip("The program to run, import an .8xp file into your Unity project to " +
             "generate a program asset.")]
    public ProgramAsset Program;

    [Tooltip("The keyboard control scheme to use that maps real-world keys to " +
             "calculator keycodes")]
    public AxeKeyboardControl Controls;

    [Tooltip("The screen component to send display data to as the program runs.")]
    public AxeScreen Screen;

    [Tooltip("A native runner to use instead of interpreted mode.  Generate a native " +
             "runner on the asset import options of your program.\n\nMUST match the " +
             "provided program to work.")]
    public NativeRunner NativeRunner;

    [Header("Assets")]
    [Tooltip("Used for drawing large text.")]
    public BitFont LargeFont;

    [Tooltip("Used for drawing small text.")]
    public BitFont SmallFont;

    [Tooltip("Which AppVars should be included on the system during execution.  Generate " +
             "an appvar asset by importing an .8xv file into your Unity project.")]
    public AppVarEntry[] AppVars;

    [Header("Execution")]
    [Tooltip("Whether or not to simulate the program, can be toggled at runtime to pause " +
             "or resume simulation.")]
    public bool Running = true;

    [Tooltip("Delay in seconds before beginning execution")]
    public float DelayStart = 0.5f;

    [Tooltip("The target FPS for frame-related operations like DispGraph.  Does not affect " +
             "timing-oriented operations like Pause.")]
    public float TargetFPS = 30;

    [Tooltip("The target simulation scale.  Turning this up will cause the entire simulation " +
             "to speed up uniformly, generating frames more often, and having Pause operations " +
             "take less time.")]
    public float SimulationScale = 1;

    [Tooltip("How many total milliseconds can be spent per Unity-frame on simulation.  If the " +
             "simulation exceeds this time, it will be stopped early.  If the simulation is " +
             "too expensive, this throttling will cause the simulation to not meet its target " +
             "FPS.")]
    public float MaxCPUPerAppFrame = 8;

    [Tooltip("The total number of operations that can be executed per frame before the simulation " +
             "is skipped for the current Unity frame.  Useful for catching infinite loops and preventing " +
             "your app from hanging.")]
    public int MaxOpsPerFrame = 10_000;

    [Tooltip("Once this many getKey operations are experienced, the simulation will consider that a " +
             "new frame.  This is to handle input-poll loops which don't update the screen.")]
    public int GetKeySkipCount = 100;

    public bool ClearScreenOnExit = true;

    [Header("Debugging")]
    [Tooltip("If non-zero, will force that key to be pressed every frame.")]
    public int LockedKey;

    private Machine _machine;
    public Machine Machine => _machine;

    private float _frameResidual = 0;
    private bool _waitForAnyKey;
    private float _delayStart;

    public void LoadFiles() {
      _machine.ResetAllFiles();

      foreach (var entry in AppVars) {
        if (entry.Archive) {
          _machine.AddToArchive("appv" + entry.AppVar.name, entry.AppVar.GetData());
        } else {
          bool didAdd = _machine.TryAddToRAM("appv" + entry.AppVar.name, entry.AppVar.GetData());
          if (!didAdd) {
            Debug.LogError($"Could not add appvar {entry.AppVar.name} because there was not enough free RAM!", entry.AppVar);
          }
        }
      }
    }

    private void OnEnable() {
      _machine = new(Program.Program, LargeFont, SmallFont);
      _delayStart = DelayStart;

      LoadFiles();

      if (Screen != null) {
        Screen.UpdateScreen(this, 0);
      }
    }

    private void OnDisable() {
      _machine.Dispose();
    }

    private void Update() {
      if (!Running) {
        return;
      }

      _delayStart -= Time.deltaTime;
      if (_delayStart > 0) {
        return;
      }

      int anykey = 0;
      foreach ((var code, var controls) in Controls.Map) {
        bool isPressed = false;
        foreach (var control in controls) {
          if (control.isPressed) {
            isPressed = true;
            anykey = code;
            break;
          }
        }

        if (Controls.CalcKeyboard != null &&
            Controls.CalcKeyboard.CodeToButton.TryGetValue(code, out var button) &&
            button.IsPressed) {
          isPressed = true;
        }

        _machine.SetKeyIsPressed(code, isPressed);
      }

      if (LockedKey != 0) {
        _machine.SetKeyIsPressed(LockedKey, true);
      }

      if (_waitForAnyKey) {
        if (anykey == 0) {
          return;
        }
        _waitForAnyKey = false;
        _machine.State.HL = (ushort)anykey;
      }

      bool didDisplay = false;
      int displayRMode = 0;

      float startTime = Time.realtimeSinceStartup;
      _frameResidual -= Time.deltaTime * SimulationScale;

      while (_frameResidual < 0 && Running) {
        float realElapsedTime = Time.realtimeSinceStartup - startTime;
        if (realElapsedTime * 1000 > MaxCPUPerAppFrame) {
          _frameResidual = 1f / TargetFPS;
          break;
        }

        int getKeyCount = 0;
        var stopSimulatingThisFrame = false;

        int opsLeft = MaxOpsPerFrame;
        while (Running) {
          OpAndMetaData executed;
          try {
            if (NativeRunner != null && NativeRunner.enabled) {
              executed = NativeRunner.Step(Machine, MaxOpsPerFrame);
            } else {
              executed = Machine.Step();
            }
            if (executed == null) {
              Running = false;
              stopSimulatingThisFrame = true;
              break;
            }
          } catch (Exception e) {
            Running = false;
            Debug.LogException(e);
            return;
          }

          if (OnStepExecution != null) {
            UnityEngine.Profiling.Profiler.BeginSample("OnStepCallback");
            OnStepExecution.Invoke(executed);
            UnityEngine.Profiling.Profiler.EndSample();
          }

          opsLeft--;
          if (opsLeft <= 0) {
            _frameResidual = 1f / TargetFPS;
            stopSimulatingThisFrame = true;
            Debug.LogWarning("Program exceeded max operations per frame");
            break;
          }

          if (executed.Op is Op.GetKey getKey && getKey.RMode == 1) {
            _frameResidual = 1f / TargetFPS;
            _waitForAnyKey = true;
            stopSimulatingThisFrame = true;
            break;
          }

          if (executed.Op is Op.DispGraph dispGraph) {
            _frameResidual += 1f / TargetFPS;
            didDisplay = true;
            displayRMode = dispGraph.RMode;
            break;
          }

          if (executed.Op is Op.ClrHome or Op.Disp or Op.Output ||
              (executed.Op is Op.Text && !_machine.State.TextToBuffer)) {
            _frameResidual += 1f / TargetFPS;
            didDisplay = true;
            displayRMode = 0;
          }

          if (executed.Op is Op.Pause) {
            if (_machine.State.IsFullSpeed) {
              _frameResidual += _machine.State.HL / 4500f;
            } else {
              _frameResidual += _machine.State.HL / 1800f;
            }
            break;
          }

          if (executed.Op is Op.GetKey) {
            getKeyCount++;
            if (getKeyCount > GetKeySkipCount) {
              _frameResidual = 1f / TargetFPS;
              stopSimulatingThisFrame = true;
              break;
            }
          }
        }

        if (stopSimulatingThisFrame) {
          break;
        }
      }

      if (!Running) {
        didDisplay = true;
        displayRMode = 0;

        var screenBuffer = Machine.State.GetBuffer(Machine.ADDR_SCREEN_FRONT, Constants.SCREEN_BYTES);
        for (int i = 0; i < screenBuffer.Length; i++) {
          screenBuffer[i] = 0;
        }
      }

      if (didDisplay && Screen != null) {
        Screen.UpdateScreen(this, displayRMode);
      }
    }

    [Serializable]
    public struct AppVarEntry {
      public DataAsset AppVar;
      public bool Archive;
    }
  }
}
