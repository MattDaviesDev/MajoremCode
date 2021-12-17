using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Date
{
    public Date() { }
    public Date(Date date)
    {
        Days = date.Days;
        Hours = date.Hours;
        Minutes = date.Minutes;
        Seconds = date.Seconds;
    }

    public static Date GetDateFromDate(Date date)
    {
        Date newData = new Date();
        newData.Days = date.Days;
        newData.Hours = date.Hours;
        newData.Minutes = date.Minutes;
        newData.Seconds = date.Seconds;
        return newData;
    }

    public int _days;
    public float _seconds;
    public int _minutes;
    public int _hours;
    public float Seconds
    {
        get
        {
            return _seconds;
        }
        set
        {
            _seconds = value;
            if (_seconds >= 60f)
            {
                _seconds -= 60f;
                Minutes++;
            }
            else if (_seconds < 0)
            {
                if (Minutes > 0 || Hours > 0 || Days > 0)
                {
                    _seconds = 60 + _seconds;
                    Minutes--;
                }
                else
                {
                    _seconds = 0;
                }
            }
        }
    }
    public int Minutes
    {
        get
        {
            return _minutes;
        }
        set
        {
            _minutes = value;
            if (_minutes >= 60)
            {
                _minutes -= 60;
                Hours++;
            }
            else if (_minutes < 0)
            {
                if (Hours > 0 || Days > 0)
                {
                    _minutes = 60 + _minutes;
                    Hours--;
                }
                else
                {
                    _minutes = 0;
                }
            }
        }
    }
    public int Hours
    {
        get
        {
            return _hours;
        }
        set
        {
            _hours = value;
            if (_hours >= 24)
            {
                _hours -= 24;
                Days++;
            }
            else if (_hours < 0)
            {
                if (Days > 0)
                {
                    _hours = 24 + _hours;
                    Days--;
                }
                else
                {
                    _hours = 0;
                }
            }
        }
    }

    public int Days
    {
        get
        {
            return _days;
        }
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            _days = value;
        }
    }

    public bool IsDay()
    {
        if (_hours < 22 && _hours >= 6)
        {
            return true;
        }
        return false;
    }

    public float TimeOfDayAsZeroToOne()
    {
        float returnVal;
        Date temp;
        temp = GetDateFromDate(GameManager.instance.dayCycle.GetCurrentDate());
        temp.Days = 0;
        int timeInSeconds = temp.TimeInSeconds();
        if (IsDay())
        {
            returnVal = timeInSeconds - ((6 * 60) * 60);
            returnVal = returnVal / ((16 * 60) * 60);
            return returnVal;
        }
        returnVal = timeInSeconds;
        if (returnVal > ((6 * 60) * 60))
        {
            returnVal -= ((22 * 60) * 60);
        }
        else
        {
            returnVal += ((2 * 60) * 60);
        }
        returnVal = returnVal / ((8 * 60) * 60);
        return returnVal;
    }

    public int TimeInSeconds()
    {
        int returnVal = (int)Seconds;
        returnVal += Minutes * 60;
        returnVal += Hours * 3600;
        returnVal += Days * 86400;
        return returnVal;
    }

    public static Date Zero()
    {
        Date date = new Date();
        date.Hours = 0;
        date.Minutes = 0;
        date.Seconds = 0;
        date.Days = 0;
        return date;
    }

    //Overrides
    public override bool Equals(object obj)
    {
        Date compareDate = obj as Date;
        if (compareDate is object)
        {
            int thisSeconds = (int)this.Seconds;
            int compareSeconds = (int)compareDate.Seconds;
            if (this.Days.Equals(compareDate.Days) && this.Hours.Equals(compareDate.Hours) && this.Minutes.Equals(compareDate.Minutes) && thisSeconds.Equals(compareSeconds))
            {
                return true;
            }
        }
        return false;
    }

    public override string ToString()
    {
        string returnStr = "";

        returnStr += Days + "d : ";
        returnStr += Date.GetIntAsTimeString(Hours) + "h : ";
        returnStr += Date.GetIntAsTimeString(Minutes) + "m : ";
        returnStr += Date.GetIntAsTimeString((int)Seconds) + "s";

        return returnStr;
    }

    public string GetTimeOfDay(bool hour24)
    {
        string returnStr = "";
        if (hour24)
        {
            returnStr = Hours + ":" + GetIntAsTimeString(Minutes);
            return returnStr;
        }

        // 12 hours clock

        int hoursText = Hours < 13 ? Hours : Hours - 12;
        returnStr += hoursText + ":" + GetIntAsTimeString(Minutes);
        if (Hours < 12)
        {
            returnStr += " AM"; 
        }
        else 
        {
            returnStr += " PM"; 
        }
        return returnStr;
    }

    /// <summary>
    /// Compares the passed in date while ignoring the day value
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool EqualsHourMinutes(object obj)
    {
        Date compareDate = obj as Date;
        compareDate.Days = this.Days;
        return this.Equals(compareDate);
    }

    public static Date operator -(Date obj, Date obj2)
    {
        Date returnDate = new Date();

        returnDate.Days = obj.Days - obj2.Days;
        returnDate.Hours = obj.Hours - obj2.Hours;
        returnDate.Minutes = obj.Minutes - obj2.Minutes;
        returnDate.Seconds = obj.Seconds - obj2.Seconds;

        return returnDate;
    }

    public static Date operator +(Date obj, Date obj2)
    {
        Date returnDate = new Date();

        returnDate.Days = obj.Days + obj2.Days;
        returnDate.Hours = obj.Hours + obj2.Hours;
        returnDate.Minutes = obj.Minutes + obj2.Minutes;
        returnDate.Seconds = obj.Seconds + obj2.Seconds;

        return returnDate;
    }

    public static Date operator *(Date obj, int obj2)
    {
        Date returnDate = new Date();

        for (int i = 0; i < obj2; i++)
        {
            returnDate += obj;
        }

        return returnDate;
    }

    public static bool operator <(Date obj, Date obj2)
    {
        bool isLower = false;
        int seconds = obj.TimeInSeconds();
        int seconds2 = obj2.TimeInSeconds();
        if (seconds < seconds2)
        {
            isLower = true;
        }

        return isLower;
    }

    public static bool operator >(Date obj, Date obj2)
    {
        return obj2 < obj;
    }

    public static Date GetDateFromSeconds(int seconds)
    {
        float d = seconds -= seconds / 86400;
        float h = seconds -= seconds / 3600;
        float m = seconds -= seconds / 60;
        float s = seconds;
        Date date = new Date();
        date.Days = (int)d;
        date.Hours = (int)h;
        date.Minutes = (int)m;
        date.Seconds = (int)s;
        return date;
    }

    public static string GetIntAsTimeString(int value)
    {
        string returnString = "";
        if (value <= 9)
        {
            returnString = "0";
        }
        returnString += value.ToString();
        return returnString;
    }

}


