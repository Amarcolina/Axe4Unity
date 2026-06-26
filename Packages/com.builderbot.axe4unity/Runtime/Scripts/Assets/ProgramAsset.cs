using System.Collections.Generic;

namespace Axe4Unity {

  public class ProgramAsset : DataAsset {

    public Program Program;

    public override List<byte> GetData() => Program.Data;

  }
}
