using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class GraphicsSettingsManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioMixer mainMixer;
    public Slider volumeSlider;

    [Header("Graphics Settings UI")]
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown fullscreenModeDropdown;

    [Header("Control Buttons (Optional)")]
    public Button applyButton; // Assign if you want to disable/enable it

    // --- Internal State Variables ---
    // Settings currently selected in the UI *before* applying
    private float currentVolumeLinear;
    private int currentQualityIndex;
    private int currentResolutionIndex;
    private int currentFullscreenModeIndex;

    // Settings that were last applied/saved
    private float savedVolumeLinear;
    private int savedQualityIndex;
    private int savedResolutionIndex;
    private int savedFullscreenModeIndex;

    private Resolution[] availableResolutions;
    private List<Resolution> filteredResolutions;

    // --- PlayerPrefs Keys ---
    const string MASTER_VOLUME_KEY = "MasterVolumePreference";
    const string QUALITY_LEVEL_KEY = "QualityLevel";
    const string RESOLUTION_WIDTH_KEY = "ResolutionWidth";
    const string RESOLUTION_HEIGHT_KEY = "ResolutionHeight";
    const string FULLSCREEN_MODE_KEY = "FullscreenMode";

    private bool settingsChanged = false; // Flag to track if Apply button should be enabled


    void Start()
    {
        Debug.Log("--- GraphicsSettingsManager Start() Initializing ---");
        // (Null checks for assigned variables)
        if (mainMixer == null || volumeSlider == null || qualityDropdown == null || resolutionDropdown == null || fullscreenModeDropdown == null) { /* ... Error Log & Disable ... */ this.enabled = false; return; }

        // --- Load ALL Saved Settings ---
        LoadAllPreferences();

        // --- Initialize UI & Current State from Saved State ---
        InitializeUI();

        // --- Add Listeners AFTER Initial Setup ---
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        fullscreenModeDropdown.onValueChanged.AddListener(OnFullscreenModeChanged);

        // Start with Apply button disabled as no changes are pending
        settingsChanged = false;
        UpdateApplyButtonState();

        Debug.Log("--- GraphicsSettingsManager Initialization Complete ---");
    }

    // Loads all settings from PlayerPrefs into the 'saved' state variables
    void LoadAllPreferences()
    {
        savedVolumeLinear = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);
        savedQualityIndex = PlayerPrefs.GetInt(QUALITY_LEVEL_KEY, QualitySettings.GetQualityLevel());
        // Validate saved quality index
        if (savedQualityIndex >= QualitySettings.names.Length || savedQualityIndex < 0) { savedQualityIndex = QualitySettings.GetQualityLevel(); }

        savedResolutionIndex = LoadSavedResolutionIndex(); // Use helper for resolution matching
        savedFullscreenModeIndex = PlayerPrefs.GetInt(FULLSCREEN_MODE_KEY, GetCurrentScreenModeIndex());
        // Validate saved fullscreen index
        if (savedFullscreenModeIndex < 0 || savedFullscreenModeIndex >= 3) { savedFullscreenModeIndex = GetCurrentScreenModeIndex(); } // Assuming 3 modes (0,1,2)

        Debug.Log($"Preferences Loaded: Vol={savedVolumeLinear}, Quality={savedQualityIndex}, Res={savedResolutionIndex}, FS={savedFullscreenModeIndex}");
    }

    // Sets up the initial UI state based on the 'saved' variables
    void InitializeUI()
    {
        // --- Volume ---
        currentVolumeLinear = savedVolumeLinear;
        volumeSlider.value = savedVolumeLinear;
        SetMixerVolume(savedVolumeLinear); // Apply initial volume to mixer immediately

        // --- Quality ---
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
        currentQualityIndex = savedQualityIndex;
        qualityDropdown.value = savedQualityIndex;
        qualityDropdown.RefreshShownValue();
        ApplyQualityLevel(savedQualityIndex); // Apply initial quality immediately

        // --- Resolution & Fullscreen ---
        SetupResolutionDropdown(); // Populates dropdown and finds saved index
        currentResolutionIndex = savedResolutionIndex; // Set current state
        resolutionDropdown.value = savedResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        fullscreenModeDropdown.ClearOptions();
        fullscreenModeDropdown.AddOptions(new List<string> { "Fullscreen", "Borderless Window", "Windowed" });
        currentFullscreenModeIndex = savedFullscreenModeIndex; // Set current state
        fullscreenModeDropdown.value = savedFullscreenModeIndex;
        fullscreenModeDropdown.RefreshShownValue();

        // Note: Resolution/Fullscreen are NOT applied here to prevent flicker on load.
        // The *initial* screen state is assumed correct based on load/platform defaults.
        // We only apply changes explicitly via Apply/Revert.
    }


    // ===================================================================
    // UI Listener Methods (Update internal state ONLY)
    // ===================================================================

    public void OnVolumeChanged(float value)
    {
        currentVolumeLinear = value;
        MarkSettingsChanged();
    }

    public void OnQualityChanged(int index)
    {
        currentQualityIndex = index;
        MarkSettingsChanged();
    }

    public void OnResolutionChanged(int index)
    {
        currentResolutionIndex = index;
        MarkSettingsChanged();
    }

    public void OnFullscreenModeChanged(int index)
    {
        currentFullscreenModeIndex = index;
        MarkSettingsChanged();
    }

    // Helper to track pending changes and enable Apply button
    void MarkSettingsChanged()
    {
        settingsChanged = true;
        UpdateApplyButtonState();
    }

    void UpdateApplyButtonState()
    {
        if (applyButton != null)
        {
            applyButton.interactable = settingsChanged;
        }
    }


    // ===================================================================
    // Apply / Revert Logic (Called by Buttons)
    // ===================================================================

    public void ApplyChanges()
    {
        Debug.Log("ApplyChanges Called.");
        // Apply ALL current settings
        SetMixerVolume(currentVolumeLinear);
        ApplyQualityLevel(currentQualityIndex);
        ApplyResolutionAndFullscreen(currentResolutionIndex, currentFullscreenModeIndex);

        // Save ALL current settings as the new 'saved' state
        savedVolumeLinear = currentVolumeLinear;
        savedQualityIndex = currentQualityIndex;
        savedResolutionIndex = currentResolutionIndex;
        savedFullscreenModeIndex = currentFullscreenModeIndex;

        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, savedVolumeLinear);
        PlayerPrefs.SetInt(QUALITY_LEVEL_KEY, savedQualityIndex);
        SaveResolutionPreference(savedResolutionIndex);
        SaveFullscreenPreference(savedFullscreenModeIndex);
        PlayerPrefs.Save(); // Ensure all prefs are written

        settingsChanged = false; // Reset changed flag
        UpdateApplyButtonState(); // Disable Apply button
        Debug.Log("All settings applied and saved.");
    }

    public void RevertChanges()
    {
        Debug.Log("RevertChanges Called.");
        // Reset the UI selections AND current state back to the last SAVED state
        currentVolumeLinear = savedVolumeLinear;
        currentQualityIndex = savedQualityIndex;
        currentResolutionIndex = savedResolutionIndex;
        currentFullscreenModeIndex = savedFullscreenModeIndex;

        volumeSlider.value = savedVolumeLinear;
        qualityDropdown.value = savedQualityIndex;
        resolutionDropdown.value = savedResolutionIndex;
        fullscreenModeDropdown.value = savedFullscreenModeIndex;

        qualityDropdown.RefreshShownValue();
        resolutionDropdown.RefreshShownValue();
        fullscreenModeDropdown.RefreshShownValue();

        // Re-apply the SAVED settings to Mixer, Quality, and Screen
        SetMixerVolume(savedVolumeLinear);
        ApplyQualityLevel(savedQualityIndex);
        ApplyResolutionAndFullscreen(savedResolutionIndex, savedFullscreenModeIndex);

        settingsChanged = false; // Reset changed flag
        UpdateApplyButtonState(); // Disable Apply button
        Debug.Log("All settings reverted to last applied state.");
    }


    // ===================================================================
    // Helper Methods (Setup, Apply, Save specific parts)
    // ===================================================================

    // --- Volume Helpers ---
    private void SetMixerVolume(float linearVolume)
    {
        float clampedVolume = Mathf.Max(linearVolume, 0.0001f);
        float dBVolume = Mathf.Log10(clampedVolume) * 20f;
        mainMixer.SetFloat("MasterVolume", dBVolume);
    }

    // --- Quality Helpers ---
    private void ApplyQualityLevel(int index)
    {
        if (index >= 0 && index < QualitySettings.names.Length) { QualitySettings.SetQualityLevel(index, true); }
    }

    // --- Resolution & Fullscreen Helpers ---
    void SetupResolutionDropdown() // Renamed from SetupResolutionAndFullscreen
    {
        availableResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        filteredResolutions = new List<Resolution>();
        Dictionary<string, Resolution> uniqueResolutions = new Dictionary<string, Resolution>();
        foreach (Resolution res in availableResolutions)
        {
            if (res.width < 800 || res.height < 600) continue;
            string key = $"{res.width}x{res.height}";
            if (!uniqueResolutions.ContainsKey(key) || uniqueResolutions[key].refreshRateRatio.value < res.refreshRateRatio.value) { uniqueResolutions[key] = res; }
        }
        filteredResolutions = uniqueResolutions.Values.OrderBy(r => r.width).ThenBy(r => r.height).ToList();

        List<string> resolutionOptions = new List<string>();
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            Resolution res = filteredResolutions[i];
            string option = $"{res.width} x {res.height} @ {res.refreshRateRatio.value.ToString("F0")} Hz";
            resolutionOptions.Add(option);
        }
        resolutionDropdown.AddOptions(resolutionOptions);
        // Note: Actual saved index is found/set in LoadAllPreferences / InitializeUI
    }

    private int LoadSavedResolutionIndex()
    {
        // Need to populate filteredResolutions first if not already done
        if (filteredResolutions == null || filteredResolutions.Count == 0)
        {
            // Minimal setup just to find the index
            availableResolutions = Screen.resolutions;
            filteredResolutions = new List<Resolution>();
            Dictionary<string, Resolution> uniqueResolutions = new Dictionary<string, Resolution>();
            foreach (Resolution res in availableResolutions) { if (res.width < 800 || res.height < 600) continue; string key = $"{res.width}x{res.height}"; if (!uniqueResolutions.ContainsKey(key) || uniqueResolutions[key].refreshRateRatio.value < res.refreshRateRatio.value) { uniqueResolutions[key] = res; } }
            filteredResolutions = uniqueResolutions.Values.OrderBy(r => r.width).ThenBy(r => r.height).ToList();
        }


        int loadedWidth = PlayerPrefs.GetInt(RESOLUTION_WIDTH_KEY, Screen.currentResolution.width);
        int loadedHeight = PlayerPrefs.GetInt(RESOLUTION_HEIGHT_KEY, Screen.currentResolution.height);
        int loadedIndex = 0; // Default
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            if (filteredResolutions[i].width == loadedWidth && filteredResolutions[i].height == loadedHeight)
            {
                loadedIndex = i;
                break;
            }
        }
        return loadedIndex;
    }


    private void ApplyResolutionAndFullscreen(int resIndex, int fsIndex)
    {
        if (resIndex < 0 || resIndex >= filteredResolutions.Count) return;
        Resolution selectedRes = filteredResolutions[resIndex];
        FullScreenMode selectedMode = FullScreenMode.Windowed;
        if (fsIndex == 0) selectedMode = FullScreenMode.ExclusiveFullScreen;
        else if (fsIndex == 1) selectedMode = FullScreenMode.FullScreenWindow;
        else if (fsIndex == 2) selectedMode = FullScreenMode.Windowed;
        Debug.Log($"Applying Graphics: {selectedRes.width}x{selectedRes.height}, Mode: {selectedMode}");
        Screen.SetResolution(selectedRes.width, selectedRes.height, selectedMode);
    }

    private void SaveResolutionPreference(int indexToSave)
    {
        if (indexToSave >= 0 && indexToSave < filteredResolutions.Count)
        {
            Resolution res = filteredResolutions[indexToSave];
            PlayerPrefs.SetInt(RESOLUTION_WIDTH_KEY, res.width);
            PlayerPrefs.SetInt(RESOLUTION_HEIGHT_KEY, res.height);
            // PlayerPrefs.Save(); // Save is called once in ApplyChanges
        }
    }

    private void SaveFullscreenPreference(int indexToSave)
    {
        PlayerPrefs.SetInt(FULLSCREEN_MODE_KEY, indexToSave);
        // PlayerPrefs.Save(); // Save is called once in ApplyChanges
    }

    private int GetCurrentScreenModeIndex()
    {
        FullScreenMode currentMode = Screen.fullScreenMode;
        if (currentMode == FullScreenMode.ExclusiveFullScreen) return 0;
        if (currentMode == FullScreenMode.FullScreenWindow) return 1;
        if (currentMode == FullScreenMode.Windowed || currentMode == FullScreenMode.MaximizedWindow) return 2;
        return 1;
    }
}