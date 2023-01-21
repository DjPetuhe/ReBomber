using System;
using System.Linq;
using System.Collections.Generic;
using Tile = TilemapManager.Tile;
using Difficulty = DifficultyManager.Difficulty;

public class LevelGenerator
{
    private static readonly Random s_rng = new();
    private static readonly List<(int y, int x)> directions = new() { (-2, 0), (0, -2), (2, 0), (0, 2) };

    private int _width;
    private int _height;
    private List<List<int>> _matrix = new();
    private List<List<bool>> _visited = new();
    private Stack<(int y, int x)> _stack = new();
    private Difficulty _difficulty;

    private readonly int _roomAmount;
    private readonly int _circlesAmount;

    private const int MAX_ROOM_HEIGHT = 10;
    private const int MIN_ROOM_HEIGHT = 3;

    private const int MAX_ROOM_WIDTH = 10;
    private const int MIN_ROOM_WIDTH = 3;

    private const int MAX_CIRCLE_RADIUS = 7;
    private const int MIN_CIRCLE_RADIUS = 2;

    public LevelGenerator() { }

    public LevelGenerator(int height, int width, Difficulty dif)
    {
        if (height == 50) height--;
        else if (height % 2 == 0) height++;
        _height = height;
        if (width == 50) width--;
        else if (width % 2 == 0) width++;
        _width = width;
        int maxRoomAmount = (_height + _width + 2) / 10;
        int minRoomAmount = (_height + _width + 2) / 20;
        int maxCirclesAmount = (_height + _width + 2) / 5;
        int minCirclesAmount = (_height + _width + 2) / 10;
        _roomAmount = s_rng.Next() % (maxRoomAmount - minRoomAmount) + minRoomAmount;
        _circlesAmount = s_rng.Next() % (maxCirclesAmount - minCirclesAmount) + minCirclesAmount;
        _difficulty = dif;
    }

    public List<List<int>> MakeLevel()
    {
        SetStartingMatrix();
        SetMaze();
        SetRooms();
        SetRoundWalls();
        SetEnemies();
        SetStartAndFinish();
        return _matrix;
    }

    private void SetStartingMatrix()
    {
        _matrix.Clear();
        _visited.Clear();
        _stack.Clear();
        for (int i = 0; i < _height; i++)
        {
            _matrix.Add(new());
            _visited.Add(new());
            for (int j = 0; j < _width; j++)
            {
                if (i % 2 == 0 && j % 2 == 0) _matrix[i].Add((int)Tile.Floor);
                else if (s_rng.Next() % GetReplaceChance() == 0) _matrix[i].Add((int)Tile.BreakableWall);
                else _matrix[i].Add((int)Tile.UnbreakableWall);
                _visited[i].Add(false);
            }
        }
        _stack.Push((0, 0));
        _visited[0][0] = true;
    }

    private void SetMaze()
    {
        (int y, int x) previous = (-1, -1);
        while (_stack.Count > 0)
        {
            (int y, int x) current = _stack.Peek();
            if (previous != (-1, -1) && current != previous && _stack.Contains(previous))
            {
                (int y, int x) adjacent = ((current.y + previous.y) / 2, (current.x + previous.x) / 2);
                _visited[adjacent.y][adjacent.x] = true;
                _matrix[adjacent.y][adjacent.x] = (int)Tile.Floor;
            }
            AddNeighbours(current);
            previous = current;
        }
    }

    private void SetRooms()
    {
        for (int k = 0; k < _roomAmount; k++)
        {
            int roomHeight, roomWidth;
            if (MAX_ROOM_HEIGHT < _height / 3) roomHeight = s_rng.Next() % (MAX_ROOM_HEIGHT - MIN_ROOM_HEIGHT) + MIN_ROOM_HEIGHT;
            else roomHeight = s_rng.Next() % (_height / 3) + MIN_ROOM_HEIGHT;
            if (MAX_ROOM_WIDTH < _width / 3) roomWidth = s_rng.Next() % (MAX_ROOM_WIDTH - MIN_ROOM_WIDTH) + MIN_ROOM_WIDTH;
            else roomWidth = s_rng.Next() % (_width / 3) + MIN_ROOM_WIDTH;
            (int y, int x) roomCenter = (s_rng.Next() % _height, s_rng.Next() % _width);
            for (int i = roomCenter.y - roomHeight / 2; i <= roomCenter.y + roomHeight / 2; i++)
            {
                if (i < 0 || i >= _height) continue;
                for (int j = roomCenter.x - roomWidth / 2; j <= roomCenter.x + roomWidth / 2; j++)
                {
                    if (j < 0 || j >= _width) continue;
                    _matrix[i][j] = (int)Tile.Floor;
                }
            }
        }
    }

