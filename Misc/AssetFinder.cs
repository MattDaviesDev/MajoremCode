using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetFinder : MonoBehaviour
{
    static AssetFinder _instance;

    public static AssetFinder instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<AssetFinder>();
                if (!_instance)
                {
                    Debug.LogError("No Asset Finder in scene.");
                }
            }
            return _instance;
        }
    }

    Transform trash;

    public Transform Trash
    {
        get
        {
            if (trash == null)
            {
                trash = new GameObject("DELETE ME!").transform;
            }
            return trash;
        }
    }

    private void Start()
    {
        if (Trash.childCount > 0)
        {
            Destroy(Trash.gameObject);
        }
    }

    private void Update()
    {
        if (Trash != null)
        {
            Destroy(Trash.gameObject);
        }
    }

    public Resource ReturnResource(int ID)
    {
        for (int i = 0; i < resourceData.Count; i++)
        {
            if (resourceData[i].ID == ID)
            {
                return resourceData[i];
            }
        }
        Debug.LogError("No item with that ID exists");
        return null;
    }

    [Header("Accessibility")]
    public Color positiveColor;
    public Color negativeColor;

    [Header("SFX Scriptable Objects")]
    public SFX waterSplashSFX;
    public SFX toolSwooshSFX;
    public SFX axeHitDefaultSFX;
    public SFX axeHitWithLeavesSFX;
    public SFX pickaxeHitDefaultSFX;
    public SFX pickaxeHitWithRubbleSFX;
    public SFX pickaxeHitMetalDefaultSFX;
    public SFX pickaxeHitMetalWithRubbleSFX;
    public SFX treeDepleatedSFX;
    public SFX rockDepleatedSFX;
    public SFX grabSeedSFX;
    public SFX lanternTurnOnSFX;
    public SFX lanternTurnOffSFX;
    public SFX doorOpenSFX;
    public SFX doorCloseSFX;
    public SFX gateOpenSFX;
    public SFX gateCloseSFX;
    public SFX[] bushShakeSFX;
    [Space(10)]
    public SFX[] dirtFootsteps;
    public SFX[] grassFootsteps;
    public SFX[] woodFootsteps;
    public SFX[] rockFootsteps;
    public SFX[] waterFootSteps;
    public SFX[] swimmingSFX;
    public SFX[] jumpingSFX;
    [Space(10)]
    public SFX[] lanternHits;
    public SFX[] lanternSqueaks;

    [Header("Other")]
    public GameObject gatherPoofVFX;
    public GameObject onHitDebrisVFX;
    public LayerMask footStepMask;

    [Header("Plots")]
    public GameObject plotBorder;
    public GameObject plotVFX;
    public GameObject plotBuildingVFX;
    public GameObject buildingCard;

    [Header("Water")]
    public GameObject waterSplashVFX;

    [Header("Resources")]
    public List<Resource> resourceData = new List<Resource>();
    public Sprite foodStockSprite;

    [Header("Happiness Effectors")]
    public Sprite verySadFace;
    public Sprite sadFace;
    public Sprite indifferentFace;
    public Sprite happyFace;
    public Sprite veryHappyFace;
    public Gradient effectorColorGradient;

    [Header("Flower Colors")]
    public Color[] allPetalColors;
    public Material[] allFlower1PetalMaterial;
    public Material[] allFlower2PetalMaterial;
    public Material[] allFlower3PetalMaterial;
    public Material[] allFlower4PetalMaterial;
    public Material[] allFlower5PetalMaterial;

    [Header("Flower Colors")]
    public Color[] rareRandomColors;
    public Color[] villagerSkinColors;
    public Color[] villagerUndershirtColors;
    public Color[] villagerOvershirtColors;
    public Color[] villagerTrouserColors;
    public Color[] villagerShoeColors;


    [Header("AI")]
    public List<UnityEngine.AI.NavMeshModifierVolume> pathVolumes = new List<UnityEngine.AI.NavMeshModifierVolume>();
    public static UnityEngine.AI.NavMeshModifierVolume ReturnPathVolume(int index)
    {
        return AssetFinder.instance.pathVolumes[index];
    }
    public static int ReturnPathVolumeCount()
    {
        return AssetFinder.instance.pathVolumes.Count;
    }
}
