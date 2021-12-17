using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(0)]
public class sSettings : MonoBehaviour
{
    public static sSettings instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    [SerializeField] TextMeshProUGUI settingNameText;
    [SerializeField] TextMeshProUGUI settingDescText;

    [Header("UI objects")]
    [SerializeField] Slider xSensitivitySlider;
    [SerializeField] Slider ySensitivitySlider;
    [SerializeField] Toggle autoSleepToggle;
    [SerializeField] TMP_Dropdown minimapStyleDropDown;
    [SerializeField] TMP_Dropdown clockStyleDropDown;
    [SerializeField] Toggle cameraShakeToggle;
    [SerializeField] Slider defaultCamDistanceSlider;
    [SerializeField] TMP_Dropdown sprintTypeDropDown;
    [SerializeField] Toggle cheatsEnabledToggle;
    [SerializeField] Toggle autoSkipCutsceneToggle;

    [SerializeField] Slider masterVolSlider;
    [SerializeField] Slider musicVolSlider;
    [SerializeField] Slider ambientVolSlider;
    [SerializeField] Slider SFXVolSlider;
    [SerializeField] Toggle musicPopUpToggle;

    [SerializeField] TMP_Dropdown qulityDropDown;
    [SerializeField] TMP_Dropdown displayModeDropDown;
    [SerializeField] TMP_Dropdown resolutionDropDown;
    [SerializeField] Slider brightnessSlider;
    [SerializeField] TMP_Dropdown fpsCapDropDown;
    [SerializeField] Toggle vSyncToggle;
    [SerializeField] TMP_Dropdown antiAliasingDropDown;
    [SerializeField] TMP_Dropdown motionBlurDropDown;
    [SerializeField] Slider bloomSlider;
    [SerializeField] Toggle showPerformanceToggle;

    [Header("Text Value objects")]
    [SerializeField] TextMeshProUGUI xSensitivityText;
    [SerializeField] TextMeshProUGUI ySensitivityText;
    [SerializeField] TextMeshProUGUI autoSleepText;
    [SerializeField] TextMeshProUGUI cameraShakeText;
    [SerializeField] TextMeshProUGUI defaultCamDistText;
    [SerializeField] TextMeshProUGUI cheatsEnabledText;
    [SerializeField] TextMeshProUGUI autoSkipCutsceneText;

    [SerializeField] TextMeshProUGUI masterVolText;
    [SerializeField] TextMeshProUGUI ambientVolText;
    [SerializeField] TextMeshProUGUI musicVolText;
    [SerializeField] TextMeshProUGUI SFXVolText;
    [SerializeField] TextMeshProUGUI musicPopUpText;

    [SerializeField] TextMeshProUGUI brightnessText;
    [SerializeField] TextMeshProUGUI vSyncText;
    [SerializeField] TextMeshProUGUI bloomText;
    [SerializeField] TextMeshProUGUI showPerformanceText;

    [Header("Variables")]
    public float xSensitivity;
    public float ySensitivity;
    public bool autoSleep;
    public int minimapStyle;
    public int clockStyle;
    public bool cameraShake;
    public float defaultCamDistance;
    public int sprintType;
    public bool cheatsEnabled;
    public bool autoSkipCutscene;

    public float masterVol;
    public float ambientVol;
    public float musicVol;
    public float SFXVol;
    public bool musicPopUp;

    public int quality;
    public int displayMode;
    public int resolution;
    public float brightness;
    public int fpsCap;
    public bool vSync;
    public int antiAliasing;
    public int motionBlur;
    public float bloom;
    public bool showPerformance;

    string xSensitivityParam = "xSense";
    string ySensitivityParam = "ySense";
    string autoSleepParam = "AutoSleep";
    string minimapStyleParam = "MinimapStyle";
    string clockStyleParam = "ClockStyle";
    string cameraShakeParam = "ClockStyle";
    string defaultCamDistParam = "DefaultCamDist";
    string sprintTypeParam = "SprintType";
    string cheatsEnabledParam = "CheatsEnabled";
    string autoSkipCutsceneParam = "AutoSkipCutsceneEnabled";

    string masterVolParam = "Master";
    string ambientVolParam = "AmbientVol";
    string musicVolParam = "MusicVol";
    string SFXVolParam = "SFXVol";
    string musicPopUpParam = "MusicPopUp";

    string qualtiyParam = "Quality";
    string displayModeParam = "DisplayMode";
    string resolutionParam = "Resolution";
    string brightnessParam = "Brightness";
    string fpsCapParam = "FPSCap";
    string vSyncParam = "VSync";
    string antiAliasingParam = "AntiAliasing";
    string motionBlurParam = "MotionBlur";
    string bloomParam = "Bloom";
    string showPerformanceParam = "ShowPerformance";

