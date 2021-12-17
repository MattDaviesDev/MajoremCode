using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class sSeedHolder : MonoBehaviour
{

    //[HideInInspector]
    [HideInInspector] public Resource seed;

    [SerializeField] TextMeshProUGUI seedName;
    [SerializeField] Image seedImage;
    [SerializeField] Image productImage;
    [SerializeField] TextMeshProUGUI numberOfProductsPerSeedText;

    int quantityInInventory;

    public void PopulateSeedInfo(Resource _seed, int _quantityInInventory)
    {
        seed = _seed;
        quantityInInventory = _quantityInInventory;

        seedName.text = seed.resourceName + " - x" + quantityInInventory;
        seedImage.sprite = seed.resourceSprite;
        productImage.sprite = seed.recipe.recipe[0].resource.resourceSprite;
        numberOfProductsPerSeedText.text = "1-" + seed.recipe.recipe[0].quantity + " per seed";
    }

    public void SelectSeed()
    {
        sMainUI.instance.cropFarmInterface.SelectSeed(seed, quantityInInventory);
    }
}