public class sDayCycle : MonoBehaviour
{
    public delegate void OnDailyReset();
    public static event OnDailyReset onDailyReset;

    public delegate void OnTimeSkipped();
    public static event OnTimeSkipped onTimeSkipped;

    const float daySecond = 32;
    const float nightSecond = 48;
    [SerializeField] public float dayTimeSecond = 32;
    [SerializeField] float nightTimeSecond = 48;
    public float currentTimeConversion;

    [SerializeField] public Date currentDate = new Date();
    [SerializeField] Date dailyResetTime = new Date();

    public bool isDay;

    [SerializeField] float timeRunning;

    [SerializeField] float t;

    [SerializeField] LimitValue celestialRotClamp;
    
    [SerializeField] Light sun;
    [SerializeField] Light moon;

    [Range(0, 20)] public int timeMultiplier;

    PhysicallyBasedSky sky;
    [SerializeField] Volume skyVolume;
    [SerializeField] float maxStarEmissionValue = 3553f;
    [SerializeField] AnimationCurve starVisibility;

    [Header("Passing out")]
    [SerializeField] public Date timeSkipped;
    [SerializeField] Date timeLostFromPassOut;
    [SerializeField] private float skipDuration;

    Coroutine sleeping;

    [SerializeField] public sVillageHall defaultVillageHall;

    // Start is called before the first frame update
    void Start()
    {
        skyVolume.profile.TryGet(out sky);

        currentDate = new Date(SaveSystem.instance.gameSave.date);
    }