    [Header("Settings Objects")]
    [SerializeField] GameObject gameplaySettingsObject;
    [SerializeField] GameObject audioSettingsObject;
    [SerializeField] GameObject videoSettingsObject;
    [SerializeField] GameObject keyBindSettingsObject;
    [SerializeField] TextMeshProUGUI settingsTitleText;
    GameObject currentSettingsObject;

    int[] resolutionsInt;

    [SerializeField] GameObject restoreDefaultObject;

    // Start is called before the first frame update
    void Start()
    {

        PopulateResolutionDropdown();

        settingNameText.text = "";
        settingDescText.text = "";

        currentSettingsObject = gameplaySettingsObject;

        AssignPlayerPrefValues();
        
    }


    public void PopulateResolutionDropdown()
    {
        resolutionDropDown.ClearOptions();

        Resolution[] resolutions = Screen.resolutions;
        List<string> temp = new List<string>();
        resolutionsInt = new int[resolutions.Length * 3];

        int i = 0;
        foreach (var res in resolutions)
        {
            resolutionsInt[i] = res.width;
            resolutionsInt[i + 1] = res.height;
            resolutionsInt[i + 2] = res.refreshRate;
            i += 3;
            temp.Add(res.width + "x" + res.height);
        }
        resolutionDropDown.AddOptions(temp);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateSettingData(string settingName, string settingDesc)
    {
        settingNameText.text = settingName;
        settingDescText.text = settingDesc;
    }

    public void ChangeSettingsObject(GameObject newSettingsObject)
    {
        currentSettingsObject.SetActive(false);
        currentSettingsObject = newSettingsObject;
        newSettingsObject.SetActive(true);
    }

    public void ChangeSettingsTitle(string newTitleAddition) 
    {
        settingsTitleText.text = "Settings - " + newTitleAddition;
    }

    #region Gameplay

    public void XSensitivityChanged(Single value)
    {
        xSensitivity = value;
        xSensitivityText.text = value.ToString("F1");
        if (GameManager.instance != null)
        {
            if (GameManager.instance.cinemachineController != null)
            {
                GameManager.instance.cinemachineController.XChanged(value);
            }
        }
        PlayerPrefs.SetFloat(xSensitivityParam, value);
    }

    public void YSensitivityChanged(Single value)
    {
        ySensitivity = value;
        ySensitivityText.text = value.ToString("F1");
        if (GameManager.instance != null)
        {
            if (GameManager.instance.cinemachineController != null)
            {
                GameManager.instance.cinemachineController.YChanged(value);
            }
        }
        PlayerPrefs.SetFloat(ySensitivityParam, value);
    }

    public void AutoSleepChanged(Boolean value)
    {
        autoSleep = value;
        autoSleepToggle.isOn = value;
        int prefVal = value ? 1 : 0;
        autoSleepText.text = value ? "On" : "Off";
        PlayerPrefs.SetInt(autoSleepParam, prefVal);
    }

    public void MinimapStyleChanged(Int32 value)
    {
        minimapStyle = value;
        if (sMainUI.instance != null)
        {
            if (sMainUI.instance.miniMap != null)
            {
                sMainUI.instance.miniMap.ChangeMiniMapStyle(value == 1);
            }
        }
        PlayerPrefs.SetInt(minimapStyleParam, value);
    }

    public void ClockStyleChanged(Int32 value)
    {
        clockStyle = value;
        PlayerPrefs.SetInt(clockStyleParam, value);
    }

    public void CameraShakeChanged(Boolean value)
    {
        cameraShake = value;
        cameraShakeToggle.isOn = value;
        int prefVal = value ? 1 : 0;
        cameraShakeText.text = value ? "On" : "Off";
        PlayerPrefs.SetInt(cameraShakeParam, prefVal);
    }

    public void DefaultCamDistanceChanged(Single value)
    {
        defaultCamDistance = value;
        defaultCamDistText.text = value.ToString("F1");
        PlayerPrefs.SetFloat(defaultCamDistParam, value);
    }

    public void SprintTypeChanged(Int32 value)
    {
        sprintType = value;
        PlayerPrefs.SetInt(sprintTypeParam, value);
    }

    public void CheatsEnabledChanged(Boolean value)
    {
        cheatsEnabled = value;
        cheatsEnabledToggle.isOn = value;
        int prefVal = value ? 1 : 0;
        cheatsEnabledText.text = value ? "On" : "Off";
        PlayerPrefs.SetInt(cheatsEnabledParam, prefVal);
    }


    public void AutoSkipCutsceneChanged(Boolean value)
    {
        autoSkipCutscene = value;
        autoSkipCutsceneToggle.isOn = value;
        int prefVal = value ? 1 : 0;
        autoSkipCutsceneText.text = value ? "On" : "Off";
        PlayerPrefs.SetInt(autoSkipCutsceneParam, prefVal);
    }

    #endregion

    #region Audio
    public void MasterVolChanged(Single value)
    {
        masterVol = value;
        masterVolText.text =  (int)Mathf.Lerp(0f, 100f, value) + "%";
        AudioManager.instance.UpdateMasterVolume(value);
        PlayerPrefs.SetFloat(masterVolParam, value);
    }

    public void AmbientVolChanged(Single value)
    {
        ambientVol = value;
        ambientVolText.text = (int)Mathf.Lerp(0, 100, value) + "%";
        AudioManager.instance.UpdateAmbientVolume(value);
        PlayerPrefs.SetFloat(ambientVolParam, value);
    }

    public void MusicVolChanged(Single value)
    {
        musicVol = value;
        musicVolText.text = (int)Mathf.Lerp(0, 100, value) + "%";
        AudioManager.instance.UpdateMusicVolume(value);
        PlayerPrefs.SetFloat(musicVolParam, value);
    }

    public void SFXVolChanged(Single value)
    {
        SFXVol = value;
        SFXVolText.text = (int)Mathf.Lerp(0, 100, value) + "%";
        AudioManager.instance.UpdateSFXVolume(value);
        PlayerPrefs.SetFloat(SFXVolParam, value);
    }

    public void MusicPopUpChanged(Boolean value)
    {
        musicPopUp = value;
        musicPopUpToggle.isOn = value;
        int prefVal = value ? 1 : 0;
        PlayerPrefs.SetInt(musicPopUpParam, prefVal);
    }
    #endregion

    #region Video

    public void QualityChanged(Int32 value)
    {
        quality = value;
        QualitySettings.SetQualityLevel(value);
        PlayerPrefs.SetInt(qualtiyParam, value);
    }

    public void DisplayModeChanged(Int32 value)
    {
        displayMode = value;
        switch (value)
        {
            case 0: // borderless window
                Screen.fullScreen = false;
                Screen.SetResolution(Screen.width, Screen.height, false);
                break;
            case 1: // fullscreen
                Screen.SetResolution(Screen.width, Screen.height, true);
                Screen.fullScreen = true;
                break;
            case 2: // window
                Screen.fullScreen = false;
                Screen.SetResolution(resolutionsInt[resolution * 3], resolutionsInt[resolution * 3 + 1], false);
                break;
            default:
                break;
        }
        PlayerPrefs.SetInt(displayModeParam, value);
        if (sMainUI.instance != null)
        {
            sMainUI.instance.ShowCursor = true;
        }
    }

    public void ResolutionChanged(Int32 value)
    {
        resolution = value;
        Screen.fullScreen = false;
        Screen.SetResolution(resolutionsInt[resolution * 3], resolutionsInt[resolution * 3 + 1], false);
        PlayerPrefs.SetInt(resolutionParam, value);
    }

    public void FPSCapChanged(Int32 value)
    {
        minimapStyle = value;
        PlayerPrefs.SetInt(fpsCapParam, value);
        int targetFPS = 60;
        switch (value)
        {
            case 0:
                targetFPS = 30;
                break;
            case 1:
                targetFPS = 60;
                break;
            case 2:
                targetFPS = 90;
                break;
            case 3:
                targetFPS = 120;
                break;
            case 4:
                targetFPS = 1000;
                break;
        }
        Application.targetFrameRate = targetFPS;
    }
    public void VSyncChanged(Boolean value)
    {
        vSync = value;
        vSyncToggle.isOn = value;
        int prefVal = value ? 1 : 0;
        PlayerPrefs.SetInt(vSyncParam, prefVal);
        vSyncText.text = value ? "On" : "Off";
        QualitySettings.vSyncCount = prefVal; // 0 or 1 * 4, 0 or 4
    }
    public void AntiAliasingChanged(Int32 value)
    {
        antiAliasing = value;
        PlayerPrefs.SetInt(antiAliasingParam, value);
        QualitySettings.antiAliasing = value;
    }
    public void MotionBlurChanged(Int32 value)
    {
        motionBlur = value;
        if (PostProcessEffects.instance != null)
        {
            PostProcessEffects.instance.SetMotionBlurValue(value);
        }
        PlayerPrefs.SetInt(motionBlurParam, value);
    }
    public void BrightnessSlider(Single value)
    {
        brightness = value;
        brightnessText.text = (int)Mathf.Lerp(0, 100, value) + "%";
        if (PostProcessEffects.instance != null)
        {
            PostProcessEffects.instance.SetGainValue(value);
        }
        PlayerPrefs.SetFloat(brightnessParam, value);
    }
    public void BloomSlider(Single value)
    {
        bloom = value;
        bloomText.text = (int)Mathf.Lerp(0, 100, value) + "%";
        if (PostProcessEffects.instance != null)
        {
            PostProcessEffects.instance.SetBloomValue(value);
        }
        PlayerPrefs.SetFloat(bloomParam, value);
    }
    public void ShowPerformanceToggle(Boolean value)
    {
        print("YSSS");

        showPerformance = value;
        showPerformanceToggle.isOn = value;
        int prefVal = value ? 1 : 0;
        showPerformanceText.text = value ? "On" : "Off";
        if (sMainUI.instance != null)
        {
            sMainUI.instance.ShowHidePerformance(value);
        }
        PlayerPrefs.SetInt(showPerformanceParam, prefVal);
    }
    public bool GetShowPerformance()
    {
        return showPerformanceToggle.isOn;
    }

    #endregion

    public void RestoreDefaultSettings()
    {
        xSensitivitySlider.value = 5f;
        ySensitivitySlider.value = 5f;
        AutoSleepChanged(true);
        minimapStyleDropDown.value = 0;
        clockStyleDropDown.value = 0;
        CameraShakeChanged(true);
        //defaultCamDistanceSlider.value = PlayerPrefs.GetFloat(defaultCamDistParam, 4f);
        sprintTypeDropDown.value = 0;
        CheatsEnabledChanged(false);
        AutoSkipCutsceneChanged(false);
        
        masterVolSlider.value = 0.5f;
        ambientVolSlider.value = 0.5f;
        musicVolSlider.value = 0.5f;
        SFXVolSlider.value = 0.5f;
        MusicPopUpChanged(true);

        qulityDropDown.value = 0;
        //displayModeDropDown.value = PlayerPrefs.GetInt(displayModeParam, 0);
        //resolutionDropDown.value = PlayerPrefs.GetInt(resolutionParam, 0);
        fpsCapDropDown.value = 1;
        VSyncChanged(false);
        //vSyncToggle.isOn = PlayerPrefs.GetInt(vSyncParam, 0) == 0 ? false : true;
        antiAliasingDropDown.value = 0;
        motionBlurDropDown.value = 0;
        brightnessSlider.value = 1f;
        bloomSlider.value = 0;
        ShowPerformanceToggle(true);
    }

    public void AssignPlayerPrefValues()
    {
        xSensitivitySlider.value = PlayerPrefs.GetFloat(xSensitivityParam, 1f);
        ySensitivitySlider.value = PlayerPrefs.GetFloat(ySensitivityParam, 1f);
        autoSleepToggle.isOn = (Boolean)(PlayerPrefs.GetInt(autoSleepParam, 1) == 1);
        minimapStyleDropDown.value = PlayerPrefs.GetInt(minimapStyleParam, 0);
        clockStyleDropDown.value = PlayerPrefs.GetInt(clockStyleParam, 0);
        cameraShakeToggle.isOn = (Boolean)(PlayerPrefs.GetInt(cameraShakeParam, 1) == 1);
        //defaultCamDistanceSlider.value = PlayerPrefs.GetFloat(defaultCamDistParam, 4f);
        sprintTypeDropDown.value = PlayerPrefs.GetInt(sprintTypeParam, 0);
        cheatsEnabledToggle.isOn = (Boolean)(PlayerPrefs.GetInt(cheatsEnabledParam, 0) == 1);
        autoSkipCutsceneToggle.isOn = (Boolean)(PlayerPrefs.GetInt(autoSkipCutsceneParam, 0) == 1);

        masterVolSlider.value = PlayerPrefs.GetFloat(masterVolParam, 0.5f);
        ambientVolSlider.value = PlayerPrefs.GetFloat(ambientVolParam, 0.5f);
        musicVolSlider.value = PlayerPrefs.GetFloat(musicVolParam, 0.5f);
        SFXVolSlider.value = PlayerPrefs.GetFloat(SFXVolParam, 0.5f);
        musicPopUpToggle.isOn = (Boolean)(PlayerPrefs.GetInt(musicPopUpParam, 1) == 1);

        qulityDropDown.value = PlayerPrefs.GetInt(qualtiyParam, 5);
        fpsCapDropDown.value = PlayerPrefs.GetInt(fpsCapParam, 4);
        vSyncToggle.isOn = (Boolean)(PlayerPrefs.GetInt(vSyncParam, 1) == 1);
        antiAliasingDropDown.value = PlayerPrefs.GetInt(antiAliasingParam, 0);
        motionBlurDropDown.value = PlayerPrefs.GetInt(motionBlurParam, 0);
        brightnessSlider.value = PlayerPrefs.GetFloat(brightnessParam, 1f);
        bloomSlider.value = PlayerPrefs.GetFloat(bloomParam, 0);
        showPerformanceToggle.isOn = (Boolean)(PlayerPrefs.GetInt(showPerformanceParam, 1) == 1);
    }

    public void ShowRestoreDefaultConfirmation(bool active)
    {
        restoreDefaultObject.SetActive(active);
    }

}
