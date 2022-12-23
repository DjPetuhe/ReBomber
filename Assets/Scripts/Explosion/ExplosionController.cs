using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    [Header("Layer Mask")]
    [SerializeField]
    private LayerMask explosionLayerMask;

    [Header("Prefab")]
    [SerializeField]
    private GameObject explosionPrefab;

    [Header("Animator")]
    [SerializeField]
    private Animator animator;

    [Header("Collider")]
    [SerializeField]
    private BoxCollider2D explosionCollider;

    private int _position;
    private int _depth;

    public int Depth
    {
        get { return _depth; }
        set 
        { 
            _depth = value >= ExplosionSize ? ExplosionSize : value;
            if (_depth == ExplosionSize - 1) _position = 2;
            else if (_depth == 0) _position = 0;
            else _position = 1;
        }
    }

    public int ExplosionSize { get; set; }

    public void Explode(TilemapManager map)
    {
        animator.SetInteger("Position", _position);
        List<(int, int)> directions = new() { (0, -1), (1, 0), (0, 1), (-1, 0) };
        for (int i = 1; i < ExplosionSize; i++)
        {
            for (int j = 0; j < directions.Count; j++)
            {
                Vector2 pos = new(Mathf.Round(transform.position.x + directions[j].Item1 * i),
                                  Mathf.Round(transform.position.y + directions[j].Item2 * i));
                if (Physics2D.OverlapBox(pos, Vector2.one / 2f, 0f, explosionLayerMask))
                {
                    map.DestroyWall(pos);
                    directions.Remove(directions[j]);
                    j--;
                }
                else
                {
                    GameObject explosion = Instantiate(explosionPrefab, pos, ExplosionRotation(directions[j]));
                    ExplosionController explosionScript = explosion.GetComponent<ExplosionController>();
                    explosionScript.ExplosionSize = ExplosionSize;
                    explosionScript.Depth = Depth + i;
                    explosionScript.ExplosionSetUp();
                }
            }
        }
    }

    private Quaternion ExplosionRotation((int, int) direction)
    {
        return direction switch
        {
            (1, 0) => Quaternion.Euler(0, 0, 90),
            (0, 1) => Quaternion.Euler(0, 0, 180),
            (-1, 0) => Quaternion.Euler(0, 0, 270),
            _ => Quaternion.identity
        };
    }

    public void ExplosionSetUp()
    {
        animator.SetInteger("Position", _position);
        explosionCollider.size = _position switch
        {
            1 => new Vector2(0.6f, 1),
            2 => new Vector2(0.6f, 0.7f),
            _ => new Vector2(1f, 1f)
        };
    }
}
