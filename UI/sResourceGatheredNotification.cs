using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class sResourceGatheredNotification : MonoBehaviour
{
    [SerializeField] Transform objectParent;
    [SerializeField] Vector3 initialOffset;
    [SerializeField] Vector3 initialVelocity;
    [SerializeField] Vector3 velocityDecrease;
    [SerializeField] float velocityMultiplier;
    [SerializeField] float maxLifetime;

    GameObject prefab;
    List<GameObject> pool = new List<GameObject>();
    List<Vector3> velocity = new List<Vector3>();
    List<float> lifeTimes = new List<float>();


    // Start is called before the first frame update
    void Start()
    {
        prefab = objectParent.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i].activeInHierarchy)
            {
                lifeTimes[i] += Time.deltaTime;
                if (velocity[i] == Vector3.one * float.MaxValue)
                {
                    velocity[i] = initialVelocity;
                }
                else
                {
                    velocity[i] -= velocityDecrease * Time.deltaTime;
                }
                pool[i].transform.position += velocity[i] * Time.deltaTime * velocityMultiplier; 
                if (lifeTimes[i] >= maxLifetime)
                {
                    pool[i].SetActive(false);
                }
            }
            else
            {
                velocity[i] = Vector3.one * float.MaxValue;
                lifeTimes[i] = 0f;
            }
        }   
    }

    public void SpawnNotification(ResourceData data)
    {
        GameObject temp = GetFreeObject();
        temp.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = data.quantity.ToString();
        temp.GetComponentInChildren<Image>().sprite = data.resource.resourceSprite;
        temp.transform.localPosition = initialOffset;
        temp.SetActive(true);

    }

    GameObject GetFreeObject()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                return pool[i];
            }
        }
        GameObject temp = Instantiate(prefab, objectParent);
        pool.Add(temp);
        velocity.Add(Vector3.one * float.MaxValue);
        lifeTimes.Add(0);
        return temp;
    }

}
