using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sFlower : MonoBehaviour
{
    [SerializeField] Renderer petalRend;
    [SerializeField] int flowerNumber;

    [Header("Flower Pot")]
    [SerializeField] public sFlowerPot myPot;
    [SerializeField] public int myPotLocationID;


    private void Awake()
    {
        Material[] temp;
        switch (flowerNumber)
        {
            case 1:
                temp = AssetFinder.instance.allFlower1PetalMaterial;
                break;
            case 2:
                temp = AssetFinder.instance.allFlower2PetalMaterial;
                break;
            case 3:
                temp = AssetFinder.instance.allFlower3PetalMaterial;
                break;
            case 4:
                temp = AssetFinder.instance.allFlower4PetalMaterial;
                break;
            case 5:
                temp = AssetFinder.instance.allFlower5PetalMaterial;
                break;
            default:
                temp = AssetFinder.instance.allFlower1PetalMaterial;
                break;
        }
        petalRend.sharedMaterial = temp[Random.Range(0, temp.Length)];
    }

}


