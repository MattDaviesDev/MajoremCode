using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class sInteractHint : MonoBehaviour
{
    [SerializeField] Transform interactHintParent;
    [SerializeField] GameObject primaryInteractMessage;
    [SerializeField] GameObject alternateInteractMessage;

    [SerializeField] public TextMeshProUGUI alternateInteractText;
    [SerializeField] public TextMeshProUGUI primaryInteractText;

    [SerializeField] Image alternateInteractTimer;

    public float AlternateInteractTimer
    {
        set
        {
            alternateInteractTimer.fillAmount = value;
        }
    }

    public bool primaryInteractAvailable = true;

    public bool alternateInteractAvailable = false;

    Coroutine hintAnim;

    bool _showHint;
    public bool ShowHint
    {
        get
        {
            return _showHint;
        }
        set
        {
            if (value == ShowHint || !interactHintParent.gameObject.activeInHierarchy)
            {
                return;
            }
            _showHint = value;
            if (hintAnim != null)
            {
                StopCoroutine(hintAnim);
                hintAnim = null;
            }
            if (value)
            {
                hintAnim = StartCoroutine(ShowHintAnim());
            }
            else
            {
                hintAnim = StartCoroutine(HideHintAnim());
            }
        }
    }

    IEnumerator ShowHintAnim()
    {
        float t = 0f;

        primaryInteractMessage.SetActive(primaryInteractAvailable);
        alternateInteractMessage.SetActive(alternateInteractAvailable);

        Vector3 startScale = interactHintParent.localScale;
        Vector3 endScale = Vector3.one * 1.2f;
        do
        {
            t += Time.deltaTime / 0.2f;
            interactHintParent.localScale = Vector3.Lerp(startScale, endScale, Lerp.SmootherStep(t));
            yield return null;
        } while (t <= 1f);
        t = 0f;
        startScale = interactHintParent.localScale;
        endScale = Vector3.one;
        do
        {
            t += Time.deltaTime / 0.1f;
            interactHintParent.localScale = Vector3.Lerp(startScale, endScale, Lerp.SmootherStep(t));
            yield return null;
        } while (t <= 1f);
        hintAnim = null;
    }

    IEnumerator HideHintAnim()
    {
        float t = 0f;
        Vector3 startScale = interactHintParent.localScale;
        Vector3 endScale = Vector3.one * 1.2f;
        do
        {
            t += Time.deltaTime / 0.2f;
            interactHintParent.localScale = Vector3.Lerp(startScale, endScale, Lerp.SmootherStep(t));
            yield return null;
        } while (t <= 1f);
        t = 0f;
        startScale = interactHintParent.localScale;
        endScale = Vector3.zero;
        do
        {
            t += Time.deltaTime / 0.1f;
            interactHintParent.localScale = Vector3.Lerp(startScale, endScale, Lerp.SmootherStep(t));
            yield return null;
        } while (t <= 1f);
        primaryInteractMessage.SetActive(primaryInteractAvailable);
        alternateInteractMessage.SetActive(alternateInteractAvailable);
        hintAnim = null;
    }

    
}
