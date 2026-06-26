using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AssetImporters;

namespace Axe4Unity {

  [ScriptedImporter(2, new string[] { ".8xv", ".8Xv",  })]
  public class AppVarScriptedImporter : ScriptedImporter {

    public override void OnImportAsset(AssetImportContext ctx) {
      var appVar = ScriptableObject.CreateInstance<AppVarAsset>();
      var bytes = new List<byte>();

      using (var reader = File.OpenRead(ctx.assetPath)) {
        //Header
        for (int i = 0; i < 74; i++) {
          reader.ReadByte();
        }

        while (true) {
          if (reader.Length - reader.Position == 2) {
            break;
          }

          bytes.Add((byte)reader.ReadByte());
        }
      }

      appVar.Data = bytes;

      ctx.AddObjectToAsset("data", appVar);
      ctx.SetMainObject(appVar);
    }
  }
}
