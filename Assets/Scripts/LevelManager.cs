using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public enum Difficulty
    {
        GodMod = 0,
        Easy = 1,
        Medium = 2,
        Hard = 3
    }
    
    [field: Header("Originality")]
    [field: SerializeField]
    public bool OriginalLevel { get; private set; }

    [Header("IDs")]
    [SerializeField]
    private int levelNumber;
    [SerializeField]
    private int nextLevelNumber;

    [field: Header("Difficulty")]
    [field: SerializeField]
    public Difficulty GameDifficulty { get; private set; }
    public int DifficultyInt 
    { 
        get { return (int) GameDifficulty; } 
    }

    void Start()
    {

    }

    void Update()
    {
        
    }
}
