using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sSoundEffect : MonoBehaviour
{
    [SerializeField] SFX sfx;

    public void PlaySFX()
    {
        SFXAudioManager.CreateSFX(sfx);
    }
}
