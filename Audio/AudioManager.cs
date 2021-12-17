using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;

public class AudioManager : MonoBehaviour
{
    public enum AudioSourceType
    {
        Ambient, Music, SFX
    }

    public static AudioManager instance;
    public static SFXAudioManager sfxInstance;

    public bool shouldPlay = true;

    [SerializeField] AudioMixer mixer;

    [SerializeField] string masterVolParam;
    [SerializeField] string ambientVolParam;
    [SerializeField] string musicVolParam;
    [SerializeField] string SFXVolParam;

    [Header("Prefabs")]
    [SerializeField] GameObject musicSource;

    [Header("Variables")]
    [SerializeField] float fadeUpTime;
    [SerializeField] float silentTime;
    [SerializeField, Range(0.5f, 1.5f)] float silentTimeMultiplier;

    [Header("Current Tracks")]
    [SerializeField] List<GameplayMusicData> allTracks = new List<GameplayMusicData>();
    [Header("Night Tracks")]
    [SerializeField] List<sAudioObject> hour22Tracks = new List<sAudioObject>();
    [SerializeField] List<sAudioObject> hour23Tracks = new List<sAudioObject>();
    [SerializeField] List<sAudioObject> hour0Tracks = new List<sAudioObject>();
    [SerializeField] List<sAudioObject> hour1Tracks = new List<sAudioObject>();
    [SerializeField] List<sAudioObject> hour2Tracks = new List<sAudioObject>();
    [SerializeField] List<sAudioObject> hour3Tracks = new List<sAudioObject>();
    [SerializeField] List<sAudioObject> hour4Tracks = new List<sAudioObject>();
    [SerializeField] List<sAudioObject> hour5Tracks = new List<sAudioObject>();
    [Header("Dawn Tracks")]
    [SerializeField] List<sAudioObject> hour6Tracks = new List<sAudioObject>();
    [SerializeField] List<sAudioObject> hour7Tracks = new List<sAudioObject>();
    [Header("Day Tracks")]
    [SerializeField] List<sAudioObject> hour8Tracks = new List<sAudioObject>();
    [SerializeField] List<sAudioObject> hour9Tracks = new List<sAudioObject>();
    [SerializeField] List<sAudioObject> hour10Tracks = new List<sAudioObject>();
    [SerializeField] List<sAudioObject> hour11Tracks = new List<sAudioObject>();
    [SerializeField] List<sAudioObject> hour12Tracks = new List<sAudioObject>();
    [SerializeField] List<sAudioObject> hour13Tracks = new List<sAudioObject>();
    [SerializeField] List<sAudioObject> hour14Tracks = new List<sAudioObject>();
    [SerializeField] List<sAudioObject> hour15Tracks = new List<sAudioObject>();
    [SerializeField] List<sAudioObject> hour16Tracks = new List<sAudioObject>();
    [SerializeField] List<sAudioObject> hour17Tracks = new List<sAudioObject>();
    [SerializeField] List<sAudioObject> hour18Tracks = new List<sAudioObject>();
    [SerializeField] List<sAudioObject> hour19Tracks = new List<sAudioObject>();
    [Header("Dusk Tracks")]
    [SerializeField] List<sAudioObject> hour20Tracks = new List<sAudioObject>();
    [SerializeField] List<sAudioObject> hour21Tracks = new List<sAudioObject>();
    [Space(30)]
    [SerializeField] List<sAudioObject> currentAvailableClips = new List<sAudioObject>();
    [Space(30)]
    [SerializeField] sAudioObject currentTrack;
    [SerializeField] sAudioObject nextTrack;
    [Space(30)]
    [SerializeField] int currentHour;
    [SerializeField] int nextHour;

