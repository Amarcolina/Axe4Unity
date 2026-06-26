using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Axe4Unity {

  public class Parser {

    private static readonly string[] Extensions = new string[] {
      ".8xp",
      ".8Xp",
      ".8XP"
    };

    public static List<List<Token>> ParseFile(string filePath) {
      List<Token> result = new();

      using (var reader = File.OpenRead(filePath)) {
        //Header
        for (int i = 0; i < 55; i++) {
          reader.ReadByte();
        }

        //Metadata
        for (int i = 0; i < 19; i++) {
          reader.ReadByte();
        }

        while (true) {
          if (reader.Length - reader.Position == 2) {
            break;
          }

          int b0 = reader.ReadByte();
          if (b0 == -1) {
            break;
          }

          if (Token.Starts2Byte((byte)b0)) {
            int b1 = reader.ReadByte();
            result.Add(new Token((byte)b0, (byte)b1));
          } else {
            result.Add(new Token((byte)b0));
          }
        }
      }

      var lines = ParseLines(result);

      ExpandIncludes(Path.GetDirectoryName(filePath), lines);

      return lines;
    }

    public static List<List<Token>> ParseLines(List<Token> tokens) {
      List<List<Token>> lines = new();

      List<Token> currLine = new();
      bool isInStr = false;

      foreach (var token in tokens) {
        if (isInStr) {
          if (token == "\"" || token == "->" || token.IsEOL) {
            isInStr = false;
          }
        } else {
          if (token == "\"") {
            isInStr = true;
          }
        }

        if (token.IsEOL || (!isInStr && token == ":")) {
          if (currLine.Count != 0) {
            lines.Add(currLine);
          }
          currLine = new();
        } else {
          currLine.Add(token);
        }
      }
      if (currLine.Count != 0) {
        lines.Add(currLine);
      }

      return lines;
    }

    private static void ExpandIncludes(string dir, List<List<Token>> tokens) {
      for (int i = 0; i < tokens.Count; i++) {
        var line = tokens[i];
        if (line.Count != 0 && line[0] == "prgm") {
          var fileName = string.Join("", line.Skip(1).Select(t => t.ToString()));
          List<List<Token>> subProgram = null;
          foreach (var ext in Extensions) {
            var filePath = Path.Combine(dir, fileName + ext);
            if (File.Exists(filePath)) {
              subProgram = ParseFile(filePath);
              break;
            }
          }

          if (subProgram == null) {
            throw new FileNotFoundException($"Could not load sub-program {Token.ToString(line)} because the " +
              $"file could not be found in directory {dir}");
          }

          tokens.RemoveAt(i);
          tokens.InsertRange(i, subProgram);
        }
      }
    }
  }
}
