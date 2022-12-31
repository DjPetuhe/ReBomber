using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    [Header("ColliderEdge")]
    [SerializeField] PolygonCollider2D mapCollider;

    [Header("Prefabs")]
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject redSlimePrefab;
    [SerializeField] GameObject blueSlimePrefab;
    [SerializeField] GameObject yellowSlimePrefab;
    [SerializeField] GameObject purpleSlimePrefab;
    [SerializeField] GameObject destroyingWallPrefab;
    [SerializeField] GameObject nextLevelTriggerPrefab;

    [Header("Tiles")]
    [SerializeField] TileBase floorTile;
    [SerializeField] TileBase exitTile;
    [SerializeField] TileBase breakableWallTile;
    [SerializeField] TileBase finishTile;
    [SerializeField] TileBase startTile;
    [SerializeField] TileBase redSlimeTile;
    [SerializeField] TileBase purpleSlimeTile;
    [SerializeField] TileBase yellowSlimeTile;
    [SerializeField] TileBase blueSlimeTile;

    [Header("Coordinates")]
    [SerializeField] Vector3 startingCoords;
    [SerializeField] Vector3 endingCoords;
    public Vector3 EndingCoords { get { return endingCoords; } }
    [SerializeField] Vector2 leftTopCoords;
    [SerializeField] Vector2 rightBottomCoords;

    private Tilemap _unbreakableTilemap;
    private Tilemap _breakableTilemap;
    private LevelManager _levelManagerScript;

    private void Start()
    {
        _levelManagerScript = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        if (_levelManagerScript.OriginalLevel)
        {
            _unbreakableTilemap = GameObject.Find("UnbreakableTilemap").GetComponent<Tilemap>();
            _breakableTilemap = GameObject.Find("BreakableTilemap").GetComponent<Tilemap>();
        }
        else LoadLevel();
        ReplaceAuxiliaryTiles();
        AdjustCameraBounds();
    }

    private void LoadLevel()
    {
        //TODO: Load Level from json (maybe by using level manager)
    }

    private void AdjustCameraBounds()
    {
        Vector2[] newPoints =
        {
            leftTopCoords + new Vector2(-4, 4),
            new Vector2(leftTopCoords.x, rightBottomCoords.y) + new Vector2(-4, -4),
            rightBottomCoords + new Vector2(4, -4),
            new Vector2(rightBottomCoords.x, leftTopCoords.y) + new Vector2(4,4)
        };
        mapCollider.points = newPoints;
    }

    private void ReplaceAuxiliaryTiles()
    {
        for (int i = Mathf.RoundToInt(rightBottomCoords.y); i <= Mathf.RoundToInt(leftTopCoords.y); i++)
        {
            for (int j = Mathf.RoundToInt(leftTopCoords.x); j <= Mathf.RoundToInt(rightBottomCoords.x); j++)
            {
                Vector3Int pos = new Vector3Int(j, i, 0);
                TileBase breakableTile = _breakableTilemap.GetTile(_breakableTilemap.WorldToCell(pos));
                TileBase unbreakableTile = _unbreakableTilemap.GetTile(_unbreakableTilemap.WorldToCell(pos));
                if (unbreakableTile == null) continue;
                else if (unbreakableTile == startTile) SummonPrefab(playerPrefab, pos);
                else if (unbreakableTile == redSlimeTile) SummonPrefab(redSlimePrefab, pos);
                else if (unbreakableTile == blueSlimeTile) SummonPrefab(blueSlimePrefab, pos);
                else if (unbreakableTile == yellowSlimeTile) SummonPrefab(yellowSlimePrefab, pos);
                else if (unbreakableTile == purpleSlimeTile) SummonPrefab(purpleSlimePrefab, pos);
                if (breakableTile == null) continue;
                else if (breakableTile == finishTile)
                {
                    _unbreakableTilemap.SetTile(_unbreakableTilemap.WorldToCell(pos), exitTile);
                    _breakableTilemap.SetTile(_breakableTilemap.WorldToCell(pos), breakableWallTile);
                    Instantiate(nextLevelTriggerPrefab, pos, Quaternion.identity);
                }
            }
        }
    }

    private void SummonPrefab(GameObject prefab, Vector3Int pos)
    {
        Instantiate(prefab, pos, Quaternion.identity);
        _unbreakableTilemap.SetTile(_unbreakableTilemap.WorldToCell(pos), floorTile);
    }

    public void DestroyWall(Vector2 position)
    {
        Vector3Int cell = _breakableTilemap.WorldToCell(position);
        if (_breakableTilemap.GetTile(cell) is not null)
        {
            _breakableTilemap.SetTile(cell, null);
            Instantiate(destroyingWallPrefab, position, Quaternion.identity);
        }
    }
}
