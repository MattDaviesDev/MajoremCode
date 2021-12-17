using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class sInfoPopUpMessage : MonoBehaviour
{

    [SerializeField] float lerpUpTime = 0.5f;
    [SerializeField] float waitTime = 0.5f;
    [SerializeField] float lerpDownTime = 0.5f;

    Transform parent;
    TextMeshProUGUI infoText;

    Coroutine notification;

    // Start is called before the first frame update
    void Start()
    {
        parent = transform;
        infoText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void PushNewInfoMessage(string msg)
    {
        if (notification == null)
        {
            infoText.text = msg;
            notification = StartCoroutine(NotificationAnim());
        }
        else
        {
            StopCoroutine(notification);
            notification = null;
            parent.localScale = Vector3.zero;
            PushNewInfoMessage(msg);
        }
    }

    IEnumerator NotificationAnim()
    {
        float t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime / lerpUpTime;
            parent.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, Lerp.EaseIn(t));
            yield return null;
        }
        parent.localScale = Vector3.one;

        t = 0f;
        yield return new WaitForSeconds(waitTime);

        while (t <= 1f)
        {
            t += Time.deltaTime / lerpDownTime;
            parent.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, Lerp.EaseIn(t));
            yield return null;
        }
        parent.localScale = Vector3.zero;
        notification = null;
    }
}
