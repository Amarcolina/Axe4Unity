using UnityEngine;
using System.IO;
using System.Linq;
using Axe4Unity;

public class AxeSpeedRunExporter : MonoBehaviour {

  public string HeaderPath;
  public string OutputPath;
  public AxeSpeedRunRecording[] Recordings;

  [ContextMenu("Export")]
  public void Export() {
    var header = File.ReadAllBytes(HeaderPath).Take(74).ToArray();
    using (var writer = File.Create(OutputPath)) {
      writer.Write(header);
      foreach (var r in Recordings) {
        foreach (var frame in r.Frames) {
          writer.WriteByte((byte)frame);
        }
      }
    }
  }



}
