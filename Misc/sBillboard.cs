using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sBillboard : MonoBehaviour
{
    Transform mainCamTransform;

    // Start is called before the first frame update
    void Start()
    {
        mainCamTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 temp = transform.position - mainCamTransform.position;
        temp.y = 0;
        transform.forward = temp;
    }

    public static Vector3 GetForwardNoYAxis(Vector3 a, Vector3 b)
    {
        Vector3 temp = a - b;
        temp.y = 0;
        return temp;
    }

    public static Vector3 GetForwardNoYAxis(Transform a, bool asNormal = false)
    {
        Vector3 temp = a.forward;
        temp.y = 0;
        if (asNormal)
        {
            return temp.normalized;
        }
        return temp;
    }
}
