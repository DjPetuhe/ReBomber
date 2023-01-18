using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using Difficulty = DifficultyManager.Difficulty;

public static class SaveManager
{
    private static readonly string s_preferencePath = Application.persistentDataPath + "/preference.pr";
    private static readonly string s_customLevelsPath = Application.persistentDataPath + "/levels/";
    private static readonly string s_statePath = Application.persistentDataPath + "/state.json";

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
        Debug.Log($"Save file not found in {s_preferencePath}!");
        return null;
    }

    public static void SaveLevel(LevelData level)
    {
        Directory.CreateDirectory(s_customLevelsPath);
        string json = JsonUtility.ToJson(level, true);
        File.WriteAllText(s_customLevelsPath + level.Name + ".json", json);
    }

    public static LevelData LoadLevel(string name)
    {
        string levelPath = s_customLevelsPath + name + ".json";
        if (File.Exists(levelPath))
        {
            string json = File.ReadAllText(levelPath);
            return JsonUtility.FromJson<LevelData>(json);
        }
        Debug.Log($"Level file not found in {levelPath}!");
        return null;
    }

    public static void DeleteLevel(string name)
    {
        string levelPath = s_customLevelsPath + name + ".json";
        if (File.Exists(levelPath)) File.Delete(levelPath);
        else Debug.Log($"Level file not found in {levelPath}!");
    }

    public static List<LevelData> LoadAllLevels()
    {
        Directory.CreateDirectory(s_customLevelsPath);
        DirectoryInfo info = new(s_customLevelsPath);
        FileInfo[] fileInfo = info.GetFiles();
        List<LevelData> levels = new();
        foreach (var file in fileInfo)
        {
            string json = File.ReadAllText(file.FullName);
            levels.Add(JsonUtility.FromJson<LevelData>(json));
        }
        return levels;
    }

    public static void SaveGameState(GameStateData gameState)
    {
        Directory.CreateDirectory(s_customLevelsPath);
        string json = JsonUtility.ToJson(gameState, true);
        File.WriteAllText(s_statePath, json);
    }

    public static GameStateData LoadGameState()
    {
        if (File.Exists(s_statePath))
        {
            string json = File.ReadAllText(s_statePath);
            return JsonUtility.FromJson<GameStateData>(json);
        }
        Debug.Log($"Game state file not found in {s_statePath}!");
        return null;
    }

    public static void DeleteGameState()
    {
        if (File.Exists(s_statePath)) File.Delete(s_statePath);
        else Debug.Log($"Game state file not found in {s_statePath}!");
    }

    public static bool IsGameStateExists() => File.Exists(s_statePath);
    public static bool IsLevelExists(string name) => File.Exists(s_customLevelsPath + name + ".json");
}
