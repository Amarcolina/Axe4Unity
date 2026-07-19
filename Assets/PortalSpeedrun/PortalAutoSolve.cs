using UnityEngine;
using System;
using System.Collections.Generic;
using Axe4Unity;

public class PortalAutoSolve : MonoBehaviour {

  public AxeRunner Runner;
  public MachineStateAsset StartState;
  public AxeSpeedRunRecording Recording;
  public int MoveFrames;
  public float ExplorationWeight;
  public float RightPressureWeight;
  public float BucketTermWeight;

  [Header("Game Constants")]
  public int LineOnFrame;
  public int LineOnWin;

  [Header("Runtime")]
  public bool DoInit;
  public bool DoStep;
  public bool FoundTerminalState;
  public int StepCount;
  public int TotalNodes;

  [SerializeField]
  private Node RootNode;

  private EdgeDef[] EdgeDefs;
  private MachineState TmpState;

  private int _playerXAddr;
  private int _playerYAddr;

  private bool[,] _visited;

  private Dictionary<BucketState, int> _bucketCounts;

  private void Update() {
    if (DoInit) {
      BeginMCTS();
      DoInit = false;
    }

    if (DoStep) {
      DoStep = false;
      for (int i = 0; i < StepCount; i++) {
        if (FoundTerminalState) {
          break;
        }
        StepMCTS();
      }
    }
  }

  public void BeginMCTS() {
    TotalNodes = 0;

    EdgeDefs = new EdgeDef[] {
      new EdgeDef(){ Move = Move.None, Frames = MoveFrames },

      new EdgeDef(){ Move = Move.Left, Frames = MoveFrames },
      new EdgeDef(){ Move = Move.Right, Frames = MoveFrames },
      //new EdgeDef(){ Move = Move.Jump, Frames = 1 },

      new EdgeDef(){ Move = Move.Portal0, Frames = 1 },
      new EdgeDef(){ Move = Move.Portal1, Frames = 1 },
      new EdgeDef(){ Move = Move.Portal2, Frames = 1 },
      new EdgeDef(){ Move = Move.Portal3, Frames = 1 },
      new EdgeDef(){ Move = Move.Portal4, Frames = 1 },
      new EdgeDef(){ Move = Move.Portal5, Frames = 1 },
      new EdgeDef(){ Move = Move.Portal6, Frames = 1 },
      new EdgeDef(){ Move = Move.Portal7, Frames = 1 },
    };

    _bucketCounts = new();

    RootNode = new Node();

    _playerXAddr = Runner.Machine.AddressOfName("X");
    _playerYAddr = Runner.Machine.AddressOfName("Y");

    StartState.State.CopyTo(Runner.Machine.State);

    byte playerX = (byte)(Runner.Machine.State.Read_U16(_playerXAddr) / 256);
    byte playerY = (byte)(Runner.Machine.State.Read_U16(_playerYAddr) / 256);

    var bucketState = new BucketState() {
      PlayerX = playerX,
      PlayerY = playerY
    };

    _bucketCounts[bucketState] = 1_000_000;
  }

  public void StepMCTS() {
    Recording.Frames.Clear();

    StartState.State.CopyTo(Runner.Machine.State);
    ExpandTree(RootNode);
  }

  public float GetNodeScore(Node node) {
    float explorationTerm = ExplorationWeight * Mathf.Sqrt(Mathf.Log(node.Parent.DescendantCount) / node.DescendantCount);
    float rightPressureTerm = RightPressureWeight * node.PlayerXScore / (18f * 256f);
    float bucketTerm = BucketTermWeight / node.VisitedCount;

    return explorationTerm + rightPressureTerm + bucketTerm;
  }

