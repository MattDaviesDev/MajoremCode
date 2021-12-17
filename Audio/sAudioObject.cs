using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class sAudioObject : MonoBehaviour
{
    public AudioManager.AudioSourceType myType;

    [Header("Ambient stuff")]
    [SerializeField] float currentRandomWait;
    [SerializeField] float maxDelayBeforePlaying;
    [SerializeField] bool muteAtNight;
    [SerializeField] bool wind;
    [SerializeField] float targetVol;

    [Header("Audio stuff")]
    public string songName;
    [SerializeField] AudioClip clip;
    [HideInInspector] public AudioSource source;

    [Header("Gameplay things")]
    [SerializeField] int minHour = 3;
    [SerializeField] int maxHour = 6;

    Coroutine playing;

    public bool isPlaying;

    private void Awake()
    {
        if (source == null)
        {
            source = GetComponent<AudioSource>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentRandomWait = Random.Range(0f, maxDelayBeforePlaying);
        targetVol = Random.Range(0f, 1f);
    }

    private void Update()
    {
        isPlaying = IsPlaying();

        if (myType == AudioManager.AudioSourceType.Ambient)
        {
            if (wind)
            {
                source.volume = Mathf.Lerp(source.volume, targetVol, 0.01f);
                if (currentRandomWait > 0f)
                {
                    currentRandomWait -= Time.deltaTime;
                }
                else
                {
                    currentRandomWait = Random.Range(maxDelayBeforePlaying * 0.5f, maxDelayBeforePlaying);
                    targetVol = Random.Range(0f, 1f);
                }
            }
            else
            {
                if (!isPlaying)
                {
                    if (currentRandomWait > 0f)
                    {
                        currentRandomWait -= Time.deltaTime;
                    }
                    else
                    {
                        Play();
                        currentRandomWait = Random.Range(0f, maxDelayBeforePlaying);
                    }
                }
            }


            if (muteAtNight) 
            {
                float targetVol = GameManager.instance.dayCycle.isDay ? 1f : 0f;
                source.volume = Mathf.Lerp(source.volume, targetVol, 0.01f);
            }
        }
    }

    public void Init(AudioClip _clip, float _defaultPitch, float _maxPitchVariance, AudioManager.AudioSourceType _type)
    {
        if (source == null)
        {
            source = GetComponent<AudioSource>();
        }
        myType = _type;
        source.pitch = _defaultPitch * Random.Range(1f - _maxPitchVariance, 1f + _maxPitchVariance);
        source.playOnAwake = false;
        clip = _clip;
        source.clip = clip;
    }

    public void Play()
    {
        if (playing == null)
        {
            playing = StartCoroutine(Playing());
        }
    }

    public void Stop()
    {
        if (playing != null)
        {
            StopCoroutine(playing);
            playing = null;
        }
    }

    public bool IsPlaying()
    {
        return playing != null;
    }

    IEnumerator Playing()
    {
        source.Play();
        do
        {
            yield return null;
        } while (source.isPlaying);
        playing = null;

        if (myType == AudioManager.AudioSourceType.Music)
        {
            AudioManager.instance.FadeToNewTrack();
        }
    }
}
