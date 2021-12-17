using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXAudioManager : MonoBehaviour
{
    [SerializeField] GameObject SFXSource;

    List<sAudioObject> sourcePool = new List<sAudioObject>();

    // Start is called before the first frame update
    void Start()
    {
        if (AudioManager.sfxInstance != null && AudioManager.sfxInstance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            AudioManager.sfxInstance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void CreateSFX(SFX sfx)
    {
        if (AudioManager.sfxInstance != null)
        {
            sAudioObject temp = AudioManager.sfxInstance.GetFreeAudioSource();
            temp.source.spatialBlend = 0;
            temp.source.volume = sfx.defaultVolume;
            temp.Init(sfx.sfx, sfx.defaultPitch, sfx.maxPitchVariance, AudioManager.AudioSourceType.SFX);
            temp.Play();
        }
    }

    public static void CreateSFX(SFX sfx, float volumeMultiplier)
    {
        if (AudioManager.sfxInstance != null)
        {
            sAudioObject temp = AudioManager.sfxInstance.GetFreeAudioSource();
            temp.source.spatialBlend = 0;
            temp.source.volume = sfx.defaultVolume * volumeMultiplier;
            temp.Init(sfx.sfx, sfx.defaultPitch, sfx.maxPitchVariance, AudioManager.AudioSourceType.SFX);
            temp.Play();
        }
    }

    public static void Create3DSFX(SFX sfx, Vector3 pos, float volumeMultiplier)
    {
        if (AudioManager.sfxInstance != null)
        {
            sAudioObject temp = AudioManager.sfxInstance.GetFreeAudioSource();
            temp.transform.position = pos;
            temp.source.spatialBlend = 1;
            temp.source.rolloffMode = AudioRolloffMode.Linear;
            temp.source.maxDistance = sfx.distance;
            temp.source.dopplerLevel = 0;
            temp.source.volume = sfx.defaultVolume * volumeMultiplier;
            temp.Init(sfx.sfx, sfx.defaultPitch, sfx.maxPitchVariance, AudioManager.AudioSourceType.SFX);
            temp.Play();
        }
    }
    public static void Create3DSFX(SFX sfx, Vector3 pos)
    {
        if (AudioManager.sfxInstance != null)
        {
            sAudioObject temp = AudioManager.sfxInstance.GetFreeAudioSource();
            temp.transform.position = pos;
            temp.source.spatialBlend = 1;
            temp.source.rolloffMode = AudioRolloffMode.Linear;
            temp.source.maxDistance = sfx.distance;
            temp.source.dopplerLevel = 0;
            temp.source.volume = sfx.defaultVolume;
            temp.Init(sfx.sfx, sfx.defaultPitch, sfx.maxPitchVariance, AudioManager.AudioSourceType.SFX);
            temp.Play();
        }
    }

    sAudioObject GetFreeAudioSource()
    {
        for (int i = 0; i < sourcePool.Count; i++)
        {
            if (!sourcePool[i].IsPlaying())
            {
                return sourcePool[i];
            }
        }
        GameObject temp = Instantiate(SFXSource, transform);
        sAudioObject tempAudioSource = temp.GetComponent<sAudioObject>();
        sourcePool.Add(tempAudioSource);
        return tempAudioSource;
    }
}
