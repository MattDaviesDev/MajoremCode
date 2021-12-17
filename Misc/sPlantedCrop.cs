using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sPlantedCrop : MonoBehaviour
{
    public int currentGrowthStage = 1;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!transform.GetChild(currentGrowthStage - 1).gameObject.activeInHierarchy)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (i == currentGrowthStage - 1)
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }
}
