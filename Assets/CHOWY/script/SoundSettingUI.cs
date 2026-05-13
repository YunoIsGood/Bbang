using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSettingsUI : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterVolumeSlider;

    private const string MasterVolumeKey = "MasterVolume";

    private void Start()
    {
        float savedValue = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);

        masterVolumeSlider.value = savedValue;

        SetMasterVolume(savedValue);

        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
    }

    public void SetMasterVolume(float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);

        float db = Mathf.Log10(value) * 20f;

        audioMixer.SetFloat(MasterVolumeKey, db);

        PlayerPrefs.SetFloat(MasterVolumeKey, value);
    }
}