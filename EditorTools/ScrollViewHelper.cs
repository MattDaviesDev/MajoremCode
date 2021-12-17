using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class ScrollViewHelper : MonoBehaviour
{
    [Header("Defaults")]
    [SerializeField] int scrollSensitivity; 


    void Start()
    {
        Destroy(gameObject);
    }
    List<ScrollRect> GetAllScrollViews()
    {
        List<ScrollRect> returnList = new List<ScrollRect>();

        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (go.GetInstanceID() > 0)
            {
                if (go.TryGetComponent(out ScrollRect rect))
                {
                    returnList.Add(rect);
                }
            }
        }
        return returnList;
    }

    public void UpdateAllScrollRects()
    {
        List<ScrollRect> allScrollView = GetAllScrollViews();
        for (int i = 0; i < allScrollView.Count; i++)
        {
            allScrollView[i].scrollSensitivity = scrollSensitivity;
        }
    }

}



#if UNITY_EDITOR
[CustomEditor(typeof(ScrollViewHelper))]
public class ScrollViewHelperEditor : Editor
{
    ScrollViewHelper scrollViewHelper;

    public override void OnInspectorGUI()
    {
        scrollViewHelper = (ScrollViewHelper)target;

        DrawDefaultInspector();

        GUILayout.Space(10);

        if (GUILayout.Button("Update all Scroll view objects"))
        {
            scrollViewHelper.UpdateAllScrollRects();
        }

    }
}
#endif