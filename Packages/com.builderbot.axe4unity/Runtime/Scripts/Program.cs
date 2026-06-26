using System;
using System.Collections.Generic;

namespace Axe4Unity {

  [Serializable]
  public class Program {

    public List<Line> Lines = new();

    public List<byte> Data = new();

    public List<Label> Labels = new();

    public List<StaticVariable> StaticVariables = new();

    public List<CustomVariable> CustomVariables = new();

    public bool TryGetVarAddress(string name, out int address) {
      foreach (var var in CustomVariables) {
        if (var.Name == name) {
          address = var.Address;
          return true;
        }
      }

      foreach (var var in StaticVariables) {
        if (var.Name == name) {
          address = var.Address;
          return true;
        }
      }

      address = default;
      return false;
    }

    public bool TryGetLabelLine(string name, out int line) {
      foreach (var label in Labels) {
        if (label.Name == name) {
          line = label.Line;
          return true;
        }
      }
      line = default;
      return false;
    }

    public void CreateLabel(string name, int line) {
      Labels.RemoveAll(l => l.Name == name);
      Labels.Add(new Label() {
        Name = name,
        Line = line
      });
    }

    public void CreateCustomVariable(string name, ushort address) {
      CustomVariables.RemoveAll(v => v.Name == name);
      CustomVariables.Add(new CustomVariable() {
        Name = name,
        Address = address
      });
    }

    public void CreateStaticVariable(string name, ushort address) {
      StaticVariables.RemoveAll(v => v.Name == name);
      StaticVariables.Add(new StaticVariable() {
        Name = name,
        Address = address
      });
    }

    [Serializable]
    public class Line {
      public string Text;
      public int Indent;
      public List<Token> Tokens;
      public List<OpAndMetaData> Ops;
    }

    [Serializable]
    public struct CustomVariable {
      public string Name;
      public ushort Address;
    }

    [Serializable]
    public struct StaticVariable {
      public string Name;
      public ushort Address;
    }

    [Serializable]
    public struct Label {
      public string Name;
      public int Line;
    }
  }
}
