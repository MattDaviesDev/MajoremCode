using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "New Gameplay Music", order = 7)]
public class GameplayMusicData : ScriptableObject
{
    public string songName;
    public AudioClip clip;
    public int minHour;
    public int maxHour;
}