    bool freezeTimeUpdate = false;

    // Update is called once per frame
    void Update()
    {
        isDay = currentDate.IsDay();
        currentTimeConversion = isDay ? daySecond : nightSecond;

        currentTimeConversion *= timeMultiplier;
        dayTimeSecond = daySecond * timeMultiplier;
        nightTimeSecond = nightSecond * timeMultiplier;

        if (!freezeTimeUpdate)
        {
            currentDate.Seconds += Time.deltaTime * currentTimeConversion;
        }

        timeRunning += Time.deltaTime;

        if (currentDate > dailyResetTime)
        {
            if (sSceneController.instance.currentSceneType == SceneType.Village)
            {
                if (sSettings.instance.autoSleep)
                {
                    Sleep(defaultVillageHall);
                }
            }
        }


        t = currentDate.TimeOfDayAsZeroToOne();
        t = Mathf.Clamp01(t);

        sun.shadows = isDay ? LightShadows.Soft : LightShadows.None;
        moon.shadows = !isDay ? LightShadows.Soft : LightShadows.None;

        float sunRot;
        sunRot = Mathf.Lerp(celestialRotClamp.max, celestialRotClamp.min, Lerp.Sinusoidal(t));

        if (!isDay)
        {
            sky.spaceEmissionMultiplier.value = maxStarEmissionValue * starVisibility.Evaluate(t);
            sunRot -= 180;

            float postExposure = Mathf.Lerp(PostProcessEffects.instance.GetColorAdjustmentPostExposure(), -2.5f, 0.00075f);
            PostProcessEffects.instance.SetColorAdjustmentPostExposure(postExposure);
        }
        else
        {
            sky.spaceEmissionMultiplier.value = 0f;

            float postExposure = Mathf.Lerp(PostProcessEffects.instance.GetColorAdjustmentPostExposure(), 0f, 0.001f);
            PostProcessEffects.instance.SetColorAdjustmentPostExposure(postExposure);
        }
        
        float moonRot = sunRot - 180;

        sun.transform.rotation = Quaternion.Euler(sunRot, -33f, 0f);
        moon.transform.rotation = Quaternion.Euler(moonRot, -33f, 0f);
        

        if (currentDate > dailyResetTime)
        {
            dailyResetTime.Days++; //prepare for the next day
            onDailyReset?.Invoke();
        }

    }

    public void SaveDate()
    {
        SaveSystem.instance.gameSave.date = new Date(currentDate);
    }

    public bool CheckTime(Date _time)
    {
        if (currentDate.Equals(_time))
        {
            return true;
        }
        return false;
    }

    public int SubtractDateFromCurrentDate(Date _date)
    {
        int a = currentDate.TimeInSeconds();
        int b = _date.TimeInSeconds();

        return a - b;
    }

    public Date GetCurrentDate()
    {
        Date _currentDate = new Date();
        _currentDate.Seconds = currentDate.Seconds;
        _currentDate.Minutes = currentDate.Minutes;
        _currentDate.Hours = currentDate.Hours;
        _currentDate.Days = currentDate.Days;

        return _currentDate;
    }

    public void Sleep(sVillageHall currentVillageHall)
    {
        if (sleeping == null)
        {
            sleeping = StartCoroutine(SleepRoutine(currentVillageHall));
        }
    }

    public void SkipTime(Date _timeSkipped)
    {
        timeSkipped = _timeSkipped;
        
        onTimeSkipped?.Invoke();
    }

    public static void ReduceTime(in Date date)
    {
        date.Equals(date - GameManager.instance.dayCycle.timeSkipped);
    }

