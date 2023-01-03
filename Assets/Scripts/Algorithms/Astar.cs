using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Auxilliary;
using System.Linq;

public static class Astar
{
    private static bool s_firstTime { get; set; } = true;
    private static List<List<bool>> s_map { get; set; }
    private static Vector2Int s_playerPos {get; set; }
    public static List<Cell> Route { get; private set; } = new();

    public static void EvaluateNewPathes((int, int) playerCell)
    {
        TilemapManager tilemapManager = GameObject.FindGameObjectWithTag("TilemapManager").GetComponent<TilemapManager>();
        EvaluateNewPathes(tilemapManager.Map, playerCell);
    }


    public static void EvaluateNewPathes(List<List<bool>> map)
    {
        PlayerMovement playerMove = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        EvaluateNewPathes(map, playerMove.CellPosition);
    }

    public static void EvaluateNewPathes(List<List<bool>> map, (int, int) playerCell)
    {
        s_firstTime = true;
        s_map = map;
        s_playerPos = new(playerCell.Item2, playerCell.Item1);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            EnemyMovement enemyScript = enemy.GetComponent<EnemyMovement>();
            AddPathToRoute(enemyScript.Position, s_playerPos);
            enemyScript.Recheck = true;
            s_firstTime = false;
        }
    }

    private static void AddPathToRoute(Vector2Int startingPoint, Vector2Int endingPoint)
    {
        //TODO: Astar algorithm
    }

    public static Vector2Int MoveDirection(Vector2Int position)
    {
        if (PathExists(position))
        {
            Cell slimeCell = Route.Where(c => c.X == position.x && c.Y == position.y).FirstOrDefault();
            if (slimeCell is not null && slimeCell.Child is not null)
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

    private static bool PathExists(Vector2Int position)
    {
        Cell current = Route.Where(c => c.X == position.x && c.Y == position.y).FirstOrDefault();
        while (current is not null && current.Child is not null)
        {
            if (current.Child.X == s_playerPos.x && current.Child.Y == s_playerPos.y) return true;
            current = current.Child;
        }
        return false;
    }
}
