using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildernessManager : MonoBehaviour
{
    public static WildernessManager instance;

    [SerializeField] float warningLength = 10f;

    Coroutine passingOut;

    [SerializeField] public sInventory wildernessInventory;

    string passOutMessage = "You are about to pass out, Get back to the village as soon as possible!";

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
        sDayCycle.onDailyReset += OnDailyReset;
    }


    public void OnDailyReset()
    {
        print(sSceneController.instance.currentSceneType);
        if (sSceneController.instance.currentSceneType == SceneType.Wilderness)
        {
            if (passingOut == null)
            {
                passingOut = StartCoroutine(PassOutWarning());
            }
        }
    }

    IEnumerator PassOutWarning()
    {
        float t = 0f;
        sMainUI.instance.infoPopUpMessage.PushNewInfoMessage(passOutMessage);
        do
        {
            t += Time.deltaTime / (warningLength * 0.2f);
            PostProcessEffects.instance.SetVignetteValue(Mathf.Lerp(0f, 0.3f, Lerp.SmootherStep(t)));
            yield return null;
        } while (t <= 1f);

        t = 0f;
        do
        {
            t += Time.deltaTime / (warningLength * 0.1f);
            PostProcessEffects.instance.SetVignetteValue(Mathf.Lerp(0.3f, 0.1f, Lerp.SmootherStep(t)));
            yield return null;
        } while (t <= 1f);

        t = 0f;
        do
        {
            t += Time.deltaTime / (warningLength * 0.2f);
            PostProcessEffects.instance.SetVignetteValue(Mathf.Lerp(0.1f, 0.5f, Lerp.SmootherStep(t)));
            yield return null;
        } while (t <= 1f);

        t = 0f;
        do
        {
            t += Time.deltaTime / (warningLength * 0.1f);
            PostProcessEffects.instance.SetVignetteValue(Mathf.Lerp(0.5f, 0.3f, Lerp.SmootherStep(t)));
            yield return null;
        } while (t <= 1f);

        t = 0f;
        do
        {
            t += Time.deltaTime / (warningLength * 0.2f);
            PostProcessEffects.instance.SetVignetteValue(Mathf.Lerp(0.3f, 0.8f, Lerp.SmootherStep(t)));
            yield return null;
        } while (t <= 1f);

        t = 0f;
        do
        {
            t += Time.deltaTime / (warningLength * 0.1f);
            PostProcessEffects.instance.SetVignetteValue(Mathf.Lerp(0.8f, 0.5f, Lerp.SmootherStep(t)));
            yield return null;
        } while (t <= 1f);

        t = 0f;
        do
        {
            t += Time.deltaTime / (warningLength * 0.1f);
            PostProcessEffects.instance.SetVignetteValue(Mathf.Lerp(0.5f, 1f, Lerp.SmootherStep(t)));
            yield return null;
        } while (t <= 1f);

        PassOut();
        passingOut = null;
    }

    public void CancelPotentialPassOut()
    {
        if (passingOut != null)
        {
            StopCoroutine(passingOut);
            passingOut = null;
        }
        StartCoroutine(CancelPassOut());
    }

    IEnumerator CancelPassOut()
    {
        if (passingOut != null)
        {
            StopCoroutine(passingOut);
            passingOut = null;
        }
        float t = 0f;
        float a = PostProcessEffects.instance.GetVignetteValue();
        do
        {
            t += Time.deltaTime / 1f;
            PostProcessEffects.instance.SetVignetteValue(Mathf.Lerp(a, 0f, Lerp.EaseIn(t)));
            yield return null;
        } while (t <= 1f);
    }

    void PassOut()
    {
        GameManager.instance.PlayerHasControl = false;
        wildernessInventory.RemoveAllItems();

        sSceneController.instance.PlayerPassOut();
    }

    public void SafelyReturnToVillage()
    {
        Dictionary<int, int> temp = wildernessInventory.ReturnInventory();
        foreach (var i in temp)
        {
            ResourceData tempData = new ResourceData(AssetFinder.instance.ReturnResource(i.Key), i.Value);
            VillageManager.instance.villageOneInventory.AddItems(tempData);
        }
        wildernessInventory.RemoveAllItems();
    }
}
