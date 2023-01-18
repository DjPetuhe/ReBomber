using UnityEngine;
using System.Collections.Generic;

public class GameStateData
{
    public int Score;
    public float Time;
    public int Health;
    public int BombAmount;
    public int ExplosionSize;
    public float PlayerSpeed;
    public Vector2Int LeftTopCoords;
    public Vector2Int RightBottomCoords;
    public int SceneID;
    public string Name;
    public List<int> TilesID;
    public List<Vector2Int> TilesPosition;
    public List<int> EntityID;
    public List<Vector2> EntityPosition;

    public GameStateData() { }

}
