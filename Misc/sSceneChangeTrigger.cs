using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sSceneChangeTrigger : MonoBehaviour
{

    [SerializeField] SceneType sceneType;
    
    [SerializeField] int villageNumber;

    [SerializeField] bool worksAtNight;

    [SerializeField] BoxCollider myCol;

    private void Update()
    {
        if (worksAtNight)
        {
            return;
        }

        myCol.isTrigger = GameManager.instance.dayCycle.isDay;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!worksAtNight && sceneType == SceneType.Wilderness && !GameManager.instance.dayCycle.isDay)
        {
            return;
        }

        if (other.transform == GameManager.instance.player)
        {
            if (sceneType == SceneType.Village)
            {
                WildernessManager.instance.CancelPotentialPassOut();
                WildernessManager.instance.SafelyReturnToVillage();
            }
            sSceneController.instance.SceneTransition(sceneType, villageNumber);
        }
    }
}
