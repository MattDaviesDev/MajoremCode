using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class sBuildInterface : MonoBehaviour
{
    [SerializeField] public GameObject buildInterfaceObject;

    [SerializeField] Transform buildingContent;

    [SerializeField] GameObject buildingUICard;

    [SerializeField] List<Building> availableBuildings = new List<Building>();

    List<sBuildingUICard> buildingCards = new List<sBuildingUICard>();

    public int currentPlotID;
    public Building currentBuilding;
    public PlotSize currentPlotSize;

    Building newBuilding;

    [Header("Current building info panel")]
    [SerializeField] TextMeshProUGUI currentName;
    [SerializeField] TextMeshProUGUI currentDescription;
    [SerializeField] TextMeshProUGUI currentPopulation;
    [SerializeField] Image currentBuildingPreview;

    [Header("New building info panel")]
    [SerializeField] TextMeshProUGUI NewName;
    [SerializeField] TextMeshProUGUI newDescription;
    [SerializeField] TextMeshProUGUI newPopulation;
    [SerializeField] Image newBuildingPreview;
    [SerializeField] Button buildButton;

    [SerializeField] GameObject currentInfoPanel;
    [SerializeField] GameObject newInfoPanel;

    // Start is called before the first frame update
    void Start()
    {
        PopulateBuildings();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    public void PlotInteraction(int _plotID, Building _building, PlotSize _plotSize)
    {
        newInfoPanel.SetActive(false);

        for (int i = 0; i < buildingCards.Count; i++)
        {
            buildingCards[i].UpdateInfoWithoutInstantiation();
        }

        currentPlotID = _plotID;
        currentPlotSize = _plotSize;
        currentBuilding = _building;

        if (currentBuilding != null)
        {
            currentInfoPanel.SetActive(true);
            currentBuilding = _building;
            currentName.text = _building.name;
            currentDescription.text = _building.description;
            currentPopulation.text = _building.numberOfOcupants.ToString();
            currentBuildingPreview.sprite = _building.image;
        }
        else
        {
            currentInfoPanel.SetActive(false);
        }
        
        ShowAvailableBuildings(_plotSize);
    }

    void ShowAvailableBuildings(PlotSize _plotSize)
    {
        for (int i = 0; i < buildingCards.Count; i++)
        {
            // only show the buildings which can be placed inside the current plot size
            buildingCards[i].gameObject.SetActive(buildingCards[i].building.size == _plotSize);
        }
    }

    public void PopulateBuildings()
    {
        for (int i = 0; i < availableBuildings.Count; i++)
        {
            GameObject temp = Instantiate(buildingUICard, buildingContent); // creating a new sroll rect item
            sBuildingUICard card = temp.GetComponent<sBuildingUICard>(); // getting card script
            card.PopulateInfo(availableBuildings[i]); // assinging the values show on card
            buildingCards.Add(card); // adding to the list
        }
    }

    public void SelectNewBuilding(Building _building)
    {
        // disable the button if they cannot build and enable if they can
        buildButton.interactable = _building.recipe.CanCraft(VillageManager.instance.villageOneInventory);

        newBuilding = _building;
        NewName.text = _building.name;
        newBuildingPreview.sprite = _building.image;
        newDescription.text = _building.description;
        newPopulation.text = _building.numberOfOcupants.ToString();

        newInfoPanel.SetActive(true);

    }

    public void StartBuilding()
    {
        VillageManager.instance.villageOnePlots[currentPlotID].StartBuilding(GameManager.instance.dayCycle.GetCurrentDate(), newBuilding);
        sMainUI.instance.CloseCurrentWindow();
    }

    public void RemoveBuilding()
    {
        VillageManager.instance.villageOnePlots[currentPlotID].RemoveBuilding();
        currentInfoPanel.SetActive(false);
    }

}
