using UnityEngine;
using UnityEngine.Tilemaps;
using Tile = TilemapManager.Tile;

public class TileTransformer : MonoBehaviour
{
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

    private static TileTransformer s_instance;

    private void Awake()
    {
        if (s_instance is null) s_instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public Tile TilebaseToTile(TileBase tile)
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

    public TileBase TileToTileBase(Tile tile)
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
}
