using System.Collections.Generic;
using UnityEngine;

namespace Axe4Unity {

  public abstract class DataAsset : ScriptableObject {

    public abstract List<byte> GetData();

  }
}
