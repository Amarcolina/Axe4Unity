using UnityEngine;

namespace Axe4Unity {

  public abstract class NativeRunner : MonoBehaviour {

    public abstract Results Step(Machine machine, int maxSteps, int maxGetKey);

    public virtual void Start() { }

    public struct Results {
      public int StepsCompleted;
      public bool IsGetKeyTimeout;
      public OpAndMetaData LastOpExecuted;
    }

  }
}
