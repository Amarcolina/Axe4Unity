using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Axe4Unity {

  public class CalcButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    public Button Button;
    public int Code;

    public bool IsPressed { get; set; }

    [ContextMenu("Update Name")]
    public void UpdateName() {
      gameObject.name = "CalcButton_" + Code;
    }

    public void OnPointerDown(PointerEventData eventData) {
      IsPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
      IsPressed = false;
    }
  }
}
