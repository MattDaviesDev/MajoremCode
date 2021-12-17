using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sVillagerColors : MonoBehaviour
{
    Renderer rend;

    const string skinColorRef = "SkinColor";
    const string undershirtColorRef = "UndershirtColor";
    const string overshirtColorRef = "OvershirtColor";
    const string trouserColorRef = "TrouserColor";
    const string shoeColorRef = "ShoeColor";

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();


        rend.material.SetColor(skinColorRef, AssetFinder.instance.villagerSkinColors[Random.Range(0, AssetFinder.instance.villagerSkinColors.Length)]);
        rend.material.SetColor(shoeColorRef, AssetFinder.instance.villagerShoeColors[Random.Range(0, AssetFinder.instance.villagerShoeColors.Length)]);

        if (Random.Range(0, 10000) == 0)
        {
            Color rareColor = AssetFinder.instance.rareRandomColors[Random.Range(0, AssetFinder.instance.rareRandomColors.Length)];
            rend.material.SetColor(undershirtColorRef, rareColor);
            rend.material.SetColor(overshirtColorRef, rareColor);
            rend.material.SetColor(trouserColorRef, rareColor);
        }
        else
        {
            rend.material.SetColor(undershirtColorRef, AssetFinder.instance.villagerUndershirtColors[Random.Range(0, AssetFinder.instance.villagerUndershirtColors.Length)]);
            rend.material.SetColor(overshirtColorRef, AssetFinder.instance.villagerOvershirtColors[Random.Range(0, AssetFinder.instance.villagerOvershirtColors.Length)]);
            rend.material.SetColor(trouserColorRef, AssetFinder.instance.villagerTrouserColors[Random.Range(0, AssetFinder.instance.villagerTrouserColors.Length)]);
        }

    }

}
