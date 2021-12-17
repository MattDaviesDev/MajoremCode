using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "New Game Save", order = 10)]
public class Save : ScriptableObject
{

    public Date date;

    public int happiness, foodStock;

    public List<Building> buildings = new List<Building>();

    public List<ResourceData> inventory = new List<ResourceData>();

}
