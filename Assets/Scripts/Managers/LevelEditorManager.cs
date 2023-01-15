using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using Tile = TilemapManager.Tile;

//TODO: call generate function and print map, check if level is possible to finish,
public class LevelEditorManager : MonoBehaviour
{
    [Header("Line")]
    [SerializeField] GameObject line;

    [Header("Tilemaps")]
    [SerializeField] Tilemap tilemap;
    [SerializeField] Tilemap SelectionTilemap;

    [Header("Tiles")]
    [SerializeField] TileBase floorTile;
    [SerializeField] TileBase breakableWallTile;
    [SerializeField] TileBase unbreakableWallTile;
    [SerializeField] TileBase finishTile;
    [SerializeField] TileBase startTile;
    [SerializeField] TileBase redSlimeTile;
    [SerializeField] TileBase purpleSlimeTile;
    [SerializeField] TileBase yellowSlimeTile;
    [SerializeField] TileBase blueSlimeTile;

    private TileBase _choosenTile;
    private LineRenderer _lineRenderer;
    private PolygonCollider2D _cameraBounds;
    private CameraMovement _cameraMovement;

    private bool _isTileChoosen = false;
    private bool _isMouseCanPaint = true;
    private Vector2Int? _choosningTilePos;
    private int _height;
    private int _width;

    private const float OFFSET = -0.5f;
    private const int CAMERA_PADDING = 5;

    private void Start()
    {
        _lineRenderer = line.GetComponent<LineRenderer>();
        _cameraBounds = line.GetComponent<PolygonCollider2D>();
        _cameraMovement = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CameraMovement>();
        ResizeMap(10, 10);
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
                    SelectionTilemap.SetTile(SelectionTilemap.WorldToCell((Vector3Int)_choosningTilePos), null);
                _choosningTilePos = currentPos;
                SelectionTilemap.SetTile(SelectionTilemap.WorldToCell((Vector3Int)_choosningTilePos), _choosenTile);
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
            SelectionTilemap.SetTile(SelectionTilemap.WorldToCell((Vector3Int)_choosningTilePos), null);
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
        TileBase choosen = TileToTileBase(tile);
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

    private TileBase TileToTileBase(Tile tile)
    {
        return tile switch
        {
            Tile.BreakableWall => breakableWallTile,
            Tile.UnbreakableWall => unbreakableWallTile,
            Tile.Finish => finishTile,
            Tile.Start => startTile,
            Tile.RedSlime => redSlimeTile,
            Tile.PurpleSlime => purpleSlimeTile,
            Tile.YellowSlime => yellowSlimeTile,
            Tile.BlueSlime => blueSlimeTile,
            _ => floorTile
        };
    }

    public void SwitchPaintOption() => _isMouseCanPaint = !_isMouseCanPaint;
}
