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

    byte[] fileData = Recordings.SelectMany(r => r.Frames).Select(f => (byte)f).ToArray();
    int dataHeaderSize = 17;
    int dataLengthSize = 2;

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
        writer.Write((byte)0x00);
      }

      writer.Write((ushort)(fileData.Length + dataHeaderSize + dataLengthSize));

      //Data section
      {
        writer.Write((ushort)0x11);
        writer.Write((ushort)(fileData.Length + dataLengthSize));
        writer.Write((byte)15);

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

        writer.Write((ushort)(fileData.Length + dataLengthSize));
        writer.Write((ushort)(fileData.Length));

        writer.Write(fileData);
      }
    }
  }



}