  public ExpandResults ExpandTree(Node node) {
    ExpandResults results;

    if (node.Children == null || node.Children.Length == 0) {
      node.Children = new Node[EdgeDefs.Length];
      for (int i = 0; i < node.Children.Length; i++) {
        node.Children[i] = new Node() {
          Parent = node,
          EdgeMove = EdgeDefs[i].Move,
          EdgeFrames = EdgeDefs[i].Frames,
          DescendantCount = 1
        };
      }

      results = new();
      results.NodesAdded = EdgeDefs.Length;
      TotalNodes += EdgeDefs.Length;

      TmpState.CopyFrom(Runner.Machine.State);
      foreach (var child in node.Children) {
        TmpState.CopyTo(Runner.Machine.State);

        byte playerXBefore = (byte)(Runner.Machine.State.Read_U16(_playerXAddr) / 256);
        byte playerYBefore = (byte)(Runner.Machine.State.Read_U16(_playerYAddr) / 256);

        SimulateEdge(child);

        child.PlayerXScore = Runner.Machine.State.Read_U16(_playerXAddr);

        byte playerXAfter = (byte)(Runner.Machine.State.Read_U16(_playerXAddr) / 256);
        byte playerYAfter = (byte)(Runner.Machine.State.Read_U16(_playerYAddr) / 256);

        var bucketState = new BucketState() {
          PlayerX = playerXAfter,
          PlayerY = playerYAfter
        };
        if (!_bucketCounts.TryGetValue(bucketState, out var count)) {
          count = 1;
        }

        if (playerXBefore != playerXAfter ||
            playerYBefore != playerYAfter) {
          count++;
        }

        _bucketCounts[bucketState] = count;

        child.VisitedCount = count;

        if (Runner.Machine.State.PC.LineIndex == LineOnWin) {
          Debug.Log("Found winning state");
          FoundTerminalState = true;
          child.IsWin = true;
        } else if (Runner.Machine.State.PC.LineIndex != LineOnFrame) {
          Debug.Log("Found losing state");
          FoundTerminalState = true;
          child.IsLoss = true;
        }
      }
    } else {
      float bestScore = 0;
      Node bestNode = null;
      foreach (var child in node.Children) {
        float score = GetNodeScore(child);
        if (score > bestScore) {
          bestScore = score;
          bestNode = child;
        }
      }

      for (int i = 0; i < bestNode.EdgeFrames; i++) {
        int key = 0;
        switch (bestNode.EdgeMove) {
          case Move.Left:
            key = 2;
            break;
          case Move.Right:
            key = 3;
            break;

          case Move.Portal0:
            key = 34;
            break;
          case Move.Portal1:
            key = 26;
            break;
          case Move.Portal2:
            key = 18;
            break;
          case Move.Portal3:
            key = 19;
            break;
          case Move.Portal4:
            key = 20;
            break;
          case Move.Portal5:
            key = 28;
            break;
          case Move.Portal6:
            key = 36;
            break;
          case Move.Portal7:
            key = 35;
            break;
        }
        Recording.Frames.Add(key);
      }

      SimulateEdge(bestNode);
      results = ExpandTree(bestNode);
    }

    node.DescendantCount += results.NodesAdded;

    node.PlayerXScore = 0;
    node.VisitedCount = int.MaxValue;
    foreach (var child in node.Children) {
      node.PlayerXScore = Mathf.Max(node.PlayerXScore, child.PlayerXScore);
      node.VisitedCount = Mathf.Min(node.VisitedCount, child.VisitedCount);
    }

    return results;
  }

  public void SimulateEdge(Node node) {
    for (int i = 0; i < Runner.Machine.State.PressedKeys.Length; i++) {
      Runner.Machine.State.PressedKeys[i] = false;
    }
    switch (node.EdgeMove) {
      case Move.Left:
        Runner.Machine.State.PressedKeys[2] = true;
        break;
      case Move.Right:
        Runner.Machine.State.PressedKeys[3] = true;
        break;
      case Move.Jump:
        Runner.Machine.State.PressedKeys[4] = true;
        break;

      case Move.Portal0:
        Runner.Machine.State.PressedKeys[34] = true;
        break;
      case Move.Portal1:
        Runner.Machine.State.PressedKeys[26] = true;
        break;
      case Move.Portal2:
        Runner.Machine.State.PressedKeys[18] = true;
        break;
      case Move.Portal3:
        Runner.Machine.State.PressedKeys[19] = true;
        break;
      case Move.Portal4:
        Runner.Machine.State.PressedKeys[20] = true;
        break;
      case Move.Portal5:
        Runner.Machine.State.PressedKeys[28] = true;
        break;
      case Move.Portal6:
        Runner.Machine.State.PressedKeys[36] = true;
        break;
      case Move.Portal7:
        Runner.Machine.State.PressedKeys[35] = true;
        break;
    }

    for (int i = 0; i < node.EdgeFrames; i++) {
      Runner.SimulateFrame();
    }
  }

  [System.Serializable]
  public class Node {
    //Edge
    public Move EdgeMove;
    public int EdgeFrames;

    [System.NonSerialized]
    public Node Parent;
    [System.NonSerialized]
    //[SerializeReference]
    public Node[] Children = null;
    public int DescendantCount;

    //Scoring
    public int PlayerXScore;
    public int VisitedCount;

    public bool IsWin;
    public bool IsLoss;
    public bool IsTerminal => IsWin || IsLoss;
  }

  public enum Move {
    None,

    Left,
    Right,
    Jump,

    Portal0,
    Portal1,
    Portal2,
    Portal3,
    Portal4,
    Portal5,
    Portal6,
    Portal7
  }

  public struct EdgeDef {
    public Move Move;
    public int Frames;
  }

  public struct ExpandResults {
    public int NodesAdded;
  }

  public struct BucketState : IEquatable<BucketState> {
    public byte PlayerX;
    public byte PlayerY;

    public bool Equals(BucketState other) {
      return PlayerX == other.PlayerX &&
             PlayerY == other.PlayerY;
    }

    public override bool Equals(object obj) {
      if (obj is BucketState other) {
        return Equals(other);
      } else {
        return false;
      }
    }

    public override int GetHashCode() {
      return PlayerX.GetHashCode() ^ PlayerY.GetHashCode();
    }
  }

}
