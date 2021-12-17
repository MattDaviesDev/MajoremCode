using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class sBuildingCard : MonoBehaviour
{

    [SerializeField] public Animator anim;

    [Header("Empty info")]
    [SerializeField] TextMeshProUGUI emptyPlotSize;

    [Header("Building info")]
    [SerializeField] TextMeshProUGUI buildingPlotSize;
    [SerializeField] TextMeshProUGUI timeToFinish;
    [SerializeField] Image buildProgress;

    [Header("Occupied info")]
    [SerializeField] TextMeshProUGUI occupiedPlotSize;
    [SerializeField] TextMeshProUGUI buildingName;
    [SerializeField] TextMeshProUGUI numberOfOccupants;
    [SerializeField] sVisualHappinessEffector visualEffector;

    [Space(10)]

    public bool isBuilding;

    Date timeToComplete = new Date();
    public Date TimeToComplete
    {
        get
        {
            return timeToComplete;
        }
        set
        {
            timeToComplete = Date.GetDateFromDate(value);
        }
    }

    public float BuildProgress
    {
        set
        {
            if (value >= 1f)
            {
                value = 1f;
            }
            else if (value <= 0f)
            {
                value = 0f;
            }
            buildProgress.fillAmount = value;
        }
    }

    [HideInInspector] public sPlot myPlot;
    [HideInInspector] public sBuilding myBuilding;

    public Coroutine overridingAnimation;

    [SerializeField] GameObject emptyInfo;
    [SerializeField] GameObject buildingInfo;
    [SerializeField] GameObject occupiedInfo;


    // Start is called before the first frame update
    void Start()
    {
        emptyPlotSize.text = "Size: " + myPlot.plotSize.ToString();
        buildingPlotSize.text = "Size: " + myPlot.plotSize.ToString();
        occupiedPlotSize.text = "Size: " + myPlot.plotSize.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (myPlot.state == PlotState.Empty)
        {
            if (!emptyInfo.activeSelf)
            {
                if (overridingAnimation == null)
                {
                    overridingAnimation = StartCoroutine(OverridingAnimation(emptyInfo));
                }
            }
        }
        else if (myPlot.state == PlotState.Building)
        {
            if (!buildingInfo.activeSelf)
            {
                if (overridingAnimation == null)
                {
                    overridingAnimation = StartCoroutine(OverridingAnimation(buildingInfo));
                }
            }
            if (isBuilding)
            {
                timeToComplete.Seconds -= Time.deltaTime * GameManager.instance.dayCycle.currentTimeConversion;

                timeToFinish.text = "Completed in\n";

                timeToFinish.text += timeToComplete.ToString();
            }
            if (timeToComplete.Equals(Date.Zero()))
            {
                timeToFinish.text = "COMPLETED\nMove away from the plot!";
            }
        }
        else if (myPlot.state == PlotState.Occupied)
        {
            buildingName.text = myPlot.currentBuilding.name;
            numberOfOccupants.text = myPlot.currentBuilding.numberOfOcupants.ToString();

            visualEffector.UpdateEffectorInfo(myBuilding.currentEffector.ReturnEffectorValue());
        }
    }

    IEnumerator OverridingAnimation(GameObject screenToShow)
    {
        anim.SetBool("Closed", true);
        anim.SetBool("Open", false);

        yield return new WaitForSeconds(1f);

        emptyInfo.SetActive(screenToShow == emptyInfo);
        occupiedInfo.SetActive(screenToShow == occupiedInfo);
        buildingInfo.SetActive(screenToShow == buildingInfo);

        anim.SetBool("Closed", false);
        anim.SetBool("Open", true);

        overridingAnimation = null;
    }

    public void UpdateInfoCalledByPlot()
    {
        emptyInfo.SetActive(false);
        occupiedInfo.SetActive(true);
        buildingInfo.SetActive(false);
    }

    private void OnEnable()
    {
        sDayCycle.onTimeSkipped += TimeSkipped;
    }

    private void OnDisable()
    {
        sDayCycle.onTimeSkipped -= TimeSkipped;
    }

    void TimeSkipped()
    {
        if (timeToComplete > Date.Zero())
        {
            sDayCycle.ReduceTime(timeToComplete);
        }
    }

}
