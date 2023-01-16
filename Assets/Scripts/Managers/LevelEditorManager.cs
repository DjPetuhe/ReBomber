using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Tile = TilemapManager.Tile;

//TODO: call generate function
public class LevelEditorManager : MonoBehaviour
{
    [Header("Line")]
    [SerializeField] GameObject line;

    [Header("Tilemaps")]
    [SerializeField] Tilemap tilemap;
    [SerializeField] Tilemap selectionTilemap;

    [Header("Tiles")]
    [SerializeField] TileBase floorTile;
    [SerializeField] TileBase unbreakableWallTile;
    [SerializeField] TileBase finishTile;
    [SerializeField] TileBase startTile;

    private TileBase _choosenTile;
    private LineRenderer _lineRenderer;
    private PolygonCollider2D _cameraBounds;
    private CameraMovement _cameraMovement;
    private TileTransformer _tileTransformer;

    private bool _isTileChoosen = false;
    private bool _isMouseCanPaint = true;
    private Vector2Int? _choosningTilePos;
    private int _height;
    private int _width;

    public static bool Editing { get; set; } = false;
    public static string LevelName { get; set; }

    private const float OFFSET = -0.5f;
    private const int CAMERA_PADDING = 5;

    private void Start()
    {
        _lineRenderer = line.GetComponent<LineRenderer>();
        _cameraBounds = line.GetComponent<PolygonCollider2D>();
        _cameraMovement = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CameraMovement>();
        _tileTransformer = GameObject.FindGameObjectWithTag("TileTransformer").GetComponent<TileTransformer>();
        if (!Editing) ResizeMap(10, 10);
        else
        {
            LevelData level = SaveManager.LoadLevel(LevelName);
            ResizeMap(level.Height, level.Width);
            for (int i = 0; i < level.Position.Count; i++)
                tilemap.SetTile(tilemap.WorldToCell((Vector3Int)level.Position[i]), _tileTransformer.TileToTileBase((Tile)level.TilesID[i]));
        }
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) DisableMouseSelect();
        else if (_isTileChoosen && _isMouseCanPaint)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int currentPos = new()
            {
                y = Mathf.RoundToInt(mousePos.y),
                x = Mathf.RoundToInt(mousePos.x)
            };
            if (OutsideBorders(currentPos))
            {
                DisableMouseSelect();
                return;
            }
            else if (_choosningTilePos == null || _choosningTilePos != currentPos)
            {
                if (_choosningTilePos != null)
                    selectionTilemap.SetTile(selectionTilemap.WorldToCell((Vector3Int)_choosningTilePos), null);
                _choosningTilePos = currentPos;
                selectionTilemap.SetTile(selectionTilemap.WorldToCell((Vector3Int)_choosningTilePos), _choosenTile);
            }
            if (Input.GetMouseButton(0))
            {
                tilemap.SetTile(tilemap.WorldToCell((Vector3Int)_choosningTilePos), _choosenTile);
            }
        }
    }

    private void DisableMouseSelect()
    {
        if (_choosningTilePos != null)
        {
            selectionTilemap.SetTile(selectionTilemap.WorldToCell((Vector3Int)_choosningTilePos), null);
            _choosningTilePos = null;
        }
    }

    private bool OutsideBorders(Vector2Int pos)
    {
        if (pos.y < 0 || pos.y > _height - 1) return true;
        if (pos.x < 0 || pos.x > _width - 1) return true;
        return false;
    }

    public void ResizeMap(int height, int width)
    {
        if (height == _height && width == _width) return;
        AdjustMap(height, width);
        _height = height;
        _width = width;
        ResizeLine();
        ResizeCameraPadding();
    }

    private void ResizeLine()
    {
        _lineRenderer.SetPosition(0, new(OFFSET, OFFSET));
        _lineRenderer.SetPosition(1, new(OFFSET, OFFSET + _height));
        _lineRenderer.SetPosition(2, new(OFFSET + _width, OFFSET + _height));
        _lineRenderer.SetPosition(3, new(OFFSET + _width, OFFSET));
    }

    private void ResizeCameraPadding()
    {
        Vector2[] newPoints =
        {
            new(OFFSET - CAMERA_PADDING, OFFSET - CAMERA_PADDING),
            new(OFFSET - CAMERA_PADDING, OFFSET + _height + CAMERA_PADDING),
            new(OFFSET + _width + CAMERA_PADDING, OFFSET + _height + CAMERA_PADDING),
            new(OFFSET + _width + CAMERA_PADDING, OFFSET - CAMERA_PADDING)
        };
        _cameraBounds.points = newPoints;
        _cameraMovement.AdjustBounds(line.GetComponent<PolygonCollider2D>());
    }

    private void AdjustMap(int newHeight, int newWidth)
    {
        int maxHeight = Mathf.Max(_height, newHeight);
        int maxWidth = Mathf.Max(_width, newWidth);
        for (int i = -1; i <= maxHeight; i++)
        {
            for (int j = -1; j <= maxWidth; j++)
            {
                Vector3 currPos = new(j, i);
                if ((i == -1 || j == -1 || i == newHeight || j == newWidth) && (i <= newHeight) && (j <= newWidth)) 
                    tilemap.SetTile(tilemap.WorldToCell(currPos), unbreakableWallTile);
                else if (i > newHeight || j > newWidth) 
                    tilemap.SetTile(tilemap.WorldToCell(currPos), null);
                else if (tilemap.GetTile(tilemap.WorldToCell(currPos)) == null || i == _height || j == _width)
                    tilemap.SetTile(tilemap.WorldToCell(currPos), floorTile);
            }
        }
    }

    public void ChooseTile(Tile tile)
    {
        TileBase choosen = _tileTransformer.TileToTileBase(tile);
        if (_isTileChoosen && _choosenTile == choosen)
        {
            _isTileChoosen = false;
            return;
        }
        else
        {
            _isTileChoosen = true;
            _choosenTile = choosen;
        }
    }

    public void SwitchPaintOption() => _isMouseCanPaint = !_isMouseCanPaint;

    public bool PossibleLevel()
    {
        if (!StartAndFinishCheck(out List<List<bool>> map, out Vector2Int startPos, out Vector2Int endPos)) return false;
        if (!Astar.LevelEditorPathExists(startPos, endPos, map)) return false;
        return true;
    }

    private bool StartAndFinishCheck(out List<List<bool>> map, out Vector2Int startPos, out Vector2Int endPos)
    {
        map = new();
        startPos = new();
        endPos = new();
        bool foundStart = false;
        bool foundFinish = false;
        for (int i = -1; i <= _height; i++)
        {
            map.Add(new());
            for (int j = -1; j <= _width; j++)
            {
                TileBase current = tilemap.GetTile(tilemap.WorldToCell(new(j, i)));
                if (current == unbreakableWallTile) map[i + 1].Add(false);
                else
                {
                    if (current == startTile)
                    {
                        if (foundStart) return false;
                        foundStart = true;
                        startPos = new(j, i);
                    }
                    else if (current == finishTile)
                    {
                        if (foundFinish) return false;
                        foundFinish = true;
                        endPos = new(j, i);
                    }
                    map[i + 1].Add(true);
                }
            }
        }
        if (!foundStart || !foundFinish) return false;
        int floorNeighbour = 0;
        if (tilemap.GetTile(tilemap.WorldToCell(new(startPos.x + 1, startPos.y))) == floorTile) floorNeighbour++;
        if (tilemap.GetTile(tilemap.WorldToCell(new(startPos.x - 1, startPos.y))) == floorTile) floorNeighbour++;
        if (tilemap.GetTile(tilemap.WorldToCell(new(startPos.x, startPos.y + 1))) == floorTile) floorNeighbour++;
        if (tilemap.GetTile(tilemap.WorldToCell(new(startPos.x, startPos.y - 1))) == floorTile) floorNeighbour++;
        if (floorNeighbour < 2) return false;
        return true;
    }

    public void SaveLevel(string name, int difficulty)
    {
        List<int> tilesID = new();
        List<Vector2Int> positions = new();
        for (int i = -1; i <= _height; i++)
        {
            for (int j = -1; j <= _width; j++)
            {
                Vector2Int curPos = new(j, i);
                tilesID.Add((int)_tileTransformer.TilebaseToTile(tilemap.GetTile(tilemap.WorldToCell((Vector2)curPos))));
                positions.Add(curPos);
            }
        }
        SaveManager.SaveLevel(_height, _width, name, difficulty, tilesID, positions);
    }
}
