using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    private LevelManager levelManagerScript;

    [Header("ColliderEdge")]
    [SerializeField]
    private PolygonCollider2D mapCollider;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject destroyingWallPrefab;

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
    public Vector3 EndingCoords { get { return endingCoords; } }
    [SerializeField]
    private Vector2 leftTopCoords;
    [SerializeField]
    private Vector2 rightBottomCoords;

    void Start()
    {
        levelManagerScript = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        if (levelManagerScript.OriginalLevel)
        {
            unbreakableTilemap = GameObject.Find("UnbreakableTilemap").GetComponent<Tilemap>();
            breakableTilemap = GameObject.Find("BreakableTilemap").GetComponent<Tilemap>();
        }
        else LoadLevel();
        unbreakableTilemap.SetTile(unbreakableTilemap.WorldToCell(startingCoords), floorTile);
        unbreakableTilemap.SetTile(unbreakableTilemap.WorldToCell(endingCoords), exitTile);
        breakableTilemap.SetTile(breakableTilemap.WorldToCell(endingCoords), breakableWallTile);
        Vector2[] newPoints = 
        { 
            leftTopCoords + new Vector2(-1, 1),
            new Vector2(leftTopCoords.x, rightBottomCoords.y) + new Vector2(-1, -1),
            rightBottomCoords + new Vector2(1, -1),
            new Vector2(rightBottomCoords.x, leftTopCoords.y) + new Vector2(1,1)
        };
        mapCollider.points = newPoints;
        Instantiate(playerPrefab, startingCoords, Quaternion.identity);
    }

    private void LoadLevel()
    {

    }

   public void DestroyWall(Vector2 position)
    {
        Vector3Int cell = breakableTilemap.WorldToCell(position);
        if (breakableTilemap.GetTile(cell) != null)
        {
            breakableTilemap.SetTile(cell, null);
            Instantiate(destroyingWallPrefab, position, Quaternion.identity);
        }
    }
}
