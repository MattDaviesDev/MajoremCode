using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class sWateringCan : MonoBehaviour
{

    VisualEffect vfx;
    const string playEvent = "Play";
    const string stopEvent = "Stop";

    // Start is called before the first frame update
    void Start()
    {
        vfx = GetComponentInChildren<VisualEffect>();
    }

    public void StartWatering()
    {
        vfx.SendEvent(playEvent);
    }

    public void StopWatering()
    {
        vfx.SendEvent(stopEvent);
    }
}
