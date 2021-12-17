using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class sMusicPopUp : MonoBehaviour
{
    [SerializeField] Transform songNamePopUp;
    
    [SerializeField] TextMeshProUGUI songNameText;

    [SerializeField] float lerpTime;
    [SerializeField] float timeToWait;
    [SerializeField] float currentT;

    Coroutine popUp;

    bool startOnEnable = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnDisable()
    {
        if (popUp != null)
        {
            startOnEnable = true;
        }    
    }

    private void OnEnable()
    {
        if (!startOnEnable) { return; }

        if (currentT > 0f && currentT < 1f)
        {
            popUp = StartCoroutine(Waiting());
        }
        else if (currentT == 0f)
        {
            popUp = StartCoroutine(PopUp());
        }
        else if (currentT == 1f)
        {
            popUp = StartCoroutine(PopDown());
        }

        startOnEnable = false;
    }

    public void NewSongPlaying(string songName)
    {
        if (!sSettings.instance.musicPopUp)
        {
            return;
        }

        currentT = 0f;
        if (popUp == null)
        {
            songNameText.text = songName;
            popUp = StartCoroutine(PopUp());
        }
        else
        {
            StopCoroutine(popUp);
            popUp = null;
            popUp = StartCoroutine(ChangeSongPopUp(songName));
        }
    }

    IEnumerator ChangeSongPopUp(string songName)
    {
        yield return StartCoroutine(PopDown());
        songNameText.text = songName;
        popUp = StartCoroutine(PopUp());
    }

    IEnumerator Waiting()
    {
        while (currentT <= 1f)
        {
            currentT += Time.deltaTime / timeToWait;
            yield return null;
        }
        currentT = 1f;
        popUp = StartCoroutine(PopDown());
    }

    IEnumerator PopUp()
    {
        float t = 0f;
        Vector3 startPos = new Vector3(0, -64, 0);
        Vector3 endPos = new Vector3(0, 64, 0);

        while (t <= 1f)
        {
            t += Time.deltaTime / lerpTime;
            songNamePopUp.localPosition = Vector3.Lerp(startPos, endPos, Lerp.Sinusoidal(t));
            yield return null;
        }
     
        songNamePopUp.localPosition = endPos;

        popUp = StartCoroutine(Waiting());
    }

    IEnumerator PopDown()
    {
        float t = 0f;
        Vector3 startPos = songNamePopUp.localPosition;
        Vector3 endPos = new Vector3(0, -64, 0);

        while (t <= 1f)
        {
            t += Time.deltaTime / lerpTime;
            songNamePopUp.localPosition = Vector3.Lerp(startPos, endPos, Lerp.Sinusoidal(t));
            yield return null;
        }

        songNamePopUp.localPosition = endPos;
        popUp = null;
    }
}
