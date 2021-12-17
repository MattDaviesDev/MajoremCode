using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class sVillageHall : MonoBehaviour
{

    public Transform sleepCameraTransfrom;

    public Transform playerSleepingTransform;

    public Transform playerWakeUpTransform;

    public CinemachineVirtualCamera sleepVCam;

    private void Start()
    {
        GameManager.instance.dayCycle.defaultVillageHall = this;
    }

}
