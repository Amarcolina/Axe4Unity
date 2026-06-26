using System.Collections.Generic;
using UnityEngine;

namespace Axe4Unity {

  public class CalcKeyboard : MonoBehaviour {

    public CalcButton[] Buttons;

    public Dictionary<int, CalcButton> CodeToButton = new();

    private void Start() {
      foreach (var button in Buttons) {
        CodeToButton[button.Code] = button;
      }
    }

  }
}
