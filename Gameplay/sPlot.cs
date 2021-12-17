using System.Collections;
using System.Collections.Generic;
using AI.Blackboard;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

public enum PlotSize
{
    Small = 8, Medium = 12, Large = 16
}

public enum PlotState
{
    Empty, Building, Occupied
}


[RequireComponent(typeof(sLineBorder))]
[RequireComponent(typeof(BoxCollider))]
public class sPlot : MonoBehaviour, IInteractableObject
{

    //[Header("Interaction")]
    string interactActionWord = "Press for plot options";
    string alternateInteractActionWord;
    
    [Header("Border")]
    [SerializeField] float activeBorderDistance;
    [SerializeField, Range(0f, 2f)] float borderHeight;
    [SerializeField, Range(0f, 10f)] float borderFadeUpTime;

    [Header("Plot")]
    public int plotID;
    [SerializeField] public PlotSize plotSize;
    BuildingType currentBuildingType;
    [SerializeField] public Building currentBuilding;
    [SerializeField] GameObject currentBuildingObject;
    public PlotState state;

    [Space(30)]
    [SerializeField] bool updatePlot = false;

    sLineBorder lineBorder;

    sLineBorder LineBorder
    {
        get
        {
            if (lineBorder == null)
            {
                lineBorder = GetComponent<sLineBorder>();
            }
            return lineBorder;
        }
    }

    BoxCollider boxCollider;

    BoxCollider Col
    {
        get
        {
            if (boxCollider == null)
            {
                boxCollider = GetComponent<BoxCollider>();
            }
            return boxCollider;
        }
    }

    Coroutine pulsing;

    public bool finishedBuilding;

    [SerializeField] bool preassignedPlot;
    [SerializeField] bool lockedPlot;

    [Header("Building")]
    [SerializeField] float buildVFXMaxEmission;
    VisualEffect buildVFX;

    sBuildingCard card;

    sBuildingCard Card
    {
        get 
        {
            if (card == null)
            {
                card = Instantiate(AssetFinder.instance.buildingCard, transform).GetComponent<sBuildingCard>();
                card.transform.rotation = transform.rotation * Quaternion.Euler(0, 180, 0);
                card.transform.parent = transform;
                card.myPlot = this;
            }

            if (currentBuildingObject != null && card.myBuilding == null)
            {
                card.myBuilding = currentBuildingObject.GetComponent<sBuilding>();
            }

            return card;
        }
    }

    Coroutine buildingAnimation;
    Coroutine waiting;

    Collider plotCollider;

