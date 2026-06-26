using System.Collections.Generic;

namespace Axe4Unity {

  public class AppVarAsset : DataAsset {

    public List<byte> Data;

    public override List<byte> GetData() => Data;
  }
}
