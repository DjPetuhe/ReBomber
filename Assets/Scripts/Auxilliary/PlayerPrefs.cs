using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerPrefs
{
    public float SoundVolume;
    public float MusicVolume;
    public int Difficulty;

    public PlayerPrefs(float soundVolume, float musicVolume, int difficulty)
    {
        SoundVolume = soundVolume;
        MusicVolume = musicVolume;
        Difficulty = difficulty;
    }
}
