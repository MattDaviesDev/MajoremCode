using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class sMainMenu : MonoBehaviour
{

    public enum MenuType
    {
        MainMenu, Load, Settings, BugReport
    }

    public static sMainMenu instance;
    
    MenuType currentMenuType = MenuType.MainMenu;

    [SerializeField] sMainMenuCameraController menuCam;

    [SerializeField] float lerpTime;

    [Header("Main menu objects")]
    [SerializeField] Transform mainMenuCamPos;
    [SerializeField] public CanvasGroup mainMenuCanvas;

    [Header("Settings objects")]
    [SerializeField] CanvasGroup settingsCanvas;
    [SerializeField] Transform settingsCamPos;

    [Header("Load objects")]
    [SerializeField] CanvasGroup loadCanvas;
    [SerializeField] Transform loadCamPos;

    [Header("Splash Screen")]
    [SerializeField] CanvasGroup splashScreenCanvas;
    [SerializeField] sSplashScreen splashScreen;

    [Header("Report a bug")]
    [SerializeField] CanvasGroup bugReportCanvas;
    [SerializeField] Transform bugReportCamPos;

    Coroutine swapping;

    [Header("Audio tracks")]
    [SerializeField] AudioSource mainMenuTrack;

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

    // Start is called before the first frame update
    void Start()
    {
        if (SceneSwitcher.instance.justOpened)
        {
            splashScreen.Splash();
        }
        else
        {
            splashScreen.HideSplash();
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.shouldPlay = true;
        }
    }

    private void OnEnable()
    {
        sInputManager.instance.closeWindowAction.performed += CloseWindowAction;
    }

    private void OnDisable()
    {
        sInputManager.instance.closeWindowAction.performed -= CloseWindowAction;
    }

    public void PlayGameButtonPressed()
    {
        mainMenuCanvas.interactable = false;
        StartCoroutine(FadeMusicOut());
        SceneSwitcher.instance.ChangeScene(mainMenuCanvas, SceneIndexes.Gameplay);
    }

    IEnumerator FadeMusicOut()
    {
        float t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime / 0.4f;
            mainMenuTrack.volume = Mathf.Lerp(1f, 0f, Lerp.EaseIn(t));
            yield return null;
        }
    }

    public void LoadButtonPressed()
    {
        menuCam.MoveCameraToPosition(loadCamPos, lerpTime);
        SwapCanvases(mainMenuCanvas, loadCanvas, lerpTime, MenuType.Load);
    }

    public void SettingsButtonPressed()
    {
        menuCam.MoveCameraToPosition(settingsCamPos, lerpTime);
        SwapCanvases(mainMenuCanvas, settingsCanvas, lerpTime, MenuType.Settings);
    }

    public void ReportBugPressed()
    {
        menuCam.MoveCameraToPosition(bugReportCamPos, lerpTime);
        SwapCanvases(mainMenuCanvas, bugReportCanvas, lerpTime, MenuType.BugReport);
    }

    public void QuitButtonPressed()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    public void SwapCanvases(CanvasGroup a, CanvasGroup b, float totalLerpTime, MenuType newMenuType)
    {
        if (swapping == null)
        {
            currentMenuType = newMenuType;
            swapping = StartCoroutine(SwapCanvasesRoutine(a, b, totalLerpTime));
        }
    }

    IEnumerator SwapCanvasesRoutine(CanvasGroup a, CanvasGroup b, float totalLerpTime)
    {
        float t = 0f;
        a.interactable = false;
        a.blocksRaycasts = false;
        do
        {
            t += Time.deltaTime / (totalLerpTime * 0.5f);
            a.alpha = Mathf.Lerp(1f, 0f, Lerp.Sinusoidal(t));
            yield return null;
        } while (t <= 1f);
        a.alpha = 0f;
        t = 0f;
        b.interactable = true;
        b.blocksRaycasts = true;
        do
        {
            t += Time.deltaTime / (totalLerpTime * 0.5f);
            b.alpha = Mathf.Lerp(0f, 1f, Lerp.Sinusoidal(t));
            yield return null;
        } while (t <= 1f);
        b.alpha = 1f;
        swapping = null;
    }

    public void CloseWindowAction(InputAction.CallbackContext input)
    {
        CloseWindow();
    }

    public void CloseWindow()
    {
        switch (currentMenuType)
        {
            case MenuType.MainMenu:
                break;
            case MenuType.Load:
                menuCam.MoveCameraToPosition(mainMenuCamPos, lerpTime);
                SwapCanvases(loadCanvas, mainMenuCanvas, lerpTime, MenuType.MainMenu);
                break;
            case MenuType.Settings:
                menuCam.MoveCameraToPosition(mainMenuCamPos, lerpTime);
                SwapCanvases(settingsCanvas, mainMenuCanvas, lerpTime, MenuType.MainMenu);
                break;
            case MenuType.BugReport:
                menuCam.MoveCameraToPosition(mainMenuCamPos, lerpTime);
                SwapCanvases(bugReportCanvas, mainMenuCanvas, lerpTime, MenuType.MainMenu);
                break;
        }
    }
}
