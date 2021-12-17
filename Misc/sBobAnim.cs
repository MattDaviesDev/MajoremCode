using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sBobAnim : MonoBehaviour
{
    [SerializeField, Range(0f, 2f)] float bobAmount;

    float t = 0f;
    bool increase = true;

    float myStartingY;
    private void Start()
    {
        myStartingY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (increase)
        {
            t += Time.deltaTime * 0.5f;
            if (t >= 1f)
            {
                increase = false;
            }
        }
        else
        {
            t -= Time.deltaTime * 0.5f;
            if (t <= 0f)
            {
                increase = true;
            }
        }

        t = Mathf.Clamp01(t);

        transform.position = new Vector3(transform.position.x, myStartingY + Mathf.Lerp(-bobAmount * 0.5f, bobAmount * 0.5f, Lerp.SmoothStep(t)), transform.position.z);
    }
}
