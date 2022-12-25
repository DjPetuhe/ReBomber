using UnityEngine;

//TODO: transition between levels, probably singleton.
public class LevelManager : MonoBehaviour
{
    [field: Header("Originality")]
    [field: SerializeField]
    public bool OriginalLevel { get; private set; }

    [Header("IDs")]
    [SerializeField] int levelNumber;
    [SerializeField] int nextLevelNumber;

    void Start()
    {

    }

    void Update()
    {
        
    }
}
