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

    private static DifficultyManager s_instance;

    [field: Header("Difficulty")]
    [field: SerializeField]
    public Difficulty GameDifficulty { get; set; }
    public int DifficultyInt { get { return (int)GameDifficulty; } }

    private void Awake()
    {
        if (s_instance is null) s_instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
}
