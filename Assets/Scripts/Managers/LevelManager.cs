using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    
    [field: Header("Originality")]
    [field: SerializeField]
    public bool OriginalLevel { get; private set; }

    [Header("IDs")]
    [SerializeField]
    private int levelNumber;
    [SerializeField]
    private int nextLevelNumber;

    void Start()
    {

    }

    void Update()
    {
        
    }
}