    private void AddNeighbours((int y, int x) current)
    {
        List<(int y, int x)> shuffeledDir = directions.OrderBy(d => s_rng.Next()).ToList();
        foreach(var dir in shuffeledDir)
        {
            (int y, int x) currPos = (current.y + dir.y, current.x + dir.x);
            if (currPos.y < 0 || currPos.y >= _height) continue;
            if (currPos.x < 0 || currPos.x >= _width) continue;
            if (_visited[currPos.y][currPos.x]) continue;
            if (_stack.Contains(currPos)) continue;
            _stack.Push(currPos);
            _visited[currPos.y][currPos.x] = true;
            return;
        }
        _stack.Pop();
    }

    private void SetRoundWalls()
    {
        for (int k = 0; k < _circlesAmount; k++)
        {
            int circleRadius;
            if (MAX_CIRCLE_RADIUS < (_height + _width) / 6) circleRadius = s_rng.Next() % (MAX_CIRCLE_RADIUS - MIN_CIRCLE_RADIUS) + MIN_CIRCLE_RADIUS;
            else circleRadius = s_rng.Next() % (((_height + _width) / 6) - MIN_CIRCLE_RADIUS) + MIN_CIRCLE_RADIUS;
            (int y, int x) circleCenter = (s_rng.Next() % _height, s_rng.Next() % _width);
            for (int i = circleCenter.y - circleRadius; i <= circleCenter.y + circleRadius; i++)
            {
                if (i < 0 || i >= _height) continue;
                for (int j = circleCenter.x - circleRadius; j <= circleCenter.x + circleRadius; j++)
                {
                    if (j < 0 || j >= _width) continue;
                    if (Math.Pow(circleCenter.y - i, 2) + Math.Pow(circleCenter.x - j, 2) > Math.Pow(circleRadius, 2)) continue;
                    if (_matrix[i][j] == (int)Tile.Floor) _matrix[i][j] = (int)Tile.BreakableWall;
                }
            }
        }
    }

    private void SetStartAndFinish()
    {
        (int y, int x) startPos = (s_rng.Next() % _height, s_rng.Next() % _width);
        (int y, int x) finPos = (s_rng.Next() % _height, s_rng.Next() % _width);
        while (Math.Abs(finPos.y - startPos.y) < _height / 2 - 1 && Math.Abs(finPos.x - startPos.x) < _width / 2 - 1)
        {
            finPos = (s_rng.Next() % _height, s_rng.Next() % _width);
        }
        SetTiles(startPos, true);
        SetTiles(finPos, false);
    }

    private void SetTiles((int y, int x) pos, bool start)
    {
        for (int i = pos.y - 1; i <= pos.y + 1; i++)
        {
            if (i < 0 || i >= _height) continue;
            for (int j = pos.x - 1; j <= pos.x + 1; j++)
            {
                if (j < 0 || j >= _width) continue;
                if (i == pos.y && j == pos.x)
                {
                    if (start) _matrix[i][j] = (int)Tile.Start;
                    else _matrix[i][j] = (int)Tile.Finish;
                }
                else
                {
                    if (start) _matrix[i][j] = (int)Tile.Floor;
                    else _matrix[i][j] = (int)Tile.BreakableWall;
                }
            }
        }
    }

    private void SetEnemies()
    {
        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                if (_matrix[i][j] == (int)Tile.Floor && s_rng.Next() % GetEnemiesChance() == 0) _matrix[i][j] = EnemyByDifficulty();
            }
        }
    }

    private int GetReplaceChance()
    {
        return _difficulty switch
        {
            Difficulty.Medium => 30,
            Difficulty.Hard => 40,
            _ => 20
        };
    }

    private int GetEnemiesChance()
    {
        return _difficulty switch
        {
            Difficulty.Medium => 40,
            Difficulty.Hard => 30,
            _ => 50
        };
    }

    private int EnemyByDifficulty()
    {
        return s_rng.Next() % 2 + _difficulty switch
        {
            Difficulty.Medium => (int)Tile.YellowSlime,
            Difficulty.Hard => (int)Tile.PurpleSlime,
            _ => (int)Tile.BlueSlime
        };
    }
}
