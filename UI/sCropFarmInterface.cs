using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class sCropFarmInterface : MonoBehaviour
{
    public GameObject cropFarmInterfaceObject;

    [SerializeField] Transform seedContent;
    [SerializeField] List<Resource> seeds = new List<Resource>();
    [SerializeField] List<GameObject> plantedCropPrefab = new List<GameObject>();

    int currentSeedIndex;

    GameObject seedHolder;

    [Header("Info panel")]
    [SerializeField] TextMeshProUGUI seedName;
    [SerializeField] Image seedImage;
    [SerializeField] TextMeshProUGUI seedDescription;
    [SerializeField] Button plantButton;
    [SerializeField] TextMeshProUGUI plantButtonText;
    string canPlantButtonText = "Select a seed";
    string cannotPlantButtonText = "Growing crops!";


    public Resource selectedSeed;
    public int seedCount;

    public sCropFarm currentCropFarm;

    [Header("Progress")]
    [SerializeField] GameObject progressObject;
    [SerializeField] Image currentProductImage;
    [SerializeField] Image progressBar;
    [SerializeField] TextMeshProUGUI progressTimer;

    // Start is called before the first frame update
    void Start()
    {
        seedHolder = seedContent.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentCropFarm != null)
        {
            progressObject.SetActive(currentCropFarm.activeCropFarm); // only show progress object if the vrop farm is active

            if (currentCropFarm.activeCropFarm)
            {
                currentProductImage.sprite = currentCropFarm.plantedSeed.recipe.recipe[0].resource.resourceSprite;
                UpdateProgressBar();
                plantButton.interactable = false;
                plantButtonText.text = cannotPlantButtonText;
            }
            else
            {
                if (selectedSeed == null)
                {
                    plantButton.interactable = false;
                    plantButtonText.text = canPlantButtonText;
                }
                else
                {
                    plantButton.interactable = true;
                }
            }
        }
    }

    public void InteractedWith()
    {
        selectedSeed = null;
    }

    public void PopulateSeedList()
    {
        for (int i = seedContent.childCount - 1; i >= 1; i--)
        {
            Destroy(seedContent.GetChild(i).gameObject);
        }

        for (int i = 0; i < seeds.Count; i++)
        {
            int quantityInInventory = VillageManager.instance.villageOneInventory.ReturnQuantity(seeds[i].ID);
            if (quantityInInventory > 0)
            {
                GameObject temp = Instantiate(seedHolder, seedContent);
                temp.GetComponent<sSeedHolder>().PopulateSeedInfo(seeds[i], quantityInInventory);
                temp.SetActive(true);
            }
        }
    }

    public void SelectSeed(Resource seed, int quantityInInventory)
    {
        for (int i = 0; i < seeds.Count; i++)
        {
            if (seed.ID == seeds[i].ID)
            {
                currentSeedIndex = i;
                print(seed.ID + " and " + seeds[i].ID + " are the same, so my seed index is " + currentSeedIndex);
                break;
            }
        }

        seedName.text = seed.resourceName;
        seedImage.sprite = seed.resourceSprite;
        seedDescription.text = seed.resourceDescription;

        seedCount = quantityInInventory > 20 ? 20 : quantityInInventory;
        plantButtonText.text = "Plant " + seedCount + " seeds";

        selectedSeed = seed;
    }

    public void PlantSeeds()
    {
        currentCropFarm.PlantSeeds(selectedSeed, seedCount, plantedCropPrefab[currentSeedIndex]);
        PopulateSeedList();
    }

    void UpdateProgressBar()
    {
        progressBar.fillAmount = currentCropFarm.GetProgress();
        progressTimer.text = currentCropFarm.GetProgressTimer();
    }

}
