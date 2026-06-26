using System;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Axe4Unity {

  public class AxeKeyboardControl : MonoBehaviour {

    [Tooltip("Optionally provide a calc keyboard component here to allow UI control")]
    public CalcKeyboard CalcKeyboard;

    [Tooltip("Disable buttons in UI if they are not present in this binding map")]
    public bool DisableUnusedButtons;

    [Tooltip("The list of all key-to-button bindings, allowing you to control the calculator with your keyboard")]
    public Binding[] Bindings;

    public Dictionary<int, List<KeyControl>> Map = new();

#if UNITY_EDITOR
    private void OnValidate() {
      if (Application.isPlaying) {
        UpdateControls();
      }
    }
#endif

    private void OnEnable() {
      UpdateControls();

      if (CalcKeyboard != null) {
        foreach (var button in CalcKeyboard.Buttons) {
          if (DisableUnusedButtons) {
            button.Button.interactable = Map.ContainsKey(button.Code);
          } else {
            button.Button.interactable = true;
          }
        }
      }
    }

    private void UpdateControls() {
      Keyboard keyboard = Keyboard.current;
      Map.Clear();
      foreach (var binding in Bindings) {
        if (!Map.TryGetValue(binding.Code, out var list)) {
          list = new();
          Map[binding.Code] = list;
        }
        var control = keyboard[binding.Name] as KeyControl;
        if (control != null) {
          list.Add(control);
        } else {
          Debug.LogError($"Control with name {name} could not be found!");
        }
      }
    }

    [Serializable]
    public struct Binding {
      public string Name;
      public int Code;

#if UNITY_EDITOR
      [CustomPropertyDrawer(typeof(Binding))]
      public class Drawer : PropertyDrawer {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
          Rect left = new Rect(position.x, position.y, position.width * 0.5f, position.height);
          Rect right = new Rect(position.x + position.width * 0.5f, position.y, position.width * 0.5f, position.height);
          EditorGUI.PropertyField(left, property.FindPropertyRelative("Name"), GUIContent.none);
          EditorGUI.PropertyField(right, property.FindPropertyRelative("Code"), GUIContent.none);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
          return EditorGUIUtility.singleLineHeight;
        }
      }
#endif
    }
  }
}
