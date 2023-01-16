using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Auxilliary;

public static class Astar
{
    private static Vector2Int s_playerPos;
    private static TilemapManager s_tilemap;
    public static List<Cell> Route { get; private set; } = new();

    public static void EvaluateNewPathes()
    {
        PlayerMovement playerMove = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        EvaluateNewPathes(playerMove.CellPosition);
    }

    public static void EvaluateNewPathes((int, int) playerCell)
    {
        s_tilemap = GameObject.FindGameObjectWithTag("TilemapManager").GetComponent<TilemapManager>();
        s_playerPos = new(playerCell.Item2, playerCell.Item1);
        Route.Clear();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            EnemyMovement enemyScript = enemy.GetComponent<EnemyMovement>();
            enemyScript.Recheck = true;
            AddPathToRoute(enemyScript.Position, s_playerPos);
        }
    }

    public static void AddPathToRoute(Vector2Int startingPoint, Vector2Int endingPoint)
    {
        if (Route.Any(c => c.Y == startingPoint.y && c.X == startingPoint.x)) return;
        List<Cell> nodes = new() { new(startingPoint.y, startingPoint.x, (endingPoint.y, endingPoint.x)) };
        List<(int, int)> visited = new();
        Cell last = new();
        bool finished = false;
        while (nodes.Count > 0 && !finished)
        {
            Cell current = nodes.OrderBy(x => x.G + x.H).First();
            visited.Add((current.Y, current.X));
            nodes.Remove(current);
            List<Cell> neighbours = GetUnmarkedNeighbors(current, visited);
            List<Cell> neighboursInRoute = neighbours.Where(n => Route.Any(c => c.Y == n.Y && c.X == n.X)).ToList();
            if (neighboursInRoute.Count > 0)
            {
                finished = true;
                Cell neighborInRoute = Route.Where(c => neighboursInRoute.Any(n => n.X == c.X && n.Y == c.Y))
                                        .OrderBy(c => c.G + c.H)
                                        .FirstOrDefault();
                current.Child = neighborInRoute;
                if (current.Parent is not null) current.Parent.Child = current;
                last = current;
                break;
            }
            foreach (var neighbour in neighbours)
            {
                if (neighbour.Y == endingPoint.y && neighbour.X == endingPoint.x)
                {
                    finished = true;
                    current.Child = neighbour;
                    last = neighbour;
                    break;
                }
                if (IsNeededToAdd(neighbour, nodes)) nodes.Add(neighbour);
            }
        }
        AddToRoute(finished, last);
    }

    private static bool IsNeededToAdd(Cell neighbour, List<Cell> nodes)
    {
        foreach (var node in nodes)
        {
            if ((neighbour.Y, neighbour.X) == (node.Y, node.X))
            {
                if ((neighbour.H + neighbour.G) < (node.H + node.G))
                {
                    node.H = neighbour.H;
                    node.G = neighbour.G;
                    node.Parent = neighbour.Parent;
                }
                return false;
            }
        }
        return true;
    }

    private static void AddToRoute(bool finished, Cell last)
    {
        if (finished)
        {
            Cell current = last;
            Route.Add(current);
            while (current.Parent is not null)
            {
                current = current.Parent;
                if (current.Parent is not null) current.Parent.Child = current;
                Route.Add(current);
            }
        }
    }

    private static List<Cell> GetUnmarkedNeighbors(Cell curr, List<(int, int)> visited)
    {
        List<(int, int)> directions = new();
        Vector2Int cord = s_tilemap.PositionToMapIndexes(new(curr.X, curr.Y));
        if (s_tilemap.IsFineCoords(cord.y - 1, cord.x)) directions.Add((-1, 0));
        if (s_tilemap.IsFineCoords(cord.y, cord.x - 1)) directions.Add((0, -1));
        if (s_tilemap.IsFineCoords(cord.y + 1, cord.x)) directions.Add((1, 0));
        if (s_tilemap.IsFineCoords(cord.y, cord.x + 1)) directions.Add((0, 1));
        if (s_tilemap.IsFineCoords(cord.y - 1, cord.x - 1) && directions.Contains((-1, 0)) && directions.Contains((0, -1)))
        {
            directions.Add((-1, -1));
        }
        if (s_tilemap.IsFineCoords(cord.y - 1, cord.x + 1) && directions.Contains((-1, 0)) && directions.Contains((0, 1)))
        {
            directions.Add((-1, 1));
        }
        if (s_tilemap.IsFineCoords(cord.y + 1, cord.x + 1) && directions.Contains((1, 0)) && directions.Contains((0, 1)))
        {
            directions.Add((1, 1));
        }
        if (s_tilemap.IsFineCoords(cord.y + 1, cord.x - 1) && directions.Contains((1, 0)) && directions.Contains((0, -1)))
        {
            directions.Add((1, -1));
        }
        List<Cell> neighbors = new();
        foreach (var dir in directions)
        {
            if (visited.Contains((curr.Y + dir.Item1, curr.X + dir.Item2))) continue;
            neighbors.Add(new Cell(curr.Y + dir.Item1, curr.X + dir.Item2, (s_playerPos.y, s_playerPos.x), curr));
        }
        return neighbors;
    }

    public static Vector2Int MoveDirection(Vector2Int position)
    {
        if (PathExists(position))
        {
            Cell slimeCell = Route.Where(c => c.X == position.x && c.Y == position.y).FirstOrDefault();
            if (slimeCell.Child is not null)
            {
                return new()
                {
                    y = slimeCell.Child.Y - slimeCell.Y,
                    x = slimeCell.Child.X - slimeCell.X
                };
            }
        }
        return new(0, 0);
    }

    public static bool PathExists(Vector2Int position)
    {
        Cell current = Route.Where(c => c.X == position.x && c.Y == position.y).FirstOrDefault();
        while (current is not null)
        {
            if (current.X == s_playerPos.x && current.Y == s_playerPos.y) return true;
            current = current.Child;
        }
        return false;
    }

    public static bool LevelEditorPathExists(Vector2Int startingPoint, Vector2Int endingPoint, List<List<bool>> map)
    {
        List<Cell> nodes = new() { new(startingPoint.y + 1, startingPoint.x + 1, (endingPoint.y + 1, endingPoint.x + 1)) };
        List<(int, int)> visited = new();
        Cell last = new();
        bool finished = false;
        while (nodes.Count > 0 && !finished)
        {
            Cell current = nodes.OrderBy(x => x.G + x.H).First();
            visited.Add((current.Y, current.X));
            nodes.Remove(current);
            List<Cell> neighbours = GetNeighbors(current, map, visited, endingPoint);
            foreach (var neighbour in neighbours)
            {
                if (neighbour.Y == endingPoint.y + 1 && neighbour.X == endingPoint.x + 1) return true;
                if (IsNeededToAdd(neighbour, nodes)) nodes.Add(neighbour);
            }
        }
        return finished;
    }

    public static List<Cell> GetNeighbors(Cell curr, List<List<bool>> map, List<(int, int)> visited, Vector2Int endingPoint)
    {
        List<(int, int)> directions = new();
        if (curr.Y - 1 >= 0 && map[curr.Y - 1][curr.X]) directions.Add((-1, 0));
        if (curr.X - 1 >= 0 && map[curr.Y][curr.X - 1]) directions.Add((0, -1));
        if (curr.Y + 1 < map.Count && map[curr.Y + 1][curr.X]) directions.Add((1, 0));
        if (curr.X + 1 < map[curr.Y].Count && map[curr.Y][curr.X + 1]) directions.Add((0, 1));
        List<Cell> neighbors = new();
        foreach (var dir in directions)
        {
            if (visited.Contains((curr.Y + dir.Item1, curr.X + dir.Item2))) continue;
            neighbors.Add(new Cell(curr.Y + dir.Item1, curr.X + dir.Item2, (endingPoint.y + 1, endingPoint.x + 1), curr));
        }
        return neighbors;
    }
}
