using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class sResourceNode : MonoBehaviour, IHarvestable
{
    enum ResourceType
    {
        Metal, Stone, Wood
    }

    sHarvesting harvesting;

    [SerializeField] sHarvesting.ToolType toolType = sHarvesting.ToolType.Pickaxe;

    [SerializeField] ResourceType resourceType = ResourceType.Stone;

    public List<Vector3> critSpotSpawnPoints;
    int numberOfSpots = 30;

    [SerializeField] Vector3 boxBounds;

    [Header("GameObject")]
    [SerializeField] GameObject replenishedObject;
    [SerializeField] GameObject depleatedObject;

    [Header("Resources")]
    [SerializeField] List<ResourceData> availableResources = new List<ResourceData>();
    [SerializeField] Date currentReplenishTime = Date.Zero();
    public int maxResources;
    bool depleated;

    [Header("Values")]
    [SerializeField] public ResourceNodeData nodeData;
    [SerializeField, Range(1, 5)] int critMultiplier = 1;
    public Transform hitPointsPosition;

    [Header("VFX")]
    [SerializeField] Mesh vfxMesh;
    [SerializeField] float vfxSize;
    [SerializeField] Texture2D vfxTexture;

    SFX defaultHit;
    SFX specialHit;
    SFX depleatedSFX;

    string treeHitInVillageMessage = "<color=red>Don't cut down trees in the village!<br>-1 Happiness.";

    private void Start()
    {

        critSpotSpawnPoints = GetCritSpawnPoints();
        //critSpotSpawnPoints = new List<Vector3>(replenishedObject.GetComponent<MeshFilter>().mesh.vertices);
        //List<Vector3> temp = new List<Vector3>();

        //for (int i = 0; i < critSpotSpawnPoints.Count; i++)
        //{
        //    if (critSpotSpawnPoints[i].y <= 2f)
        //    {
        //        temp.Add(critSpotSpawnPoints[i]);
        //    }
        //    //if (Vector3.Distance(replenishedMeshVertices[i], replenishedBounds.center) <= 1f)
        //    //{
        //    //}
        //}
        //critSpotSpawnPoints = temp;

        for (int i = 0; i < nodeData.defaultResources.Count; i++)
        {
            maxResources += nodeData.defaultResources[i].quantity;
        }
        OnReplenish();

        if (resourceType == ResourceType.Wood)
        {
            defaultHit = AssetFinder.instance.axeHitDefaultSFX;
            specialHit = AssetFinder.instance.axeHitWithLeavesSFX;
            depleatedSFX = AssetFinder.instance.treeDepleatedSFX;
        }
        else if (resourceType == ResourceType.Stone)
        {
            defaultHit = AssetFinder.instance.pickaxeHitDefaultSFX;
            specialHit = AssetFinder.instance.pickaxeHitWithRubbleSFX;
            depleatedSFX = AssetFinder.instance.rockDepleatedSFX;
        }
        else if (resourceType == ResourceType.Metal)
        {
            defaultHit = AssetFinder.instance.pickaxeHitMetalDefaultSFX;
            specialHit = AssetFinder.instance.pickaxeHitMetalWithRubbleSFX;
            depleatedSFX = AssetFinder.instance.rockDepleatedSFX;
        }

        depleatedObject.SetActive(false);
        replenishedObject.SetActive(true);

        harvesting = GameManager.instance.player.GetComponent<sHarvesting>();
    }

    // Update is called once per frame
    void Update()
    {
        if (depleated)
        {
            currentReplenishTime.Seconds -= Time.deltaTime * GameManager.instance.dayCycle.currentTimeConversion;

            if (currentReplenishTime.Equals(Date.Zero()))
            {
                OnReplenish();
            }
        }
    }

    List<Vector3> GetCritSpawnPoints()
    {
        List<Vector3> returnList = new List<Vector3>();

        for (int i = 0; i < numberOfSpots; i++)
        {
            Vector3 randomPoint = new Vector3(Random.Range(-boxBounds.x, boxBounds.x), Random.Range(0f, boxBounds.y), Random.Range(-boxBounds.z, boxBounds.z));
            returnList.Add(randomPoint);
        }

        return returnList;
    }

    public sHarvesting.ToolType GetTool()
    {
        return toolType;
    }

    public int GetCurrentResourceCount()
    {
        int returnVal = 0;
        for (int i = 0; i < availableResources.Count; i++)
        {
            returnVal += availableResources[i].quantity;
        }
        return returnVal;
    }

    public float GetCurrentResourceProgres()
    {
        int temp = 0;
        for (int i = 0; i < availableResources.Count; i++)
        {
            temp += availableResources[i].quantity;
        }
        return (float)temp / (float)maxResources;
    }

    public void OnDepleated()
    {
        SFXAudioManager.CreateSFX(depleatedSFX);
        currentReplenishTime = new Date(nodeData.replenishTime);
        depleatedObject.SetActive(true);
        replenishedObject.SetActive(false);
        depleated = true;

        if (sSceneController.instance.currentSceneType == SceneType.Village)
        {
            VillageManager.instance.villageOneHappiness.UpdateHappiness(-1);
            sMainUI.instance.infoPopUpMessage.PushNewInfoMessage(treeHitInVillageMessage);
        }
    }

    public void OnReplenish()
    {
        availableResources.Clear();
        for (int i = 0; i < nodeData.defaultResources.Count; i++)
        {
            availableResources.Add(new ResourceData(nodeData.defaultResources[i].resource, nodeData.defaultResources[i].quantity));
        }

        replenishedObject.SetActive(true);
        depleatedObject.SetActive(false);
        depleated = false;
    }

    public void OnHit(bool isCrit)
    {
        if (Random.Range(0, 5) == 1) // 20% chance to play rareSFX
        {
            SFXAudioManager.CreateSFX(specialHit);
        }
        else
        {
            SFXAudioManager.CreateSFX(defaultHit);
        }

        int randomIndex = Random.Range(0, availableResources.Count);
        // storing the resource data to be manipulated by crit
        ResourceData newData = new ResourceData(availableResources[randomIndex].resource, 1);

        // setting the amount to remove based on the amount left and the crit value
        int critQuantity = availableResources[randomIndex].quantity > critMultiplier ? critMultiplier : availableResources[randomIndex].quantity;
        newData.quantity = isCrit ? critQuantity : newData.quantity;

        // removing the correct number of resources
        int valueToRemove = isCrit ? critQuantity : 1;
        availableResources[randomIndex].quantity -= valueToRemove;

        // removing the item from the list if there is none left
        if (availableResources[randomIndex].quantity == 0)
        {
            availableResources.RemoveAt(randomIndex);
            if (availableResources.Count == 0)
            {
                OnDepleated();
            }
        }

        QuestManager.instance.QuestObjectiveProgress(Quest.QuestObjective.Gather, newData);

        sMainUI.instance.critSpot.HideCrit();

        // adding items to the wilderness inventory
        sMainUI.instance.resourceNotification.SpawnNotification(newData);
        if (sSceneController.instance.currentSceneType == SceneType.Wilderness)
        {
            WildernessManager.instance.wildernessInventory.AddItems(newData);
        }
        else
        {
            VillageManager.instance.villageOneInventory.AddItems(newData);
        }

    }

    public void SpawnCritSpot()
    {
        Vector3 posOffset = critSpotSpawnPoints[Random.Range(0, critSpotSpawnPoints.Count)];

        Vector3 worldPos = replenishedObject.transform.position + posOffset;

        sMainUI.instance.critSpot.ShowCrit(worldPos);
    }

    public bool GetDepleated()
    {
        return depleated;
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
        if (currentReplenishTime > Date.Zero())
        {
            sDayCycle.ReduceTime(currentReplenishTime);
        }
    }

    public sResourceNode GetNode()
    {
        return this;
    }

    public Mesh GetMesh()
    {
        return vfxMesh;
    }

    public float GetVFXSize()
    {
        return vfxSize;
    }

    public Texture2D GetTexture()
    {
        return vfxTexture;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + (Vector3.up * boxBounds.y * 0.5f), boxBounds);
    }

}
