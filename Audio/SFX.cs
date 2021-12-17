using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "New SFX", order = 8)]
public class SFX : ScriptableObject
{
    public AudioClip sfx;
    public float defaultVolume;
    public float defaultPitch;
    [Range(0f,1f)] public float maxPitchVariance;

    [Header("3D stuff")]
    public float distance;
}
