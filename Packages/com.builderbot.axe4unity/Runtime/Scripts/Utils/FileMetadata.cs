using System;
using UnityEngine;
using Unity.Collections;

namespace Axe4Unity {

  [Serializable]
  public struct FileMetadata {
    public FixedString32Bytes Name;
    public ushort Address;
    public ushort Size;
    public bool IsArchived;
  }
}
