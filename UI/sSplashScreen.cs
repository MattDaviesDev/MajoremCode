using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sSplashScreen : MonoBehaviour
{

    [SerializeField] CanvasGroup splashScreenCanvas;
    [SerializeField] CanvasGroup[] logos;

    [SerializeField] float fadeUpTime = 1f;
    [SerializeField] float timeOnScreen = 1f;
    [SerializeField] float fadeDownTime = 1f;

    Coroutine splashing;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Splash()
    {
        if (splashing == null)
        {
            splashScreenCanvas.alpha = 1f;
            sMainMenu.instance.mainMenuCanvas.blocksRaycasts = false;
            sMainMenu.instance.mainMenuCanvas.interactable = false;
            sMainMenu.instance.mainMenuCanvas.alpha = 0f;
            splashing = StartCoroutine(SplashScreenRoutine());
        }
    }

    public void HideSplash()
    {
        sMainMenu.instance.mainMenuCanvas.interactable = true;
        sMainMenu.instance.mainMenuCanvas.blocksRaycasts = true;
        splashScreenCanvas.alpha = 0f;
    }

    IEnumerator SplashScreenRoutine()
    {
        WaitForSeconds waitTime = new WaitForSeconds(timeOnScreen);
        float t;
        float startA;
        float endA;
        for (int i = 0; i < logos.Length; i++)
        {
            t = 0f;
            startA = 0f;
            endA = 1f;
            while (t <= 1f)
            {
                t += Time.deltaTime / fadeUpTime;
                logos[i].alpha = Mathf.Lerp(startA, endA, Lerp.Sinusoidal(t));
                yield return null;
            }
            yield return waitTime;
            t = 0f;
            startA = 1f;
            endA = 0f;
            while (t <= 1f)
            {
                t += Time.deltaTime / fadeUpTime;
                logos[i].alpha = Mathf.Lerp(startA, endA, Lerp.Sinusoidal(t));
                yield return null;
            }
        }

        t = 0f;
        startA = 1f;
        endA = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime / fadeDownTime;
            splashScreenCanvas.alpha = Mathf.Lerp(startA, endA, Lerp.Sinusoidal(t));
            yield return null;
        }

        sMainMenu.instance.mainMenuCanvas.blocksRaycasts = true;
        sMainMenu.instance.mainMenuCanvas.interactable = true;

        SceneSwitcher.instance.CompleteSceneChange(sMainMenu.instance.mainMenuCanvas, true);
        //t = 0f;
        //startA = 0f;
        //endA = 1f;
        //while (t <= 1f)
        //{
        //    t += Time.deltaTime / fadeUpTime;
        //    sMainMenu.instance.mainMenuCanvas.alpha = Mathf.Lerp(startA, endA, Lerp.Sinusoidal(t));
        //    yield return null;
        //}
        splashing = null;

    }
}