    IEnumerator SleepRoutine(sVillageHall currentVillageHall)
    {
        string sleepOn = "\"Auto Sleep\" is currently enabled. Turn it off in the 'Gameplay' settings.";
        string sleepOff = "\"Auto Sleep\" is currently disabled. Turn it on in the 'Gameplay' settings.";

        string temp = sSettings.instance.autoSleep ? sleepOn : sleepOff;

        sMainUI.instance.sleepInfoText.text = temp;

        GameManager.instance.PlayerHasControl = false;

        GameManager.instance.hasCameraControl = false; 
        GameManager.instance.playerMovement.enabled = false;
        GameManager.instance.playerMovement.cc.enabled = false;
        
        float t = 0f;
        do
        {
            t += Time.deltaTime / 2f;
            sMainUI.instance.sleepCanvas.alpha = Mathf.Lerp(0f, 1f, Lerp.Sinusoidal(t));
            sMainUI.instance.sleepInfoCanvas.alpha = Mathf.Lerp(0f, 1f, Lerp.Sinusoidal(t));
            yield return null;
        } while (t <= 1f);

        sMainUI.instance.sleepCanvas.alpha = 1f;
        sMainUI.instance.sleepInfoCanvas.alpha = 1f;

        GameManager.instance.player.position = currentVillageHall.playerSleepingTransform.position;
        GameManager.instance.player.rotation = currentVillageHall.playerSleepingTransform.rotation;

        GameManager.instance.gameCamera.Priority = 0;
        currentVillageHall.sleepVCam.Priority = 10;
        

        t = 0f;
        do
        {
            t += Time.deltaTime / 1f;
            sMainUI.instance.sleepCanvas.alpha = Mathf.Lerp(1f, 0.15f, Lerp.Exponential(t));
            yield return null;
        } while (t <= 1f);
        sMainUI.instance.sleepCanvas.alpha = 0f;

        Date newDate = new Date(currentDate);
        if (newDate.Hours > 21)
        {
            newDate.Days++;
        }
        newDate.Seconds = 0;
        newDate.Minutes = 0;
        newDate.Hours = 6;
        Date _timeSkipped = new Date(newDate - currentDate);
        float minutesToDay = (_timeSkipped._days * 24 * 60) + _timeSkipped._minutes + (_timeSkipped._seconds / 60) + (_timeSkipped._hours * 60);
        float skipStep = skipDuration / (minutesToDay * 60);
        float minutesToSkip = 1; 
        if (Mathf.Max(skipStep, 0.05f) != skipStep)
        {
            minutesToSkip = minutesToDay/ Mathf.CeilToInt(skipDuration/0.05f);
        }

        freezeTimeUpdate = true;

        currentDate.Seconds = 0;
        do
        {
            print(Mathf.CeilToInt(minutesToSkip));
            currentDate.Minutes += Mathf.CeilToInt(minutesToSkip);
            //currentDate.Minutes++;
//Time.deltaTime was often larger than the wait time which lead to spending a long time just looking at the town hall. Now should always take around skipDuration to complete as long as FPS is above 20
            yield return new WaitForSeconds(Mathf.Max(skipStep, 0.05f));
        } while (!GameManager.instance.dayCycle.isDay);

        freezeTimeUpdate = true;

        t = 0f;
        do
        {
            t += Time.deltaTime / 1f;
            sMainUI.instance.sleepCanvas.alpha = Mathf.Lerp(0.15f, 1f, Lerp.Sinusoidal(t));
            yield return null;
        } while (t <= 1f);

        sMainUI.instance.sleepCanvas.alpha = 1f;

        SkipTime(_timeSkipped);

        GameManager.instance.player.position = currentVillageHall.playerWakeUpTransform.position;
        GameManager.instance.player.rotation = currentVillageHall.playerWakeUpTransform.rotation;

        GameManager.instance.hasCameraControl = true;
        GameManager.instance.gameCamera.Priority = 10;
        currentVillageHall.sleepVCam.Priority = 0;
        GameManager.instance.playerMovement.cc.enabled = true;
        GameManager.instance.playerMovement.enabled = true;

        freezeTimeUpdate = false;

        t = 0f;
        do
        {
            t += Time.deltaTime / 1f;
            sMainUI.instance.sleepCanvas.alpha = Mathf.Lerp(1f, 0f, Lerp.Exponential(t));
            sMainUI.instance.sleepInfoCanvas.alpha = Mathf.Lerp(1f, 0f, Lerp.Sinusoidal(t));
            yield return null;
        } while (t <= 1f);

        sMainUI.instance.sleepCanvas.alpha = 0f;
        sMainUI.instance.sleepInfoCanvas.alpha = 0f;

        GameManager.instance.PlayerHasControl = true;

        sleeping = null;

    }

}