    Coroutine fading;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (GameManager.instance != null)
        {
            currentHour = GameManager.instance.dayCycle.GetCurrentDate().Hours;
            if (currentTrack == null && shouldPlay)
            {
                print("no track playing");
                UpdateAvailableTracks();
                FadeToNewTrack();
            }

            if (currentHour == nextHour)
            {
                print("no track playing");
                nextHour = LoopTimeAroundZero(nextHour + 1);
                UpdateAvailableTracks();
                CheckIfTrackIsInAvailableTracks();
            }
            else
            {
                nextHour = LoopTimeAroundZero(currentHour + 1);
            }
        }
    }

    public void UpdateMasterVolume(System.Single value)
    {
        mixer.SetFloat(masterVolParam, Audio.GetDecibelFromSingle(value));
    }

    public void UpdateAmbientVolume(System.Single value)
    {
        mixer.SetFloat(ambientVolParam, Audio.GetDecibelFromSingle(value));
    }

    public void UpdateMusicVolume(System.Single value)
    {
        mixer.SetFloat(musicVolParam, Audio.GetDecibelFromSingle(value));
    }

    public void UpdateSFXVolume(System.Single value)
    {
        mixer.SetFloat(SFXVolParam, Audio.GetDecibelFromSingle(value));
    }

    public void SpawnInAllMusicObjects()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            transform.GetChild(i).parent = AssetFinder.instance.Trash;
        }

        ClearAllLists();

        int min;
        int max;
        for (int i = 0; i < allTracks.Count; i++)
        {
            min = LoopTimeAroundZero(allTracks[i].minHour + 2);
            max = LoopTimeAroundZero(allTracks[i].maxHour + 2);
            sAudioObject temp = CreateNewGlobalAudio(allTracks[i].clip);
            temp.songName = allTracks[i].songName;

            for (int j = 0; j < 24; j++) // 0 - 7 are in the night, 8 - 23 are in the the day
            {
                if (j >= min && j <= max)
                {
                    List<sAudioObject> listToAdd = null;
                    switch (j)
                    {
                        case 0:
                            listToAdd = hour22Tracks;
                            break;
                        case 1:
                            listToAdd = hour23Tracks;
                            break;
                        case 2:
                            listToAdd = hour0Tracks;
                            break;
                        case 3:
                            listToAdd = hour1Tracks;
                            break;
                        case 4:
                            listToAdd = hour2Tracks;
                            break;
                        case 5:
                            listToAdd = hour3Tracks;
                            break;
                        case 6:
                            listToAdd = hour4Tracks;
                            break;
                        case 7:
                            listToAdd = hour5Tracks;
                            break;
                        case 8:
                            listToAdd = hour6Tracks;
                            break;
                        case 9:
                            listToAdd = hour7Tracks;
                            break;
                        case 10:
                            listToAdd = hour8Tracks;
                            break;
                        case 11:
                            listToAdd = hour9Tracks;
                            break;
                        case 12:
                            listToAdd = hour10Tracks;
                            break;
                        case 13:
                            listToAdd = hour11Tracks;
                            break;
                        case 14:
                            listToAdd = hour12Tracks;
                            break;
                        case 15:
                            listToAdd = hour13Tracks;
                            break;
                        case 16:
                            listToAdd = hour14Tracks;
                            break;
                        case 17:
                            listToAdd = hour15Tracks;
                            break;
                        case 18:
                            listToAdd = hour16Tracks;
                            break;
                        case 19:
                            listToAdd = hour17Tracks;
                            break;
                        case 20:
                            listToAdd = hour18Tracks;
                            break;
                        case 21:
                            listToAdd = hour19Tracks;
                            break;
                        case 22:
                            listToAdd = hour20Tracks;
                            break;
                        case 23:
                            listToAdd = hour21Tracks;
                            break;
                    }
                    listToAdd.Add(temp);
                }   

            }
        }
    }

    public void ClearAllLists()
    {
        hour0Tracks.Clear();
        hour1Tracks.Clear();
        hour2Tracks.Clear();
        hour3Tracks.Clear();
        hour4Tracks.Clear();
        hour5Tracks.Clear();
        hour6Tracks.Clear();
        hour7Tracks.Clear();
        hour8Tracks.Clear();
        hour9Tracks.Clear();
        hour10Tracks.Clear();
        hour11Tracks.Clear();
        hour12Tracks.Clear();
        hour13Tracks.Clear();
        hour14Tracks.Clear();
        hour15Tracks.Clear();
        hour16Tracks.Clear();
        hour17Tracks.Clear();
        hour18Tracks.Clear();
        hour19Tracks.Clear();
        hour20Tracks.Clear();
        hour21Tracks.Clear();
        hour22Tracks.Clear();
        hour23Tracks.Clear();
    }


    int LoopTimeAroundZero(int val)
    {
        if (val >= 24)
        {
            return val - 24;
        }
        return val;
    }

    public sAudioObject CreateNewGlobalAudio(AudioClip _clip)
    {
        GameObject newSource = GetNewAudioSource(_clip.name);
        sAudioObject temp = newSource.GetComponent<sAudioObject>();
        temp.Init(_clip, 1f, 0f, AudioSourceType.Music);
        return temp;
    }

    GameObject GetNewAudioSource(string clipName)
    {
        GameObject temp = Instantiate(musicSource, transform);
        temp.name = "Music source: " + clipName;
        temp.transform.parent = transform;
        temp.GetComponent<sAudioObject>();
        return temp;
    }

    void UpdateAvailableTracks()
    {
        switch (currentHour)
        {
            case 0:
                currentAvailableClips = hour0Tracks;
                break;
            case 1:
                currentAvailableClips = hour1Tracks;
                break;
            case 2:
                currentAvailableClips = hour2Tracks;
                break;
            case 3:
                currentAvailableClips = hour3Tracks;
                break;
            case 4:
                currentAvailableClips = hour4Tracks;
                break;
            case 5:
                currentAvailableClips = hour5Tracks;
                break;
            case 6:
                currentAvailableClips = hour6Tracks;
                break;
            case 7:
                currentAvailableClips = hour7Tracks;
                break;
            case 8:
                currentAvailableClips = hour8Tracks;
                break;
            case 9:
                currentAvailableClips = hour9Tracks;
                break;
            case 10:
                currentAvailableClips = hour10Tracks;
                break;
            case 11:
                currentAvailableClips = hour11Tracks;
                break;
            case 12:
                currentAvailableClips = hour12Tracks;
                break;
            case 13:
                currentAvailableClips = hour13Tracks;
                break;
            case 14:
                currentAvailableClips = hour14Tracks;
                break;
            case 15:
                currentAvailableClips = hour15Tracks;
                break;
            case 16:
                currentAvailableClips = hour16Tracks;
                break;
            case 17:
                currentAvailableClips = hour17Tracks;
                break;
            case 18:
                currentAvailableClips = hour18Tracks;
                break;
            case 19:
                currentAvailableClips = hour19Tracks;
                break;
            case 20:
                currentAvailableClips = hour20Tracks;
                break;
            case 21:
                currentAvailableClips = hour21Tracks;
                break;
            case 22:
                currentAvailableClips = hour22Tracks;
                break;
            case 23:
                currentAvailableClips = hour23Tracks;
                break;
        }
    }

    public void CheckIfTrackIsInAvailableTracks()
    {
        if (currentTrack == null)
        {
            FadeToNewTrack();
            return;
        }
        if (!currentAvailableClips.Contains(currentTrack))
        {
            FadeToNewTrack();
        }
    }

    sAudioObject GetNewTrack()
    {
        List<sAudioObject> temp = new List<sAudioObject>(currentAvailableClips);
        if (currentTrack != null && temp.Count > 1)
        {
            temp.Remove(currentTrack);
        }
        return temp[Random.Range(0, temp.Count)];
    }

    public void FadeToNewTrack()
    {
        if (fading == null)
        {
            nextTrack = GetNewTrack();
            fading = StartCoroutine(FadingToNewTrack());
        }
    }

    public void StopPlayingCurrentTrack()
    {
        if (fading == null)
        {
            fading = StartCoroutine(StoppingCurrentTrack());
            shouldPlay = false;
        }
    }

    IEnumerator StoppingCurrentTrack()
    {
        float t = 0f;
        float startVol = 1f;
        float endVol = 0f;

        if (currentTrack != null)
        {
            while (t <= 1f)
            {
                t += Time.deltaTime / 0.2f;
                currentTrack.source.volume = Mathf.Lerp(startVol, endVol, Lerp.EaseIn(t));
                yield return null;
            }
            currentTrack.Stop();
        }
        currentTrack = null;
        fading = null;
    }

    IEnumerator FadingToNewTrack()
    {
        float t = 0f;
        float startVol = 1f;
        float endVol = 0f;

        if (currentTrack != null)
        {
            while (t <= 1f)
            {
                t += Time.deltaTime / fadeUpTime;
                currentTrack.source.volume = Mathf.Lerp(startVol, endVol, Lerp.EaseIn(t));
                yield return null;
            }

            currentTrack.Stop();
        }

        t = 0f;
        startVol = 0f;
        endVol = 1f;

        if (nextTrack != null)
        {
            nextTrack.Play();
        }

        sMainUI.instance.musicPopUp.NewSongPlaying(nextTrack.songName);

        while (t <= 1f)
        {
            t += Time.deltaTime / fadeUpTime;
            if (nextTrack != null)
            {
                nextTrack.source.volume = Mathf.Lerp(startVol, endVol, Lerp.EaseIn(t));
            }
            yield return null;
        }
        fading = null;
        currentTrack = nextTrack;
        nextTrack = null;
    }

}



#if UNITY_EDITOR
[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{
    AudioManager _target;

    public override void OnInspectorGUI()
    {
        _target = (AudioManager)target;

        DrawDefaultInspector();

        GUILayout.Space(20);
        if (GUILayout.Button("Clear all lists"))
        {
            _target.ClearAllLists();
        }

        GUILayout.Space(20);
        if (GUILayout.Button("Create all music objects"))
        {
            _target.SpawnInAllMusicObjects();
        }
    }
}
#endif