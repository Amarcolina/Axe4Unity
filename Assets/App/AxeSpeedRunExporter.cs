using UnityEngine;
using System.IO;
using System.Linq;
using Axe4Unity;
using System.Collections;
using System.Collections.Generic;

public class AxeSpeedRunExporter : MonoBehaviour {

  public string OutputPath;
  public MachineStateAsset[] States;
  public AxeSpeedRunRecording[] Recordings;
  public AxeSpeedRunOptimizer Optimizer;

  public AxeSpeedRunRecording A, B, AB;

  [ContextMenu("Ammend")]
  public void Ammend() {
    AB.Frames.AddRange(A.Frames);
    AB.Frames.AddRange(B.Frames);
  }

  [ContextMenu("Export")]
  public void Export() {
    var dataSection = BuildDataSection();
    var dataChecksum = CheckSum(dataSection);

    using (var writer = new BinaryWriter(File.Create(OutputPath))) {
      writer.Write((byte)'*');
      writer.Write((byte)'*');
      writer.Write((byte)'T');
      writer.Write((byte)'I');
      writer.Write((byte)'8');
      writer.Write((byte)'3');
      writer.Write((byte)'F');
      writer.Write((byte)'*');

      writer.Write((byte)0x1A);
      writer.Write((byte)0x0A);
      writer.Write((byte)0x00);

      for (int i = 0; i < 42; i++) {
        writer.Write((byte)' ');
      }

      writer.Write((ushort)dataSection.Length);

      writer.Write(dataSection);

      writer.Write(dataChecksum);
    }
  }

  [ContextMenu("Start trim recordings")]
  public void StartTrimRecordings() {
    StartCoroutine(TrimRecordings());
  }

  public IEnumerator TrimRecordings() {
    for (int i = 0; i < States.Length; i++) {
      Optimizer.StartingState = States[i];
      Optimizer.InputRecording = Recordings[i];
      Optimizer.OutputRecording = Recordings[i];
      Optimizer.enabled = true;
      Optimizer.SimulationCount = 1;

      yield return null;
      yield return null;
      yield return null;

      Optimizer.enabled = false;

      yield return null;
    }
  }

  private ushort CheckSum(byte[] data) {
    ushort sum = 0;
    foreach (var b in data) {
      sum += b;
    }
    return sum;
  }

  private byte[] BuildDataSection() {
    var fileData = BuildFileData();

    using (var stream = new MemoryStream())
    using (var writer = new BinaryWriter(stream)) {
      writer.Write((ushort)11);
      writer.Write((ushort)fileData.Length);
      writer.Write((byte)0x15);

      writer.Write((byte)'S');
      writer.Write((byte)'P');
      writer.Write((byte)'E');
      writer.Write((byte)'E');
      writer.Write((byte)'D');
      writer.Write((byte)'R');
      writer.Write((byte)'U');
      writer.Write((byte)'N');

      writer.Write((byte)0);
      writer.Write((byte)0);

      writer.Write((ushort)fileData.Length);

      writer.Write(fileData);

      return stream.ToArray();
    }
  }

  private byte[] BuildFileData() {
    using (var stream = new MemoryStream())
    using (var writer = new BinaryWriter(stream)) {
      List<byte> fileData = new();
      int offset = Recordings.Length * 2;
      foreach (var r in Recordings) {
        fileData.Add((byte)(offset & 0xFF));
        fileData.Add((byte)(offset >> 8));
        offset += r.Frames.Count;
        offset += 8;
      }

      foreach (var r in Recordings) {
        fileData.AddRange(r.Frames.Select(f => (byte)f));
        for (int i = 0; i < 8; i++) {
          fileData.Add(3);
        }
      }

      writer.Write((ushort)fileData.Count);
      writer.Write(fileData.ToArray());

      return stream.ToArray();
    }
  }



}
