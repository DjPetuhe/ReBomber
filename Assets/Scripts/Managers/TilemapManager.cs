using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    [Header("ColliderEdge")]
    [SerializeField] PolygonCollider2D mapCollider;

    [Header("Prefabs")]
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject destroyingWallPrefab;

    [Header("Tilemaps")]
    [SerializeField] Tilemap unbreakableTilemap;
    [SerializeField] Tilemap breakableTilemap;

    [Header("Tiles")]
    [SerializeField] TileBase floorTile;
    [SerializeField] TileBase exitTile;
    [SerializeField] TileBase breakableWallTile;

    [Header("Coordinates")]
    [SerializeField] Vector3 startingCoords;
    [SerializeField] Vector3 endingCoords;
    public Vector3 EndingCoords { get { return endingCoords; } }
    [SerializeField] Vector2 leftTopCoords;
    [SerializeField] Vector2 rightBottomCoords;

    private LevelManager _levelManagerScript;

    private void Start()
    {
        _levelManagerScript = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        if (_levelManagerScript.OriginalLevel)
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
            leftTopCoords + new Vector2(-4, 4),
            new Vector2(leftTopCoords.x, rightBottomCoords.y) + new Vector2(-4, -4),
            rightBottomCoords + new Vector2(4, -4),
            new Vector2(rightBottomCoords.x, leftTopCoords.y) + new Vector2(4,4)
        };
        mapCollider.points = newPoints;
        Instantiate(playerPrefab, startingCoords, Quaternion.identity);
    }

    private void LoadLevel()
    {
        //TODO: Load Level from json (maybe by using level manager)
    }

    public void DestroyWall(Vector2 position)
    {
        Vector3Int cell = breakableTilemap.WorldToCell(position);
        if (breakableTilemap.GetTile(cell) is not null)
        {
            breakableTilemap.SetTile(cell, null);
            Instantiate(destroyingWallPrefab, position, Quaternion.identity);
        }
    }
}
