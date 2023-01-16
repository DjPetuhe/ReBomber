using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TilemapManager : MonoBehaviour
{
    public enum Tile
    {
        Empty = 0,
        Floor = 1,
        BreakableWall = 2,
        UnbreakableWall = 3,
        Start = 4,
        Finish = 5,
        Exit = 6,
        BlueSlime = 7,
        YellowSlime = 8,
        PurpleSlime = 9,
        RedSlime = 10
    }

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

    [Header("Coordinates")]
    [SerializeField] Vector2 leftTopCoords;
    [SerializeField] Vector2 rightBottomCoords;
    private Vector3 _endingCoords;
    public Vector3 EndingCoords { get { return _endingCoords; } }

    private Tilemap _unbreakableTilemap;
    private Tilemap _breakableTilemap;
    private LevelManager _levelManagerScript;
    private TileTransformer _tileTransofrmer;

    public List<List<bool>> Map { get; private set; } = new();

    private void Start()
    {
        _levelManagerScript = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        _unbreakableTilemap = GameObject.Find("UnbreakableTilemap").GetComponent<Tilemap>();
        _breakableTilemap = GameObject.Find("BreakableTilemap").GetComponent<Tilemap>();
        _tileTransofrmer = GameObject.FindGameObjectWithTag("TileTransformer").GetComponent<TileTransformer>();
        if (!_levelManagerScript.OriginalLevel) LoadLevel();
        ReplaceAuxiliaryTiles();
        AdjustCameraBounds();
    }

    private void LoadLevel()
    {
        LevelData level = SaveManager.LoadLevel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().CustomName);
        for (int i = 0; i < level.Position.Count; i++)
        {
            TileBase tile = _tileTransofrmer.TileToTileBase((Tile)level.TilesID[i]);
            if (tile == breakableWallTile || tile == finishTile)
            {
                _breakableTilemap.SetTile(_breakableTilemap.WorldToCell((Vector3Int)level.Position[i]), tile);
                _unbreakableTilemap.SetTile(_unbreakableTilemap.WorldToCell((Vector3Int)level.Position[i]), floorTile);
            }
            else _unbreakableTilemap.SetTile(_unbreakableTilemap.WorldToCell((Vector3Int)level.Position[i]), tile);
        }
        leftTopCoords = new(-1, level.Height + 1);
        rightBottomCoords = new(level.Width + 1, -1);
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
            Map.Add(new List<bool>());
            for (int j = Mathf.RoundToInt(leftTopCoords.x); j <= Mathf.RoundToInt(rightBottomCoords.x); j++)
            {
                Vector3Int pos = new(j, i, 0);
                Tile breakableTile = _tileTransofrmer.TilebaseToTile(_breakableTilemap.GetTile(_breakableTilemap.WorldToCell(pos)));
                Tile unbreakableTile = _tileTransofrmer.TilebaseToTile(_unbreakableTilemap.GetTile(_unbreakableTilemap.WorldToCell(pos)));

                if (unbreakableTile == Tile.UnbreakableWall) Map[i - Mathf.RoundToInt(rightBottomCoords.y)].Add(true);
                else if (breakableTile == Tile.BreakableWall || breakableTile == Tile.Finish) Map[i - Mathf.RoundToInt(rightBottomCoords.y)].Add(true);
                else Map[i - Mathf.RoundToInt(rightBottomCoords.y)].Add(false);

                if (unbreakableTile == Tile.Empty) continue;
                GameObject prefab = PrefabOnTile(unbreakableTile);

                if (prefab is not null) SummonPrefab(prefab, pos);
                if (breakableTile == Tile.Finish)
                {
                    _unbreakableTilemap.SetTile(_unbreakableTilemap.WorldToCell(pos), exitTile);
                    _breakableTilemap.SetTile(_breakableTilemap.WorldToCell(pos), breakableWallTile);
                    _endingCoords = new(j, i, 0);
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
    private GameObject PrefabOnTile(Tile tile)
    {
        return tile switch
        {
            Tile.Start => playerPrefab,
            Tile.BlueSlime => blueSlimePrefab,
            Tile.YellowSlime => yellowSlimePrefab,
            Tile.PurpleSlime => purpleSlimePrefab,
            Tile.RedSlime => redSlimePrefab,
            _ => null
        };
    }

    public Vector2Int PositionToMapIndexes(Vector2 pos)
    {
        return new()
        {
            y = Mathf.RoundToInt(pos.y) - Mathf.RoundToInt(rightBottomCoords.y),
            x = Mathf.RoundToInt(pos.x) - Mathf.RoundToInt(leftTopCoords.x)
        };
    }

    public bool IsFineCoords(int i, int j)
    {
        if (i < 0 || j < 0) return false;
        if (Map.Count == 0 || Map[0].Count == 0) return false;
        if (i > Map.Count - 1 || j > Map[i].Count - 1) return false;
        if (Map[i][j]) return false;
        return true;
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

    public void AfterWallDestroy(Vector2 position)
    {
        Vector2Int mapIndexes = PositionToMapIndexes(position);
        Map[mapIndexes.y][mapIndexes.x] = false;
        Astar.EvaluateNewPathes();
    }

    public void MarkBombOnMap(Vector2Int position) => BombOnMap(true, position);

    public void RemoveBombFromMap(Vector2Int position) => BombOnMap(false, position);

    private void BombOnMap(bool set, Vector2Int position)
    {
        Vector2Int MapIndexes = PositionToMapIndexes(position);
        Map[MapIndexes.y][MapIndexes.x] = set;
    }
}
