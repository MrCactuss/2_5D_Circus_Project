using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioMixer mainMixer;
    public Slider volumeSlider;

    const string MASTER_VOLUME_KEY = "MasterVolumePreference";

    void Start()
    {
        Debug.Log("--- VolumeControl Start() ---"); // ADD THIS

        if (mainMixer == null) Debug.LogError("Main Mixer is NOT assigned in Inspector!"); // ADD THIS
        if (volumeSlider == null) Debug.LogError("Volume Slider is NOT assigned in Inspector!"); // ADD THIS

        float savedVolumeLinear = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);
        Debug.Log("Loaded Volume (Linear): " + savedVolumeLinear); // ADD THIS
        volumeSlider.value = savedVolumeLinear;

        SetMixerVolume(savedVolumeLinear); // Apply initial volume

        volumeSlider.onValueChanged.AddListener(SetMasterVolume);
        Debug.Log("Added listener to slider."); // ADD THIS
    }

    public void SetMasterVolume(float linearVolume)
    {
        Debug.Log("SetMasterVolume called with value: " + linearVolume); // ADD THIS
        SetMixerVolume(linearVolume);
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, linearVolume);
        PlayerPrefs.Save();
    }

    private void SetMixerVolume(float linearVolume)
    {
        float clampedVolume = Mathf.Max(linearVolume, 0.0001f);
        float dBVolume = Mathf.Log10(clampedVolume) * 20f;

        Debug.Log("Attempting to set Mixer Parameter 'MasterVolume' to dB: " + dBVolume); // ADD THIS
        if (mainMixer != null) // ADD NULL CHECK
        {
            mainMixer.SetFloat("MasterVolume", dBVolume);
        }
        else
        {
            Debug.LogError("Cannot set mixer volume because mainMixer is null!"); // ADD ERROR
        }
    }
}
