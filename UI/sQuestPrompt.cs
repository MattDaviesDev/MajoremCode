using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sQuestPrompt : MonoBehaviour
{
    [SerializeField] Transform newQuestIcon;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LerpIconScale());
    }

    public void NewQuest()
    {
        newQuestIcon.gameObject.SetActive(true);
    }

    public void OpenedQuestLog()
    {
        newQuestIcon.gameObject.SetActive(false);
    }

    private void Update()
    {
        newQuestIcon.gameObject.SetActive(QuestManager.instance.AnyUninspectedQuests());
    }

    IEnumerator LerpIconScale()
    {
        while (true)
        {
            float t = 0f;
            Vector3 startScale = Vector3.one * 0.95f;
            Vector3 endScale = Vector3.one * 1.05f;
            while (t <= 1f)
            {
                t += Time.deltaTime;
                newQuestIcon.localScale = Vector3.Lerp(startScale, endScale, Lerp.SmootherStep(t));
                yield return null;
            }
            t = 0f;
            while (t <= 1f)
            {
                t += Time.deltaTime;
                newQuestIcon.localScale = Vector3.Lerp(endScale, startScale, Lerp.SmootherStep(t));
                yield return null;
            }
        }
    }
}
