using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class sResourceHitPoints : MonoBehaviour
{
    Camera mainCam;

    [SerializeField] RectTransform hitPointsPosition;

    [SerializeField] TextMeshProUGUI resourceNameText;

    [SerializeField] TextMeshProUGUI hitPointsText;

    [SerializeField] Image catchUpBG;
    [SerializeField] Image progressBar;

    public IHarvestable selectedNode;

    public bool shown = false;

    Coroutine showing;

    Transform currentHealthBarPoint;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedNode != null)
        {
            progressBar.fillAmount = selectedNode.GetNode().GetCurrentResourceProgres();
            hitPointsText.text = selectedNode.GetNode().GetCurrentResourceCount() + "/" + selectedNode.GetNode().maxResources;
            if (Mathf.Abs(catchUpBG.fillAmount - progressBar.fillAmount) > 0.02f)
            {
                catchUpBG.fillAmount = Mathf.Lerp(catchUpBG.fillAmount, progressBar.fillAmount, 0.1f);
            }
            else
            {
                catchUpBG.fillAmount = progressBar.fillAmount;
            }
        }
    }

    private void LateUpdate()
    {
        if (selectedNode != null)
        {
            Vector3 temp = mainCam.WorldToScreenPoint(currentHealthBarPoint.position);

            if (temp.z < 0f)
            {
                hitPointsPosition.gameObject.SetActive(false);
            }
            else
            {
                hitPointsPosition.gameObject.SetActive(true);
            }

            temp.z = 0f;
            hitPointsPosition.position = temp;
        }
    }

    public void ShowHealthBar(IHarvestable node)
    {
        if (showing == null)
        {
            selectedNode = node;
            sResourceNode temp = node.GetNode();
            currentHealthBarPoint = temp.hitPointsPosition;
            resourceNameText.text = temp.nodeData.nodeName;
            hitPointsText.text = temp.GetCurrentResourceCount() + "/" + temp.maxResources;
            catchUpBG.fillAmount = temp.GetCurrentResourceProgres();
            progressBar.fillAmount = temp.GetCurrentResourceProgres();
            showing = StartCoroutine(HealthBarAnim(true));
            shown = true;
        }
    }

    public void HideHealthBar()
    {
        if (showing == null)
        {
            shown = false;
            selectedNode = null;
            showing = StartCoroutine(HealthBarAnim(false));
        }
    }

    IEnumerator HealthBarAnim(bool show)
    {
        float t = show ? 0f : 1f;

        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;

        bool breakCase = false;

        do
        {
            if (show)
            {
                t += Time.deltaTime / 0.1f;
            }
            else
            {
                t -= Time.deltaTime / 0.1f;
            }

            hitPointsPosition.localScale = Vector3.Lerp(startScale, endScale, Lerp.Sinusoidal(t));
            yield return null;

            if ((show && t >= 1f) || (!show && t <= 0f))
            {
                breakCase = true;
            }

        } while (!breakCase);

        showing = null;
    }
}
