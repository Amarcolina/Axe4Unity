using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Axe4Unity {

  public class AxeSpeedRunOptimizer : MonoBehaviour {

    public AxeRunner Runner;
    public MachineStateAsset StartingState;
    public AxeSpeedRunRecording InputRecording;
    public AxeSpeedRunRecording OutputRecording;

    [Header("Simulation")]
    public int ExtraRightFrames;
    public int WinLine;
    public int WinLine2;
    public int WinX;
    public int ContinueLine;

    [Header("Annealing")]
    public float MaxFrameMS;
    public int SimulationCount;
    public int MaxFrameLoss;
    public int ResetLossThreshold;
    public float SchedulePower;
    public int MaxMutationCount;

    [Header("Runtime")]
    [Range(0, 100)]
    public float Progress;
    public int SimulationProgress;
    public int StartCost;
    public int CurrCost;
    public int BestCost;

    private List<int> _currRun = new();
    private List<int> _neighborRun = new();
    private List<int> _bestRun = new();

    private void OnEnable() {
      Runner.Running = false;

      _currRun = new();
      _currRun.AddRange(InputRecording.Frames);
      _bestRun = _currRun.ToList();

      SimulationProgress = 0;
      CurrCost = GetCost(_currRun);
      StartCost = CurrCost;
      BestCost = CurrCost;
    }

    private void Update() {
      float startTime = Time.realtimeSinceStartup;

      while (true) {
        float elapsedMs = 1000 * (Time.realtimeSinceStartup - startTime);
        if (elapsedMs > MaxFrameMS) {
          break;
        }

        float scheduleT = 1f - SimulationProgress / (float)SimulationCount;
        float scheduleTMapped = Mathf.Pow(scheduleT, SchedulePower);

        if ((CurrCost - BestCost) > ResetLossThreshold) {
          CurrCost = BestCost;
          _currRun.Clear();
          _currRun.AddRange(_bestRun);
        }

        GenerateNeighborRun();

        int neighborCost = GetCost(_neighborRun);

        int costDelta = neighborCost - CurrCost;
        bool isAccepted;
        if (costDelta < 0) {
          //Always accept better solutions
          isAccepted = true;
        } else {
          float costCeiling = scheduleTMapped * MaxFrameLoss;
          float costChance = Mathf.InverseLerp(costCeiling, 0, costDelta);
          isAccepted = Random.value < costChance;
        }

        if (isAccepted) {
          while (_neighborRun.Count > neighborCost) {
            _neighborRun.RemoveAt(_neighborRun.Count - 1);
          }

          _currRun.Clear();
          _currRun.AddRange(_neighborRun);
          CurrCost = neighborCost;
        }

        if (neighborCost < BestCost) {
          _bestRun.Clear();
          _bestRun.AddRange(_neighborRun);
          BestCost = neighborCost;
        }

        SimulationProgress++;
        Progress = 100f - Mathf.Round(10000f * scheduleT) / 100f;

        if (SimulationProgress >= SimulationCount) {
          if (BestCost == int.MaxValue) {
            throw new System.Exception();
          }

          OutputRecording.Frames.Clear();
          OutputRecording.Frames.AddRange(_bestRun);

          while (OutputRecording.Frames.Count < BestCost) {
            OutputRecording.Frames.Add(3);
          }
          while (OutputRecording.Frames.Count > BestCost) {
            OutputRecording.Frames.RemoveAt(OutputRecording.Frames.Count - 1);
          }

          UnityEditor.EditorUtility.SetDirty(OutputRecording);
          enabled = false;

          Debug.Log($"Output {OutputRecording.name} with {BestCost} frames");
        }
      }
    }

    public void GenerateNeighborRun() {
      _neighborRun.Clear();
      _neighborRun.AddRange(_currRun);

      for (int i = 0; i < Random.Range(1, MaxMutationCount + 1); i++) {
        switch (Random.Range(0, 5)) {
          case 0: {
            //nudge edge
            (int edge, int dir) = GetRandomEdge(_neighborRun);
            _neighborRun[edge] = _neighborRun[edge + dir];
            break;
          }
          case 1: {
            //insert edge frame
            (int edge, _) = GetRandomEdge(_neighborRun);
            _neighborRun.Insert(edge, _neighborRun[edge]);
            break;
          }
          case 2: {
            //delete edge frame
            (int edge, _) = GetRandomEdge(_neighborRun);
            _neighborRun.RemoveAt(edge);
            break;
          }
          case 3: {
            //insert new move
            int frame = Random.Range(0, _neighborRun.Count);
            _neighborRun.Insert(frame, Random.Range(2, 5));
            break;
          }
          case 4: {
            //Change move
            int frame = Random.Range(0, _neighborRun.Count);
            _neighborRun[frame] = Random.Range(2, 5);
            break;
          }
        }
      }
    }

    private (int edge, int dir) GetRandomEdge(List<int> moves) {
      float highest = -1;
      int chosenEdge = 0;

      for (int i = 1; i < moves.Count - 1; i++) {
        if (moves[i] != moves[i - 1] || moves[i] != moves[i + 1]) {
          var r = Random.value;
          if (r > highest) {
            chosenEdge = i;
            highest = r;
          }
        }
      }

      int dir = Random.value > 0.5f ? -1 : 1;
      if (moves[chosenEdge] == moves[chosenEdge + dir]) {
        dir = -1;
      }

      return (chosenEdge, dir);
    }

    public int GetCost(List<int> run) {
      StartingState.State.CopyTo(Runner.Machine.State);

      for (int i = 0; i < Runner.Machine.State.PressedKeys.Length; i++) {
        Runner.Machine.State.PressedKeys[i] = false;
      }

      int frames = 0;
      int playerXAddr = Runner.Machine.AddressOfName("X");

      foreach (var key in run) {
        for (int i = 0; i < Runner.Machine.State.PressedKeys.Length; i++) {
          Runner.Machine.SetKeyIsPressed(i, i == key);
        }
        Runner.SimulateFrame();

        int playerX = Runner.Machine.State.Read_U16(playerXAddr);
        if (playerX > 8000) {
          //Glitch detected
          return int.MaxValue;
        }

        if (playerX > WinX) {
          return frames;
        }

        frames++;

        if (Runner.Machine.State.PC.LineIndex == WinLine ||
            Runner.Machine.State.PC.LineIndex == WinLine2) {
          return frames;
        } else if (Runner.Machine.State.PC.LineIndex != ContinueLine) {
          //Debug.Log("Found weird line at: " + Runner.Machine.State.PC.LineIndex);
          return int.MaxValue;
        }
      }

      Runner.Machine.State.PressedKeys[3] = true;
      for (int i = 0; i < ExtraRightFrames; i++) {
        Runner.SimulateFrame();

        int playerX = Runner.Machine.State.Read_U16(playerXAddr);
        if (playerX > 8000) {
          //Glitch detected
          return int.MaxValue;
        }

        if (playerX > WinX) {
          return frames;
        }

        frames++;

        if (Runner.Machine.State.PC.LineIndex == WinLine ||
            Runner.Machine.State.PC.LineIndex == WinLine2) {
          return frames;
        } else if (Runner.Machine.State.PC.LineIndex != ContinueLine) {
          return int.MaxValue;
        }
      }

      return int.MaxValue;
    }
  }
}
