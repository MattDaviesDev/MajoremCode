using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Audio 
{
    public static float GetDecibelFromSingle(System.Single value)
    {
        return Mathf.Log10(value) * 20;
    }

}
