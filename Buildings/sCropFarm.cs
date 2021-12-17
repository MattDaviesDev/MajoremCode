using System.Collections;
using System.Collections.Generic;
using AI.Blackboard;
using UnityEngine;

public class sCropFarm : MonoBehaviour, IBuilding
{
    public int plantedSeedCount;
    public Resource plantedSeed;
    GameObject plantedCropPrefab;

    public Date timeToMake;

    public bool activeCropFarm = false;

    sBuilding myBuilding;

    [SerializeField] HappinessEffector effector;

    bool isUsingDefaultEffector;
    [SerializeField] HappinessEffectorData defaultEffector;
    [SerializeField] HappinessEffectorData workingEffector;
    [SerializeField] HappinessEffectorData noCropEffector;

    [SerializeField] int maxWorkingEffectorValue;
    [SerializeField] int maxNotWorkingEffectorValue;

    int dayOfLastHarvest;
    int dayOfLastEffectorApplication;
    int consecutiveDaysWithNoCrops = 1;
    int consecutiveDaysWithCrops = 1;

    int currentDay;

    const float firstGrowthStage = 0.66f;
    const float secondGrowthStage = 0.33f;
    const float thirdGrowthStage = 0f;

    int stageOneValue;
    int stageTwoValue;
    int stageThreeValue;

    [SerializeField] List<Transform> cropSpawnPoints = new List<Transform>();
    public List<sPlantedCrop> plantedCrops = new List<sPlantedCrop>();
    public List<sPlantedCrop> harvestCrops = new List<sPlantedCrop>(); //used by farmer havest action


    private GameObject farmerObject;
    [Header("AI")] [SerializeField] private GameObject farmer;
    [SerializeField] private Transform spawn;
    public Transform chair;
    public Transform wateringCanPosition;
    public Transform wateringCanObject;
    public sWateringCan wateringCan;

    private void Start()
    {
        dayOfLastHarvest = GameManager.instance.dayCycle.GetCurrentDate().Days;
        myBuilding = GetComponent<sBuilding>();
        effector = new HappinessEffector("Settling in : Crop Farm", "The Crop Farmer is settling in nicely. Give him some crops to keep him happy!", defaultEffector.effectorValue, defaultEffector.effectorSprite);
        myBuilding.currentEffector = effector;
        VillageManager.instance.villageOneHappiness.AddEffector(effector);
        isUsingDefaultEffector = true;
    }

    private void OnEnable()
    {
        sDayCycle.onDailyReset += OnDailyReset;
        sDayCycle.onTimeSkipped += TimeSkipped;
    }

    private void OnDisable()
    {
        sDayCycle.onDailyReset -= OnDailyReset;
        sDayCycle.onTimeSkipped -= TimeSkipped;
    }

    // Update is called once per frame
    void Update()
    {
        currentDay = GameManager.instance.dayCycle.GetCurrentDate().Days;

        if (plantedSeed != null)
        {
            activeCropFarm = true;
            if (!timeToMake.Equals(Date.Zero()))
            {
                timeToMake.Seconds -= Time.deltaTime * GameManager.instance.dayCycle.currentTimeConversion;
            }
            else
            {
                if (GameManager.instance.dayCycle.isDay)
                {
                    HarvestCrop();
                }
            }

            if (plantedCrops.Count > 0)
            {
                int currentStage = 1;
                int tempTime = timeToMake.TimeInSeconds();
                if (tempTime < stageTwoValue)
                {
                    currentStage = 2;
                }
                else if (tempTime < stageThreeValue)
                {
                    currentStage = 3;
                }
                for (int i = 0; i < plantedCrops.Count; i++)
                {
                    plantedCrops[i].currentGrowthStage = currentStage;
                }
            }
        }
        else
        {
            activeCropFarm = false;
            
            if (consecutiveDaysWithNoCrops > 1 && currentDay != dayOfLastEffectorApplication)
            {
                dayOfLastEffectorApplication = currentDay;
                if (isUsingDefaultEffector)
                {
                    VillageManager.instance.villageOneHappiness.RemoveEffector(effector);
                    isUsingDefaultEffector = false;
                }
                effector = new HappinessEffector(noCropEffector);
                effector.SetEffectorValue(Mathf.Clamp(effector.ReturnEffectorValue() * consecutiveDaysWithNoCrops, 0, maxNotWorkingEffectorValue));
                myBuilding.currentEffector = effector;
                VillageManager.instance.villageOneHappiness.AddEffector(effector);
            }
        }
    }

