using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sRotateChildren : MonoBehaviour
{
    [SerializeField] bool updateChildren = false;

    [Header("Rotation arguments")]
    [SerializeField] bool rotateX = false;
    [SerializeField] LimitValue xLimit;

    [Space(10)]

    [SerializeField] bool rotateY = false;
    [SerializeField] LimitValue yLimit;

    [Space(10)]

    [SerializeField] bool rotateZ = false;
    [SerializeField] LimitValue zLimit;


    // Start is called before the first frame update
    void Start()
    {
        Destroy(this);
    }

    private void OnValidate()
    {
        if (updateChildren)
        {
            UpdateChildren();
            updateChildren = false;
        }
    }

    void UpdateChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            float randomX = Random.Range(xLimit.min, xLimit.max);
            float randomY = Random.Range(yLimit.min, yLimit.max);
            float randomZ = Random.Range(zLimit.min, zLimit.max);
            transform.GetChild(i).transform.rotation *= Quaternion.Euler(randomX, randomY, randomZ); 
        }
    }
}
