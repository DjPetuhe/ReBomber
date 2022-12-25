using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using Difficulty = DifficultyManager.Difficulty;

public static class SaveManager
{
    private static readonly string s_preferencePath = Application.persistentDataPath + "/preference.pr";

    public static void SavePreferences(float soundVolume, float musicVolume, Difficulty difficulty, int recordLevelID)
    {
        BinaryFormatter formatter = new();
        FileStream stream = new(s_preferencePath, FileMode.OpenOrCreate, FileAccess.Write)
        {
            Position = 0
        };
        PlayerPrefs preferences = new(soundVolume, musicVolume, (int)difficulty, recordLevelID);
        formatter.Serialize(stream, preferences);
        stream.Close();
    }

    public static PlayerPrefs LoadPreferences()
    {
        if (File.Exists(s_preferencePath))
        {
            BinaryFormatter formatter = new();
            FileStream stream = new(s_preferencePath, FileMode.Open, FileAccess.Read)
            {
                Position = 0
            };
            PlayerPrefs preferences = formatter.Deserialize(stream) as PlayerPrefs;
            stream.Close();
            return preferences;
        }
        else Debug.Log($"Save file not found in {s_preferencePath}!");
        return null;
    }
}
