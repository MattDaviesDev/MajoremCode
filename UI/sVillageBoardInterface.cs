using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class sVillageBoardInterface : MonoBehaviour
{
    public sVillageBoardToolTip tooltip;

    public GameObject interfaceObject;

    [Header("Food Stocks")]
    [SerializeField] Image foodStocks;
    [SerializeField] TextMeshProUGUI foodStockAmountText;
    [SerializeField] TextMeshProUGUI foodStockLossText;

    [Header("Population")]
    [SerializeField] Image populationSprite;
    [SerializeField] TextMeshProUGUI populationText;

    [Header("Happiness")]
    [SerializeField] bool showHappinessAsPercentage = true;
    [SerializeField] Image happinessSprite;
    [SerializeField] TextMeshProUGUI happinessText;

    [Header("Effectors")]
    [SerializeField] GameObject effectorObject;
    [SerializeField] Transform effectorHolder;

    List<GameObject> previousEffectors = new List<GameObject>();

    public bool mouseOverEffector = false;

    private void Start()
    {
        effectorObject.SetActive(false);
    }

    private void Update()
    {
        if (mouseOverEffector)
        {
            tooltip.UpdatePosition();
        }
    }

    public void Populate()
    {
        PopulateFoodStocks();
        PopulatePopulation();
        PopulateHappiness();
        PopulateEffectors();
    }

    void PopulateFoodStocks()
    {
        if (VillageManager.instance.villageOneFoodStock != null)
        {
            //foodStocks.fillAmount = VillageManager.instance.villageOneFoodStock.foodStockValue / 100f;
            foodStocks.fillAmount = 1f;
            foodStockAmountText.text = VillageManager.instance.villageOneFoodStock.foodStockValue.ToString();
            foodStockLossText.text = "-" + VillageManager.instance.villageOneFoodStock.FoodStockDrain + " Per day";
        }
    }

    void PopulatePopulation()
    {
        populationText.text = VillageManager.instance.villageOnePopulation.ToString();
    }

    void PopulateHappiness()
    {
        int currentHappiness = VillageManager.instance.villageOneHappiness.currentHappinessLevel;
        if (showHappinessAsPercentage)
        {
            happinessText.text = currentHappiness + "%";
        }
        else
        {
            happinessText.text = currentHappiness + "/100";
        }

        happinessSprite.color = AssetFinder.instance.effectorColorGradient.Evaluate(currentHappiness * 0.01f);
        Sprite temp = null;

        if (currentHappiness >= 80)
        {
            temp = AssetFinder.instance.veryHappyFace;
        }
        else if (currentHappiness >= 60)
        {
            temp = AssetFinder.instance.happyFace;
        }
        else if (currentHappiness >= 40)
        {
            temp = AssetFinder.instance.indifferentFace;
        }
        else if (currentHappiness >= 20)
        {
            temp = AssetFinder.instance.verySadFace;
        }
        else if (currentHappiness >= 0)
        {
            temp = AssetFinder.instance.sadFace;
        }

        happinessSprite.sprite = temp;
    }

    void PopulateEffectors()
    {
        RemoveOldEffectors();

        List<HappinessEffector> currentEffectors = VillageManager.instance.villageOneHappiness.effectorsToApply;
        for (int i = 0; i < currentEffectors.Count; i++)
        {
            GameObject temp = Instantiate(effectorObject, effectorHolder);
            sVisualHappinessEffector tempEffector = temp.GetComponent<sVisualHappinessEffector>();
            tempEffector.UpdateEffectorInfo(currentEffectors[i]);
            previousEffectors.Add(temp);
            temp.SetActive(true);
        }
    }

    void RemoveOldEffectors()
    {
        for (int i = 0; i < previousEffectors.Count; i++)
        {
            Destroy(previousEffectors[i]);
        }
        previousEffectors.Clear();
    }
}
