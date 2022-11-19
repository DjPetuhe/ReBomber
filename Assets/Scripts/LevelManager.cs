using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private bool originalLevel;

    [Header("IDs")]
    [SerializeField]
    private int levelNumber;
    [SerializeField]
    private int nextLevelNumber;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject playerPrefab;

    [Header("Tilemaps")]
    [SerializeField]
    private Tilemap unbreakableTilemap;
    [SerializeField]
    private Tilemap breakableTilemap;

    [Header("Tiles")]
    [SerializeField]
    private TileBase floorTile;
    [SerializeField]
    private TileBase exitTile;
    [SerializeField]
    private TileBase breakableWallTile;

    [Header("Coordinates")]
    [SerializeField]
    private Vector3 startingCoords;
    [SerializeField]
    private Vector3 endingCoords;

    void Start()
    {
        if (originalLevel)
        {
            unbreakableTilemap = GameObject.Find("UnbreakableTilemap").GetComponent<Tilemap>();
            breakableTilemap = GameObject.Find("BreakableTilemap").GetComponent<Tilemap>();
        }
        else LoadLevel();
        unbreakableTilemap.SetTile(unbreakableTilemap.WorldToCell(startingCoords), floorTile);
        unbreakableTilemap.SetTile(unbreakableTilemap.WorldToCell(endingCoords), exitTile);
        breakableTilemap.SetTile(breakableTilemap.WorldToCell(endingCoords), breakableWallTile);
        Instantiate(playerPrefab, startingCoords, Quaternion.identity);
    }

    private void LoadLevel()
    {

    }

    void Update()
    {
        
    }
}