    public void PlantSeeds(Resource _seed, int _seedCount, GameObject plantPrefab)
    {
        plantedSeed = _seed;
        plantedSeedCount = _seedCount;
        plantedCropPrefab = plantPrefab;

        timeToMake = plantedSeed.recipe.recipe[0].resource.recipe.timeToMake; 

        timeToMake = new Date(plantedSeed.recipe.recipe[0].resource.recipe.timeToMake);

        stageOneValue = (int)(timeToMake.TimeInSeconds() * firstGrowthStage);
        stageTwoValue = (int)(stageOneValue * secondGrowthStage);
        stageThreeValue = (int)(stageOneValue * thirdGrowthStage);

        VillageManager.instance.villageOneInventory.RemoveItems(new ResourceData(_seed, _seedCount));

        // Positive effctor things
        if (dayOfLastHarvest == currentDay)
        {
            consecutiveDaysWithCrops++;
        }
        else
        {
            consecutiveDaysWithCrops = 1;
        }

        if (isUsingDefaultEffector)
        {
            VillageManager.instance.villageOneHappiness.RemoveEffector(effector);
            isUsingDefaultEffector = false;
        }
        else if (consecutiveDaysWithNoCrops > 1)
        {
            effector = new HappinessEffector(noCropEffector);
            effector.SetEffectorValue(Mathf.Clamp(effector.ReturnEffectorValue() * consecutiveDaysWithNoCrops, 0, maxNotWorkingEffectorValue));
            VillageManager.instance.villageOneHappiness.RemoveEffector(effector);
        }

        // Add positive effector
        effector = new HappinessEffector("Working : Crop Farm", "The Crop Farm is happily growing some crops!", 1, workingEffector.effectorSprite);
        effector.SetEffectorValue(Mathf.Clamp(effector.ReturnEffectorValue() * consecutiveDaysWithCrops, 1, maxWorkingEffectorValue));
        myBuilding.currentEffector = effector;
        VillageManager.instance.villageOneHappiness.AddEffector(effector);

        SpawnCrops();
    }

    public void SpawnCrops()
    {
        List<sPlantedCrop> tempCrops = new List<sPlantedCrop>();
        for (int i = 0; i < plantedSeedCount; i++)
        {
            GameObject temp = Instantiate(plantedCropPrefab, cropSpawnPoints[i]);
            temp.transform.position = cropSpawnPoints[i].position;
            temp.transform.rotation = Quaternion.identity * Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            temp.SetActive(false); //Enabled by farmer agent
            tempCrops.Add(temp.GetComponent<sPlantedCrop>());
        }

        plantedCrops = tempCrops;
        harvestCrops = tempCrops;
    }

    public void HarvestCrop()
    {
        int harvestCount = 0;
        for (int i = 0; i < plantedSeedCount; i++)
        {
            harvestCount += Random.Range(1, plantedSeed.recipe.recipe[0].quantity + 1);
        }

        ResourceData temp = new ResourceData(plantedSeed.recipe.recipe[0].resource, harvestCount);
        VillageManager.instance.villageOneInventory.AddItems(temp);

        QuestManager.instance.QuestObjectiveProgress(Quest.QuestObjective.Grow, temp);

        plantedSeed = null;
        plantedSeedCount = 0;
        //TODO: Handle the below code in Farmers harvest action
        // for (int i = 0; i < plantedCrops.Count; i++)
        // {
        //     if (plantedCrops[i] != null)
        //     {
        //         Destroy(plantedCrops[i].gameObject);
        //     }
        // }
        plantedCrops.Clear();

        dayOfLastHarvest = GameManager.instance.dayCycle.GetCurrentDate().Days;
    }

    public float GetProgress()
    {
        return 1f - ((float)timeToMake.TimeInSeconds() / (float)plantedSeed.recipe.recipe[0].resource.recipe.timeToMake.TimeInSeconds());
    }

    public string GetProgressTimer()
    {
        return timeToMake.ToString();
    }

    void OnDailyReset()
    {
        // might need to change to && instead of ||, have to test first
        if (currentDay > dayOfLastHarvest || plantedSeed == null)
        {
            consecutiveDaysWithNoCrops++;
            consecutiveDaysWithCrops = 1;
        }
        else
        {
            consecutiveDaysWithNoCrops = 1;
            consecutiveDaysWithCrops++;
        }
    }

    void TimeSkipped()
    {
        if (timeToMake > Date.Zero())
        {
            sDayCycle.ReduceTime(timeToMake);
        }
    }

    public List<sPlantedCrop> ReturnCropList()
    {
        return plantedCrops;
    }

    public bool IsActive()
    {
        return plantedCrops.Count > 0;
    }

    public void PerformTask()
    {
        
    }

    public void Placed()
    {
        
    }

    public void Build()
    {
        
    }

    public void Built()
    {
        farmerObject = Instantiate(farmer, spawn.position, Quaternion.identity, transform);
    }

    public void HandleVisual()
    {
        throw new System.NotImplementedException();
    }

    public void AddToTaskList(ResourceData newItem, int priorityPosition)
    {
        throw new System.NotImplementedException();
    }

    public void CancelTask(int position, bool complete)
    {
        throw new System.NotImplementedException();
    }

    public int ReturnDaysWorked()
    {
        return consecutiveDaysWithCrops - consecutiveDaysWithNoCrops;
    }

    public void Removed()
    {
        Destroy(farmerObject);
        GetComponent<sAgentCreator>()?.OnRemoved();
    }
}
