using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;

namespace Axe4Unity {

  [ScriptedImporter(2, new string[] { ".8xp", ".8Xp", ".8XP", ".axe" })]
  public class ProgramScriptedImporter : ScriptedImporter {

    [Tooltip("Should the program attempt import even if it is marked as a sub-program.")]
    public bool ImportSubProgram;

    [Tooltip("Should a native-runner script be generated for this program.\n\nCURRENTLY EXPERIMENTAL!")]
    public bool GenerateNativeRunner;

    public override void OnImportAsset(AssetImportContext ctx) {
      List<List<Token>> lines;

      if (ctx.assetPath.EndsWith(".axe")) {
        lines = new();
        foreach (var line in File.ReadAllLines(ctx.assetPath)) {
          lines.Add(Token.ParseLine(line));
        }
      } else {
        lines = Parser.ParseFile(ctx.assetPath);
      }

      if (lines[0].Count < 2) {
        return;
      }

      if (lines[0][0] != "." || (lines[0][1] == "." && !ImportSubProgram)) {
        return;
      }

      //File.WriteAllLines("Parsed.txt", lines.Select(l => Token.ToString(l)));

      var program = Compiler.Compile(lines, Path.GetDirectoryName(ctx.assetPath));

      var asset = ScriptableObject.CreateInstance<ProgramAsset>();
      asset.Program = program;

      string dir = Path.GetDirectoryName(ctx.assetPath);

      if (GenerateNativeRunner) {
        string fileName = Path.GetFileNameWithoutExtension(ctx.assetPath);
        string filePath = Path.Combine(dir, $"{fileName}_NativeRunner.cs");
        CodeGen.Generate(filePath, program);
        EditorApplication.delayCall += () => AssetDatabase.ImportAsset(filePath);
      }

      ctx.AddObjectToAsset("program", asset);
      ctx.SetMainObject(asset);
    }
  }
}
