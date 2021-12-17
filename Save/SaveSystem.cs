using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public class SaveSystem : MonoBehaviour
{
    public static SaveSystem _instance;
    public static SaveSystem instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SaveSystem>();
            }
            return _instance;
        }
    }

    public Save gameSave;

}
