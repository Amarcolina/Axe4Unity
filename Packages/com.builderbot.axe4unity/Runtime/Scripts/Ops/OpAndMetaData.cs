using System;
using UnityEngine;

namespace Axe4Unity {

  [Serializable]
  public class OpAndMetaData {
    public string Display;
    public string Type;
    public int Row, ColStart, ColEnd;
    public int LineIndex, OpIndex;
    public bool IsDataAddr;
    public bool IsStringData;

    [SerializeReference]
    public IOp Op;
  }
}
