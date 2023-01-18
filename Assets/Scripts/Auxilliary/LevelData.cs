using UnityEngine;
using System.Collections.Generic;

public class LevelData
{
    public int Height;
    public int Width;
    public string Name;
    public int Difficulty;
    public List<int> TilesID;
    public List<Vector2Int> Position;

    public LevelData() { }

    public LevelData(int height, int width, string name, int difficulty, List<int> tilesID, List<Vector2Int> position)
    {
        Height = height;
        Width = width;
        Name = name;
        Difficulty = difficulty;
        TilesID = tilesID;
        Position = position;
    }
}
