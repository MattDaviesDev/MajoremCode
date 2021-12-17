using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sCritSpot : MonoBehaviour
{
    public bool shown = false;

    [SerializeField] Vector3 currentWorldPosition;

    [SerializeField] float critLeniency;
    [SerializeField] RectTransform critMarker;

    Camera mainCam;

    Coroutine showing;

    [SerializeField] bool updatePosition = false;

    Vector2 middleOfScreenPos;

    public bool isCrit = false;

    [SerializeField] Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        middleOfScreenPos = new Vector2(Screen.currentResolution.width * 0.5f, Screen.currentResolution.height * 0.5f);
    }


    // Update is called once per frame
    void LateUpdate()
    {
        if (updatePosition)
        {
            Vector3 temp = mainCam.WorldToScreenPoint(currentWorldPosition);

            if (temp.z < 0f)
            {
                critMarker.gameObject.SetActive(false);
            }
            else
            {
                critMarker.gameObject.SetActive(true);
            }

            temp.z = 0f;
            
            critMarker.position = temp;
        }

        isCrit = IsCrit();
    }

    public void ShowCrit(Vector3 newWorldPos)
    {
        updatePosition = true;

        currentWorldPosition = newWorldPos;
        if (showing != null)
        {
            StopCoroutine(showing);
            showing = null;
        }

        showing = StartCoroutine(ShowCritMarkerAnim(true));
    }

    public void HideCrit()
    {
        if (showing != null)
        {
            StopCoroutine(showing);
            showing = null;
        }

        showing = StartCoroutine(ShowCritMarkerAnim(false));
    }

    public bool IsCrit()
    {
        if (Vector2.Distance(critMarker.position, middleOfScreenPos) <= critLeniency)
        {
            return true;
        }
        return false;
    }

    IEnumerator ShowCritMarkerAnim(bool show)
    {
        float t = show ? 0f : 1f;
        bool breakCase = false;
        if (show)
        {
            shown = true;  
        }
        do
        {
            t += Time.deltaTime / 0.1f;
            critMarker.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, Lerp.SmoothStep(t));

            yield return null;

            if ((show && t >= 1f) || (!show && t <= 0f))
            {
                breakCase = true;    
            }

        } while (!breakCase);

        if (!show)
        {
            shown = false;
        }

        showing = null;
    }
}
