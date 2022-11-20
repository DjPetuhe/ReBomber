using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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

    public void Explode()
    {
        animator.SetInteger("Position", _position);
        List<(int, int)> directions = new() { (0, -1), (1, 0), (0, 1), (-1, 0) };
        for (int i = 1; i < ExplosionSize; i++)
        {
            for (int j = 0; j < directions.Count; j++)
            {
                Vector2 pos = new(Mathf.Round(transform.position.x + directions[j].Item1 * i), Mathf.Round(transform.position.y + directions[j].Item2 * i));
                if (Physics2D.OverlapBox(pos, Vector2.one / 2f, 0f, explosionLayerMask))
                {
                    directions.Remove(directions[j]);
                    j--;
                }
                else
                {
                    GameObject explosion = Instantiate(explosionPrefab, pos, Quaternion.identity);
                    ExplosionController explosionScript = explosion.GetComponent<ExplosionController>();
                    explosionScript.ExplosionSize = ExplosionSize;
                    explosionScript.Depth = Depth + i;
                    explosionScript.ExplosionAdaptation(directions[j]);
                }
            }
        }
    }

    public void ExplosionAdaptation((int, int) direction)
    {
        animator.SetInteger("Position", _position);
        switch (_position)
        {
            case 1:
                explosionCollider.size = new Vector2(0.6f, 1);
                break;
            case 2:
                explosionCollider.size = new Vector2(0.6f, 0.7f);
                break;
            default:
                break;
        }
        switch (direction)
        {
            case (1, 0):
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case (0, 1):
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case (-1, 0):
                transform.rotation = Quaternion.Euler(0, 0, 270);
                break;
            default:
                break;
        }
    }
}