    // Start is called before the first frame update
    void Start()
    {
        LineBorder.GenerateSquareBorder();
        LineBorder.SpawnVFX((int)plotSize, (int)plotSize * 5);

        // spawn in Building VFX to be used when the plot is building
        buildVFX = Instantiate(AssetFinder.instance.plotBuildingVFX, transform).GetComponent<VisualEffect>();
        float ySize = 3f;
        float xzSize = ((int)plotSize * 0.5f) - 1;
        buildVFX.SetFloat("EmissionRate", 0);
        buildVFX.SetVector3("Size", new Vector3(xzSize, ySize, xzSize));
        buildVFX.SetFloat("yCenter", ySize * 0.5f);
        buildVFX.transform.parent = transform; // cleaning up Hierarchy
        buildVFX.transform.position = transform.position;
        buildVFX.transform.rotation = transform.rotation;

        if (SaveSystem.instance.gameSave.buildings[plotID] != null)
        {
            print("Plot " + plotID + " is occupied by " + SaveSystem.instance.gameSave.buildings[plotID].name);
            currentBuilding = SaveSystem.instance.gameSave.buildings[plotID];
            preassignedPlot = true;
        }

        if (preassignedPlot)
        {
            PreAssignedPlotStart();
        }
        else
        {
            Card.anim.SetBool("Open", false);
        }

        plotCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    { 
        if (currentBuilding != null)
        {
            currentBuildingType = currentBuilding.type;
        }

        if (preassignedPlot)
        {
            preassignedPlot = false;
            return;
        }
        
        if (currentBuilding == null)
        {
            state = PlotState.Empty;
            
        }
        else
        {
            if (finishedBuilding)
            {
                state = PlotState.Occupied;
            }
            else
            {
                state = PlotState.Building;
            }
        }



        float dist = Vector3.Distance(GameManager.instance.player.position, transform.position);
        float alphaDifference = Time.deltaTime / borderFadeUpTime;

        if (buildingAnimation == null && Card.overridingAnimation == null)
        {
            Card.anim.SetBool("Open", dist < activeBorderDistance);
            Card.anim.SetBool("Closed", !Card.anim.GetBool("Open"));
        }

        if (state == PlotState.Empty)
        {
            finishedBuilding = false;
            Card.transform.localPosition = new Vector3((float)plotSize * 0.25f, 0.8f, (float)plotSize * 0.48f);


            Card.isBuilding = false;


            if (dist < activeBorderDistance * 1.5f)
            {
                if (pulsing == null)
                {
                    if (LineBorder.BorderAlpha >= 1f)
                    {
                        pulsing = StartCoroutine(PulseBorder());
                    }
                    else
                    {
                        LineBorder.BorderAlpha += alphaDifference * 0.5f;
                        LineBorder.VFXEmissionRate = (float)plotSize * 5f / LineBorder.BorderAlpha;
                    }
                }
            }
            else
            {
                if (pulsing != null)
                {
                    StopCoroutine(pulsing);
                    pulsing = null;
                }
                LineBorder.BorderAlpha -= alphaDifference;
                LineBorder.VFXEmissionRate = (float)plotSize * 5f / LineBorder.BorderAlpha;
            }

            if (LineBorder.BorderAlpha < 0.01f)
            {
                LineBorder.borderParent.gameObject.SetActive(false);
                LineBorder.VFXEmissionRate = 0;
            }
            else
            {
                LineBorder.borderParent.gameObject.SetActive(true);
            }
        }
        else if (state == PlotState.Building)
        {
            float _progress = GetBuildProgress();

            // set the emission rate of the building VFX based on the current build progress
            float _emission = Mathf.Lerp(0f, buildVFXMaxEmission, _progress);
            buildVFX.SetFloat("EmissionRate", _emission);

            Card.BuildProgress = GetBuildProgress();
            Card.isBuilding = true;

            LineBorder.VFXEmissionRate = 0f;

            if (currentBuildingObject == null)
            {
                currentBuildingObject = Instantiate(currentBuilding.prefab, transform);
                currentBuildingObject.SetActive(false);
            }

            if (_progress >= 1f)
            {
                Card.isBuilding = false;
                //if (Vector3.Distance(GameManager.instance.player.position, transform.position) > Mathf.Sqrt(((int)plotSize * 0.5f) * ((int)plotSize * 0.5f)) * 2 && buildingAnimation == null)
                if (!plotCollider.bounds.Contains(GameManager.instance.player.position))
                {
                    finishedBuilding = true;

                    buildingAnimation = StartCoroutine(SpawnBuildingAnim());
                }
            }
        }
        else if (state == PlotState.Occupied)
        {
            if (pulsing != null)
            {
                StopCoroutine(pulsing);
                pulsing = null;
            }

            if (LineBorder.BorderAlpha > 0f)
            {
                LineBorder.BorderAlpha -= alphaDifference;
            }

            buildVFX.SetFloat("EmissionRate", 0);
        }

    }

    private void OnValidate()
    {
        LineBorder.borderSize = (int)plotSize;
        LineBorder.borderHeight = borderHeight;

        Col.isTrigger = true;
        Col.size = new Vector3((int)plotSize, 5f, (int)plotSize);
        Col.center = new Vector3(0, 2.5f, 0);

        if (updatePlot)
        {
            LineBorder.GenerateSquareBorder();
            updatePlot = false;
        }

    }

    public string ReturnInteractActionWord()
    {
        if (state != PlotState.Occupied)
        {
            return interactActionWord;
        }
        if (currentBuilding != null && currentBuildingType == BuildingType.VillageHall && QuestManager.instance.numberOfQuestsToTurnIn > 0)
        {
            return "Press to Turn In " + QuestManager.instance.numberOfQuestsToTurnIn + " Quests";
        }
        return "Press for " + currentBuilding.name + " options";
    }

    public string ReturnAlternateInteractActionWord()
    {
        if (currentBuilding != null && currentBuildingType == BuildingType.VillageHall)
        {
            return "Hold to sleep though the night";
        }
        return "Hold for plot options";
    }

    public bool Interact()
    {
        if (currentBuildingType == BuildingType.VillageHall && QuestManager.instance.numberOfQuestsToTurnIn > 0)
        {
            QuestManager.instance.TurnInAllQuests();
            return true;
        }
        

        // bring up build interface
        if (state == PlotState.Occupied)
        {
            OpenCorrespondingUI(currentBuildingObject.GetComponent<sBuilding>().buildingType);
            return true;
        }
        sMainUI.instance.OpenBuildInterface();
        sMainUI.instance.buildInterface.PlotInteraction(plotID, currentBuilding, plotSize);
        return true;
    }

    void OpenCorrespondingUI(BuildingType buildingType)
    {
        switch (buildingType)
        {
            case BuildingType.VillageHall:
                sMainUI.instance.OpenVIllageBoardUI();
                // Open townhall UI
                break;
            case BuildingType.Blacksmith:
                sMainUI.instance.OpenBlacksmithUI(currentBuildingObject.GetComponent<sBlacksmithBuilding>());
                // Open blacksmith UI
                break;
            case BuildingType.Cookery:
                // Open Cookery UI
                sMainUI.instance.OpenCookeryUI(currentBuildingObject.GetComponent<sCookery>());
                break;
            case BuildingType.CropFarm:
                sMainUI.instance.OpenCropFarmUI(currentBuildingObject.GetComponent<sCropFarm>());
                break;
            case BuildingType.House:
                // not sure what this UI will show, unsure of whether its needed
                break;
            default:
                break;
        }
    }

    public bool AlternateInteract()
    {
        if (currentBuildingType == BuildingType.VillageHall)// the CanInteract function should stop you interacting in the day
        {
            GameManager.instance.dayCycle.Sleep(currentBuildingObject.GetComponent<sVillageHall>());
            return true;
        }
        if (state == PlotState.Occupied)
        {
            sMainUI.instance.OpenBuildInterface();
            sMainUI.instance.buildInterface.PlotInteraction(plotID, currentBuilding, plotSize);
        }
        return true;
    }

    public bool CanInteract()
    {
        if (state != PlotState.Building)
        {
            if (currentBuilding != null)
            {
                if (currentBuilding.type != BuildingType.House)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    public bool CanAlternateInteract()
    {
        if (lockedPlot && currentBuildingType != BuildingType.VillageHall)
        {
            return false;
        }
        
        if (currentBuildingType == BuildingType.VillageHall && !GameManager.instance.dayCycle.isDay)
        {
            return true;
        }
        else if (currentBuildingType == BuildingType.VillageHall && GameManager.instance.dayCycle.isDay)
        {
            return false;
        }

        if (state == PlotState.Occupied)
        {
            return true;
        }
        return false;
    }

    IEnumerator PulseBorder()
    {
        float t;
        float startA;
        do
        {
            t = 0f;
            startA = LineBorder.BorderAlpha;
            do
            {
                t += Time.deltaTime;
                LineBorder.BorderAlpha = Mathf.Lerp(startA, 0.5f, t);
                yield return null;
            } while (t <= 1f);
            t = 0f;
            startA = LineBorder.BorderAlpha;
            do
            {
                t += Time.deltaTime;
                LineBorder.BorderAlpha = Mathf.Lerp(startA, 1f, t);
                yield return null;
            } while (t <= 1f);
        } while (true);
    }

    public void StartBuilding(Date _startDate, Building _building)
    {
        if (waiting != null) { return; }

        if (currentBuilding != null)
        {
            if (buildingAnimation == null)
            {
                buildingAnimation = StartCoroutine(RemoveBuildingAnim(_building));
                return;
            }
        }
        else
        {
            if (waiting == null && buildingAnimation != null)
            {
                waiting = StartCoroutine(WaitForNullRoutine(_startDate, _building));
                return;
            }
        }
        currentBuilding = _building;

        Card.isBuilding = true;
        Card.TimeToComplete = _building.recipe.timeToMake;


        for (int i = 0; i < _building.recipe.recipe.Count; i++)
        {
            VillageManager.instance.villageOneInventory.RemoveItems(_building.recipe.recipe[i]);
        }

        SaveSystem.instance.gameSave.buildings[plotID] = currentBuilding;
    }

    public void RemoveBuilding()
    {
        if (buildingAnimation == null)
        {
            currentBuildingObject.GetComponent<IBuilding>()?.Removed();
            buildingAnimation = StartCoroutine(RemoveBuildingAnim(null));
            SaveSystem.instance.gameSave.buildings[plotID] = null;
            return;
        }
    }

    /// <summary>
    /// Calculate the 0-1 lerp value of time remaining
    /// </summary>
    /// <returns></returns>
    float GetBuildProgress()
    {
        Date temp = currentBuilding.recipe.timeToMake;

        float returnVal = (float)Card.TimeToComplete.TimeInSeconds() / (float)temp.TimeInSeconds();
        
        returnVal = Mathf.Clamp(returnVal, 0f, 1f);

        return 1f - returnVal; 
    }

    IEnumerator SpawnBuildingAnim()
    {
        currentBuildingObject.transform.localScale = Vector3.zero;
        currentBuildingObject.SetActive(true);

        float t = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one * 1.3f;

        Card.anim.SetBool("Open", false);
        Card.anim.SetBool("Closed", true);

        yield return new WaitForSeconds(1.5f);
        Card.UpdateInfoCalledByPlot();

        sBuilding newBuildingScr = currentBuildingObject.GetComponent<sBuilding>();

        Vector3 newPos = newBuildingScr.cardPosition.localPosition;
        newPos.y = 0.8f;
        Card.transform.localPosition = newPos;

        Card.myBuilding = newBuildingScr;

        do
        {
            t += Time.deltaTime / 0.4f;
            currentBuildingObject.transform.localScale = Vector3.Lerp(startScale, endScale, Lerp.SmootherStep(t));
            yield return null;
        } while (t <= 1f);

        t = 0f;
        startScale = currentBuildingObject.transform.localScale;
        endScale = Vector3.one;

        do
        {
            t += Time.deltaTime / 0.25f;
            currentBuildingObject.transform.localScale = Vector3.Lerp(startScale, endScale, Lerp.SmootherStep(t));
            yield return null;
        } while (t <= 1f);

        Card.anim.SetBool("Open", Vector3.Distance(GameManager.instance.player.position, transform.position) < activeBorderDistance);
        Card.anim.SetBool("Closed", Card.anim.GetBool("Open"));
        
        currentBuildingObject.GetComponent<sAgentCreator>()?.OnBuilt();

        buildingAnimation = null;

        VillageManager.instance.villageOnePopulation += currentBuilding.numberOfOcupants;
        if (currentBuildingObject.GetComponent<IBuilding>() is object)
        {
            currentBuildingObject.GetComponent<IBuilding>().Built();
        }

        QuestManager.instance.QuestObjectiveProgress(Quest.QuestObjective.Build, currentBuildingType);
    }

    void PreAssignedPlotStart()
    {
        currentBuildingObject = Instantiate(currentBuilding.prefab, transform);
        currentBuildingObject.SetActive(true);

        currentBuildingObject.transform.localScale = Vector3.one;

        sBuilding newBuildingScr = currentBuildingObject.GetComponent<sBuilding>();

        Vector3 newPos = newBuildingScr.cardPosition.localPosition;
        newPos.y = 0.8f;
        Card.transform.localPosition = newPos;

        Card.UpdateInfoCalledByPlot();

        Card.myBuilding = newBuildingScr;

        Card.anim.SetBool("Open", false);
        Card.anim.SetBool("Closed", true);

        LineBorder.BorderAlpha = 0f;

        state = PlotState.Occupied;

        finishedBuilding = true;

        VillageManager.instance.villageOnePopulation += currentBuilding.numberOfOcupants;
        if (currentBuildingObject.TryGetComponent(out IBuilding scr))
        {
            scr.Built();
        }
        if (currentBuildingObject.TryGetComponent(out sAgentCreator scr2))
        {
            scr2.OnBuilt();
        }
    }

    IEnumerator RemoveBuildingAnim(Building _newBuilding)
    {
        currentBuildingObject.transform.localScale = Vector3.one;

        float t = 0f;
        Vector3 startScale = Vector3.one;
        Vector3 endScale = Vector3.one * 1.3f;

        Card.anim.SetBool("Open", false);
        Card.anim.SetBool("Closed", true);

        yield return new WaitForSeconds(1.5f);

        Card.transform.localPosition = new Vector3((float)plotSize * 0.25f, 0.8f, (float)plotSize * 0.48f);

        do
        {
            t += Time.deltaTime / 0.25f;
            currentBuildingObject.transform.localScale = Vector3.Lerp(startScale, endScale, Lerp.SmootherStep(t));
            yield return null;
        } while (t <= 1f);

        t = 0f;
        startScale = currentBuildingObject.transform.localScale;
        endScale = Vector3.zero;

        do
        {
            t += Time.deltaTime / 0.4f;
            currentBuildingObject.transform.localScale = Vector3.Lerp(startScale, endScale, Lerp.SmootherStep(t));
            yield return null;
        } while (t <= 1f);

        Destroy(currentBuildingObject);
        currentBuildingObject = null;
        currentBuilding = null;

        finishedBuilding = false;

        buildingAnimation = null;

        VillageManager.instance.villageOnePopulation -= currentBuilding.numberOfOcupants;

        if (_newBuilding != null)
        {
            StartBuilding(GameManager.instance.dayCycle.GetCurrentDate(), _newBuilding);
        }
    }

    IEnumerator WaitForNullRoutine(Date _startDate, Building _building)
    {
        do
        {
            yield return null;
        } while (buildingAnimation != null);
        StartBuilding(_startDate, _building);
    }

}
