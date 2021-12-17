using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class sDateDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dateDisplay;

    public bool clockIs24Hour = true;

    [SerializeField] RectTransform sunRect;
    [SerializeField] AnimationCurve sunMovementCurve;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Date currentDate = GameManager.instance.dayCycle.GetCurrentDate();

        dateDisplay.text = "Day " + (currentDate.Days + 1) + "\n";

        dateDisplay.text += currentDate.GetTimeOfDay(sSettings.instance.clockStyle == 0);

        float t = SunLerpTime(currentDate);

        sunRect.localPosition = new Vector3(sunRect.localPosition.x, Mathf.Lerp(-109.4f, 108.8f, sunMovementCurve.Evaluate(t)), 0f);

    }

    float SunLerpTime(Date currentTime)
    {
        if (currentTime.IsDay())
        {
            int tempHours = (currentTime.Hours - 6) * 3600;
            int tempMins = currentTime.Minutes * 60;
            int tempSecs = (int)currentTime.Seconds;
            return (float)(tempHours + tempMins + tempSecs) / (float)57600;
        }
        return 0f;
    }
}
