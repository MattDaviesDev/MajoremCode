using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class sScaleChildren : MonoBehaviour
{
    [Header("Uniform scale arguments")]
    [SerializeField] bool uniformScale = true;
    [SerializeField] LimitValue unitformScaleLimits;


    [Header("Scale arguments")]
    [SerializeField] bool xScale = false;
    [SerializeField] LimitValue xLimit;

    [Space(10)]

    [SerializeField] bool yScale = false;
    [SerializeField] LimitValue yLimit;

    [Space(10)]

    [SerializeField] bool zScale = false;
    [SerializeField] LimitValue zLimit;


    // Start is called before the first frame update
    void Start()
    {
        Destroy(this);
    }

    public void UpdateChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (uniformScale)
            {
                float randomScale = Random.Range(unitformScaleLimits.min, unitformScaleLimits.max);
                transform.GetChild(i).transform.localScale = new Vector3(randomScale, randomScale, randomScale);
            }
            else
            {
                float randomX = Random.Range(xLimit.min, xLimit.max);
                float randomY = Random.Range(yLimit.min, yLimit.max);
                float randomZ = Random.Range(zLimit.min, zLimit.max);
                transform.GetChild(i).transform.localScale = new Vector3(randomX, randomY, randomZ);
            }
        }
    }

    public void ResetChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).transform.localScale = Vector3.one;
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(sScaleChildren))]
public class ScaleChildrenEditor : Editor
{
    sScaleChildren _target;

    public override void OnInspectorGUI()
    {
        _target = (sScaleChildren)target;

        DrawDefaultInspector();

        GUILayout.Space(20);
        if (GUILayout.Button("Update All Children"))
        {
            _target.UpdateChildren();
        }

        GUILayout.Space(20);
        if (GUILayout.Button("Reset all children scales to 1"))
        {
            _target.UpdateChildren();
        }
    }
}
#endif