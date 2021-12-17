using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingType
{
    VillageHall, Blacksmith, Cookery, CropFarm, House, MediumHouse, LargeHouse
}
[CreateAssetMenu(fileName = "Data", menuName = "New Building", order = 2)]
public class Building : ScriptableObject
{
    public string name;
    [Multiline] public string description;
    public Sprite image;
    public int numberOfOcupants;
    public BuildingType type;
    public PlotSize size;
    public int requiredPopulation;
    public GameObject prefab;
    public Recipe recipe;

}
