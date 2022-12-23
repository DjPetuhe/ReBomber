using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public enum Difficulty
    {
        GodMod = 0,
        Easy = 1,
        Medium = 2,
        Hard = 3
    }

    private static DifficultyManager instance;

    [field: Header("Difficulty")]
    [field: SerializeField]
    public Difficulty GameDifficulty { get; set; }
    public int DifficultyInt
    {
        get { return (int)GameDifficulty; }
    }

    private void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
}
