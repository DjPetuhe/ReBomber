
[System.Serializable]
public class PlayerPrefs
{
    public float SoundVolume { get; set; }
    public float MusicVolume { get; set; }
    public int Difficulty { get; set; }
    public int RecordLevelID { get; set; }

    public PlayerPrefs(float soundVolume, float musicVolume, int difficulty, int recordLevelID)
    {
        SoundVolume = soundVolume;
        MusicVolume = musicVolume;
        Difficulty = difficulty;
        RecordLevelID = recordLevelID;
    }
}
