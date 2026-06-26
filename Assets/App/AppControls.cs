using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Axe4Unity;

public class AppControls : MonoBehaviour {

  public Entry[] Entries;
  public float[] SimScales = new float[] {
    0.1f,
    0.2f,
    0.5f,
    1f,
    2f,
    3f,
    10f
  };

  public Slider ScaleSlider;
  public Dropdown Dropdown;
  public Text ScaleLabel;
  public Text Description;
  public Button LoadStateButton;

  private MachineState _saveState;
  private Entry _currEntry;

  [Serializable]
  public class Entry {
    public string Name;
    public AxeRunner Runner;
    [TextArea]
    public string Description;
  }

  private void Awake() {
    foreach (var entry in Entries) {
      entry.Runner.gameObject.SetActive(false);
    }

    Dropdown.options = Entries.Select(t => new Dropdown.OptionData() {
      text = t.Name
    }).ToList();
    LoadStateButton.interactable = false;

    LoadProgram(0);
  }

  public void SetScaleLevel(float level) {
    int index = Mathf.RoundToInt(level);
    _currEntry.Runner.SimulationScale = SimScales[index];
    ScaleLabel.text = $"Timescale: {Mathf.RoundToInt(SimScales[index] * 100)}%";
  }

  public void RestartProgram() {
    _currEntry.Runner.Machine.Reset();
    _currEntry.Runner.LoadFiles();
    _currEntry.Runner.Running = true;
  }

  public void SaveState() {
    _saveState.CopyFrom(_currEntry.Runner.Machine.State);
    LoadStateButton.interactable = true;
  }

  public void LoadState() {
    _saveState.CopyTo(_currEntry.Runner.Machine.State);
  }

  public void LoadProgram(int index) {
    if (_currEntry != null) {
      _currEntry.Runner.gameObject.SetActive(false);
    }

    _currEntry = Entries[index];
    _currEntry.Runner.gameObject.SetActive(true);

    Description.text = _currEntry.Description;

    LoadStateButton.interactable = false;

    SetScaleLevel(3);

    RestartProgram();
  }

}
