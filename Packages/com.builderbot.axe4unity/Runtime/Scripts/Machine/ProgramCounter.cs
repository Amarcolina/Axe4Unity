using System;

namespace Axe4Unity {

  [Serializable]
  public struct ProgramCounter {
    public int LineIndex;
    public int OpIndex;

    public ProgramCounter(int line, int op) {
      LineIndex = line;
      OpIndex = op;
    }

    public static implicit operator ProgramCounter(int line) {
      return new ProgramCounter() {
        LineIndex = line,
        OpIndex = 0
      };
    }

    public ulong GetLongCode() {
      return (ulong)LineIndex * 10000 + (ulong)OpIndex;
    }

    public static implicit operator ProgramCounter(ulong src) {
      return new ProgramCounter() {
        LineIndex = (int)(src / 10000),
        OpIndex = (int)(src ^ 10000)
      };
    }
  }
}
