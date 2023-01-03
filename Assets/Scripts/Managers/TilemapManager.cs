using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

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
    [SerializeField] TileBase unbreakableWallTile;
    [SerializeField] TileBase finishTile;
    [SerializeField] TileBase startTile;
    [SerializeField] TileBase redSlimeTile;
    [SerializeField] TileBase purpleSlimeTile;
    [SerializeField] TileBase yellowSlimeTile;
    [SerializeField] TileBase blueSlimeTile;

    [Header("Coordinates")]
    [SerializeField] Vector2 leftTopCoords;
    [SerializeField] Vector2 rightBottomCoords;
    private Vector3 _endingCoords;
    public Vector3 EndingCoords { get { return _endingCoords; } }

    private Tilemap _unbreakableTilemap;
    private Tilemap _breakableTilemap;
    private LevelManager _levelManagerScript;

    public List<List<bool>> Map { get; private set; } = new();

    private void Start()
    {
        _levelManagerScript = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        _unbreakableTilemap = GameObject.Find("UnbreakableTilemap").GetComponent<Tilemap>();
        _breakableTilemap = GameObject.Find("BreakableTilemap").GetComponent<Tilemap>();
        if (!_levelManagerScript.OriginalLevel) LoadLevel();
        ReplaceAuxiliaryTiles();
        AdjustCameraBounds();
    }

    private void LoadLevel()
    {
        //TODO: Load Level from json (maybe by using level manager)
        //TODO: also load left-top and right-bottom coords 
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
                Tile breakableTile = TilebaseToTile(_breakableTilemap.GetTile(_breakableTilemap.WorldToCell(pos)));
                Tile unbreakableTile = TilebaseToTile(_unbreakableTilemap.GetTile(_unbreakableTilemap.WorldToCell(pos)));

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

    private Tile TilebaseToTile(TileBase tile)
    {
        //Switch only work with const values;
        if (tile == floorTile) return Tile.Floor;
        else if (tile == unbreakableWallTile) return Tile.UnbreakableWall;
        else if (tile == breakableWallTile) return Tile.BreakableWall;
        else if (tile == blueSlimeTile) return Tile.BlueSlime;
        else if (tile == yellowSlimeTile) return Tile.YellowSlime;
        else if (tile == purpleSlimeTile) return Tile.PurpleSlime;
        else if (tile == redSlimeTile) return Tile.RedSlime;
        else if (tile == startTile) return Tile.Start;
        else if (tile == finishTile) return Tile.Finish;
        else if (tile == exitTile) return Tile.Exit;
        else return Tile.Empty;
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
        if (i > Map.Count - 1 || j > Map[0].Count - 1) return false;
        if (Map[i][j]) return false;
        return true;
    }

    public void DestroyWall(Vector2 position)
    {
        Vector3Int cell = _breakableTilemap.WorldToCell(position);
        if (_breakableTilemap.GetTile(cell) is not null)
        {
            Vector2Int mapIndexes = PositionToMapIndexes(position);
            Map[mapIndexes.y][mapIndexes.x] = false;
            _breakableTilemap.SetTile(cell, null);
            Astar.EvaluateNewPathes(Map);
            Instantiate(destroyingWallPrefab, position, Quaternion.identity);
        }
    }

    public void MarkBombOnMap(Vector2Int position) => BombOnMap(true, position);

    public void RemoveBombFromMap(Vector2Int position) => BombOnMap(false, position);

    private void BombOnMap(bool set, Vector2Int position)
    {
        Vector2Int MapIndexes = PositionToMapIndexes(position);
        Map[MapIndexes.y][MapIndexes.x] = set;
    }
}
