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
      byte[] fileData = Recordings.SelectMany(r => r.Frames).Select(f => (byte)f).ToArray();

      writer.Write((ushort)fileData.Length);
      writer.Write(fileData);

      return stream.ToArray();
    }
  }



}
