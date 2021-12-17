using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TextMeshProHelper;

public class sTextMeshObject : MonoBehaviour
{
    public TextMeshType textType = TextMeshType.Body;
    public bool locked = false;

    private void Start()
    {
        Destroy(this);
    }
}
