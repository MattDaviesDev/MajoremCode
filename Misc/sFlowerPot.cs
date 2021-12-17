using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class FlowerLocation
{
    public Transform location;
    public bool occupied;
}

public class sFlowerPot : MonoBehaviour
{

    public List<FlowerLocation> flowerLocations = new List<FlowerLocation>();

    int currentFlowerCount = 0;

    int currentFlowerLocationID = -1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool HasFreeLocation()
    {
        if (currentFlowerCount < flowerLocations.Count)
        {
            return true;
        }
        return false;
    }

    public int GetFreeLocation()
    {
        if (currentFlowerLocationID < 0 || flowerLocations[currentFlowerLocationID].occupied)
        {
            currentFlowerLocationID = Random.Range(0, flowerLocations.Count);
            return GetFreeLocation();
        }
        return currentFlowerLocationID;
    }

    public void PlaceFlowerInLocation(sFlower flower)
    {
        flower.myPotLocationID = currentFlowerLocationID;
        flower.myPot = this;
        flowerLocations[currentFlowerLocationID].occupied = true;
        flower.transform.parent = transform;
        currentFlowerCount++;
    }

    public void MovingFlower(sFlower flower)
    {
        flowerLocations[flower.myPotLocationID].occupied = false;
        flower.transform.parent = sSceneController.instance.GetCurrentSceneParent();
    }

    public void RemoveFlowerFromPot(sFlower flower)
    {
        flowerLocations[flower.myPotLocationID].occupied = false;
    }

}
