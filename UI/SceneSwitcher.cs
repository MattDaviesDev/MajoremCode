using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneIndexes
{
    MainMenu = 0, Gameplay = 1
}

public class SceneSwitcher : MonoBehaviour
{
    public static SceneSwitcher instance;

    [SerializeField] CanvasGroup switchingCanvas;
    [SerializeField] TextMeshProUGUI loadingText;

    Coroutine fakeLoading;

    public Coroutine switchingScene;

    public bool justOpened = true;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ChangeScene(CanvasGroup fadeOutCanvas, SceneIndexes sceneIndex)
    {
        if (switchingScene == null)
        {
            justOpened = false;
            switchingScene = StartCoroutine(ChangeSceneRoutine(fadeOutCanvas, sceneIndex));
        }
    }

    IEnumerator ChangeSceneRoutine(CanvasGroup fadeOutCanavs, SceneIndexes sceneIndex)
    {
        float t = 0f;
        if (fadeOutCanavs != null)
        {
            do
            {
                t += Time.unscaledDeltaTime / 0.3f;
                fadeOutCanavs.alpha = Mathf.Lerp(1f, 0f, Lerp.SmootherStep(t));
                yield return null;
            } while (t <= 1f);
        }
        
        if (fakeLoading == null)
        {
            fakeLoading = StartCoroutine(LoadingIndicator());
        }

        t = 0f;
        do
        {
            t += Time.unscaledDeltaTime / 0.3f;
            switchingCanvas.alpha = Mathf.Lerp(0f, 1f, Lerp.SmootherStep(t));
            yield return null;
        } while (t <= 1f);

        Time.timeScale = 1f;

        AsyncOperation async = SceneManager.LoadSceneAsync((int)sceneIndex);
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            if (async.progress >= 0.9f)
            {
                async.allowSceneActivation = true;
            }
            yield return null;
        }

    }

    IEnumerator LoadingIndicator()
    {
        const string loadingStr = "Loading the game";
        string ellipses = "";

        WaitForSeconds waitTime = new WaitForSeconds(0.5f);

        while (true)
        {
            ellipses = "";
            for (int i = 0; i <= 3; i++)
            {
                loadingText.text = loadingStr + ellipses;
                yield return waitTime;
                ellipses += ".";
            }
        }
    }

    public void CompleteSceneChange(CanvasGroup fadeInCanvas, bool skipWait)
    {
        fadeInCanvas.alpha = 0f;
        switchingScene = StartCoroutine(CompleteSceneTransition(fadeInCanvas, skipWait));
    }

    IEnumerator CompleteSceneTransition(CanvasGroup fadeInCanvas, bool skipWait)
    {
        if (!skipWait)
        {
            yield return new WaitForSeconds(0.5f);
        }
        
        float t = 0f;
        if (!justOpened)
        {
            do
            {
                t += Time.unscaledDeltaTime / 0.3f;
                switchingCanvas.alpha = Mathf.Lerp(1f, 0f, Lerp.SmootherStep(t));
                yield return null;
            } while (t <= 1f);
        } 

        t = 0f;
        do
        {
            t += Time.unscaledDeltaTime / 0.3f;
            fadeInCanvas.alpha = Mathf.Lerp(0f, 1f, Lerp.SmootherStep(t));
            yield return null;
        } while (t <= 1f);

        if (fakeLoading != null)
        {
            StopCoroutine(fakeLoading);
            fakeLoading = null;
        }


        switchingScene = null;
    }
}
