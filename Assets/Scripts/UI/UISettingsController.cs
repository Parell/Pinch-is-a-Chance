using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UISettingsController : MonoBehaviour
{
    [SerializeField] private Dropdown _resolutionDropdown;
    [SerializeField] private Toggle _fullscreenToggle;
    [SerializeField] private Toggle _vsyncToggle;
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _effectsSlider;
    [Space]
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private List<Resolution> _resolutions;

    private string directory = "/";
    private string fileName = "Settings.json";
    private string path;
    private SettingsData settingsData;

    private void Awake()
    {
        settingsData = new SettingsData();

        GUIUtility.systemCopyBuffer = Application.persistentDataPath;

        //path = Application.persistentDataPath + directory;

        //if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        SetResolutionDropdown();

        _resolutionDropdown.onValueChanged.AddListener(SetResolution);
        _fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        _vsyncToggle.onValueChanged.AddListener(SetVsync);
        _masterSlider.onValueChanged.AddListener(SetMasterVolume);
        _musicSlider.onValueChanged.AddListener(SetMusicVolume);
        _effectsSlider.onValueChanged.AddListener(SetEffectsVolume);

        LoadSettings();
    }

    #region Settings
    private void SetResolutionDropdown()
    {
        int selected = 0;
        var options = new List<Dropdown.OptionData>();

        for (int i = 0; i < _resolutions.Count; i++)
        {
            if (Screen.width == _resolutions[i].width && Screen.height == _resolutions[i].height)
            {
                selected = i;
            }
            else
            {
                Resolution newResolution = new Resolution();
                newResolution.width = Screen.width;
                newResolution.height = Screen.height;

                _resolutions.Add(newResolution);
                selected = _resolutions.Count - 1;
            }

            string text = _resolutions[i].width + "x" + _resolutions[i].height;

            options.Add(new Dropdown.OptionData(text));
        }

        _resolutionDropdown.options = options;
        _resolutionDropdown.value = selected;
    }

    public void SetResolution(int value)
    {
        var resolution = _resolutions[value];
        Screen.SetResolution(resolution.width, resolution.height, _fullscreenToggle.isOn);
    }

    public void SetFullscreen(bool value)
    {
        Screen.fullScreen = value;
        SetResolution(_resolutionDropdown.value);
    }

    public void SetVsync(bool value)
    {
        if (!value) QualitySettings.vSyncCount = 0;
        else QualitySettings.vSyncCount = 1;
    }

    public void SetMasterVolume(float value)
    {
        _audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
    }

    public void SetMusicVolume(float value)
    {
        _audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
    }

    public void SetEffectsVolume(float value)
    {
        _audioMixer.SetFloat("EffectsVolume", Mathf.Log10(value) * 20);
    }
    #endregion

    public void RestoreSettings()
    {
        settingsData.resolution = 3;
        settingsData.fullscreen = false;
        settingsData.vsync = true;
        settingsData.masterVolume = 1;
        settingsData.musicVolume = 1;
        settingsData.effectsVolume = 1;

        Debug.Log("Restoring data");
    }

    private void LoadSettings()
    {
        if (File.Exists(path + fileName))
        {
            string json = File.ReadAllText(path + fileName);
            settingsData = JsonUtility.FromJson<SettingsData>(json);

            Debug.Log("Loading data");
        }
        else RestoreSettings();

        _resolutionDropdown.value = settingsData.resolution;
        _fullscreenToggle.isOn = settingsData.fullscreen;
        _vsyncToggle.isOn = settingsData.vsync;
        _masterSlider.value = settingsData.masterVolume;
        _musicSlider.value = settingsData.musicVolume;
        _effectsSlider.value = settingsData.effectsVolume;
    }

    public void SaveSettings()
    {
        settingsData.resolution = _resolutionDropdown.value;
        settingsData.fullscreen = _fullscreenToggle.isOn;
        settingsData.vsync = _vsyncToggle.isOn;
        settingsData.masterVolume = _masterSlider.value;
        settingsData.musicVolume = _musicSlider.value;
        settingsData.effectsVolume = _effectsSlider.value;

        string json = JsonUtility.ToJson(settingsData, true);
        File.WriteAllText(path + fileName, json);

        Debug.Log("Saving data");
    }

    private void OnApplicationQuit()
    {
        SaveSettings();
    }
}

[System.Serializable]
public class Resolution
{
    public int width, height;
}

public class SettingsData
{
    public int resolution;
    public bool fullscreen, vsync;
    public float masterVolume, musicVolume, effectsVolume;
}