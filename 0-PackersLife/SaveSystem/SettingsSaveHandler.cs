using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsSaveHandler : MonoBehaviour
{
    [SerializeField] private Slider _mouseSensitivitySlider;
    [SerializeField] private TMP_Text _sensitivityValueText;

    [System.Serializable]
    public class SettingsData
    {
        public float MouseSensitivity;
    }

    public void LoadData(SettingsData data)
    {
        CameraMovement.Instance.MouseSensitivity = data.MouseSensitivity;
        _mouseSensitivitySlider.value = data.MouseSensitivity;
        _sensitivityValueText.text = data.MouseSensitivity.ToString("F1");
    }

    public SettingsData CreateData()
    {
        SettingsData data = new()
        {
            MouseSensitivity = _mouseSensitivitySlider.value
        };

        return data;
    }
}