using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class sPausedUI : MonoBehaviour
{
    public static sPausedUI instance;

    GameObject previousMenu;
    GameObject currentMenu;

    [Header("Menu Objects")]
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject quitConfirmation;

    [Header("Pause Objects")]
    [SerializeField] Image blackBackground;
    [SerializeField] RectTransform pausePanel;

    [Header("Quit Confirmation")]
    [SerializeField] TextMeshProUGUI quitTitle;
    [SerializeField] TextMeshProUGUI quitDescription;
    bool desktopQuit = false;

    Coroutine animatingCanvas;
    [SerializeField] public CanvasGroup pauseCanvas;

    [SerializeField] GameObject resumeButton;

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
    }

    private void OnEnable()
    {
        sInputManager.instance.closeWindowAction.performed += CloseWindow;
    }

    private void OnDisable()
    {
        sInputManager.instance.closeWindowAction.performed -= CloseWindow;
    }

    void CloseWindow(InputAction.CallbackContext input)
    {
        if (previousMenu != null)
        {
            previousMenu.SetActive(true);
            currentMenu.SetActive(false);
            previousMenu = null;
        }
        else
        {
            ResumeButtonPressed();
        }
    }

    public void ShowPauseCanvas()
    {
        if (animatingCanvas == null)
        {
            pauseMenu.SetActive(true);
            animatingCanvas = StartCoroutine(PauseCanvasAnim(true, false));
            currentMenu = pauseMenu;
            if (sMainUI.instance.PlayerIsUsingGamepad())
            {
                sMainUI.instance.SetCurrentSelectedObjectForGamepad(resumeButton);
            }
        }
    }

    public void ResumeButtonPressed()
    {
        if (animatingCanvas == null)
        {
            animatingCanvas = StartCoroutine(PauseCanvasAnim(false, false));
        }
    }

    void Resume()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        sMainUI.instance.ResumeGame();
        // show main UI
    }

    public void UnStuckButtonPressed()
    {
        sSceneController.instance.SceneTransition(sSceneController.instance.currentSceneType, 1);
        Resume();
    }

    public void DecorationEditorButtonPressed()
    {
        if (animatingCanvas == null)
        {
            animatingCanvas = StartCoroutine(PauseCanvasAnim(false, true));
        }
        // show main UI
    }

    IEnumerator PauseCanvasAnim(bool showCanvas, bool showDecorationEditor)
    {
        float t = showCanvas ? 0f: 1f;
        Vector2 startPos = new Vector2(-pausePanel.sizeDelta.x, pausePanel.position.y);
        Vector2 endPos = new Vector2(0, pausePanel.position.y);

        Color startColor = new Color(blackBackground.color.r, blackBackground.color.g, blackBackground.color.b, 0f);
        Color endColor = new Color(blackBackground.color.r, blackBackground.color.g, blackBackground.color.b, 0.7f);
        
        bool breakCase = false;

        if (showCanvas)
        {
            pauseCanvas.blocksRaycasts = true;
        }

        if (!showCanvas)
        {
            pauseCanvas.blocksRaycasts = false;
        }

        do
        {
            if (showCanvas)
            {
                t += Time.unscaledDeltaTime / 0.3f;
            }
            else
            {
                t -= Time.unscaledDeltaTime / 0.3f;
            }

            float t2 = t / 0.3f;
            t2 = Mathf.Clamp01(t2);

            pausePanel.position = Vector2.Lerp(startPos, endPos, Lerp.Sinusoidal(t));
            blackBackground.color = Color.Lerp(startColor, endColor, Lerp.Sinusoidal(t2));

            yield return null;

            if ((showCanvas && t >= 1f) || (!showCanvas && t <= 0f))
            {
                breakCase = true;
            }
        } while (!breakCase);

        t = Mathf.Clamp01(t);

        pausePanel.position = Vector2.Lerp(startPos, endPos, t);
        blackBackground.color = Color.Lerp(startColor, endColor, t);

        if (!showCanvas && !showDecorationEditor)
        {
            Resume();
        }
        else if (showDecorationEditor)
        {
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
            sMainUI.instance.ShowCursor = true;
            sDecorationEditor.instance.ShowDecorationEditor(false);
            sInputManager.instance.MoveToDecorationEditorMap();
        }

        animatingCanvas = null;
    }

    public void SettingsButtonPressed()
    {
        pauseMenu.SetActive(false);
        previousMenu = pauseMenu;
        currentMenu = settingsMenu;
        settingsMenu.SetActive(true);
    }

    public void QuitButtonPressed(bool _desktopQuit)
    {
        string quitMessage = "Are you sure you would like to save and quit to ";
        string _quitTitle = "Quit to ";
        if (_desktopQuit)
        {
            quitMessage += "Desktop?";
            _quitTitle += "Desktop";
        }
        else
        {
            quitMessage += "Menu?";
            _quitTitle += "Menu";
        }
        quitDescription.text = quitMessage;
        quitTitle.text = _quitTitle;
        desktopQuit = _desktopQuit;
        quitConfirmation.SetActive(true);
    }

    public void QuitConfirmation(bool confirm)
    {
        if (confirm)
        {
            // Save all data
            WildernessManager.instance.SafelyReturnToVillage();
            VillageManager.instance.villageOneInventory.SaveInventory();

            GameManager.instance.dayCycle.SaveDate();



            if (desktopQuit)
            {
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #endif  
                Application.Quit();
            }
            else
            {
                // Switch scene to main menu
                AudioManager.instance.StopPlayingCurrentTrack();
                SceneSwitcher.instance.ChangeScene(pauseCanvas, SceneIndexes.MainMenu);
            }
        }
        else
        {
            quitConfirmation.SetActive(false);
        }
    }


}
