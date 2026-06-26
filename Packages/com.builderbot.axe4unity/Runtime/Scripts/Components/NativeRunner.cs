using UnityEngine;

namespace Axe4Unity {

  public abstract class NativeRunner : MonoBehaviour {

    public abstract OpAndMetaData Step(Machine machine, int maxSteps);

    public virtual void Start() { }

  }
}
