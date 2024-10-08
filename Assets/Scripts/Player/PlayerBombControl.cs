using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameState = GameManager.GameState;
using PlayerState = PlayerHealthControl.PlayerState;

public class PlayerBombControl : MonoBehaviour
{
    [Header("Colliders")]
    [SerializeField] Collider2D playerCollider;

    [Header("Prefabs")]
    [SerializeField] GameObject bombPrefab;
    [SerializeField] GameObject explosionPrefab;

    [Header("Keys")]
    [SerializeField] KeyCode placeBombKey = KeyCode.Space;

    [Header("Characteristics")]
    [Range(0f, 10f)]
    [SerializeField] float fuseTimeSeconds = 5f;

    private GameManager _gameManager;
    private TilemapManager _tilemapManager;
    private PlayerHealthControl _playerHealthControl;
    private List<(GameObject, Vector2Int)> _bombs = new();

    private int _bombsLeft;

    private void OnEnable()
    {
        _tilemapManager = GameObject.Find("Map").GetComponent<TilemapManager>();
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _playerHealthControl = GetComponent<PlayerHealthControl>();
        _bombsLeft = _gameManager.BombsCount;
    }

    private void Update()
    {
        if (_playerHealthControl.State == PlayerState.Dead) return;
        if (_gameManager.State == GameState.Pause || _gameManager.State == GameState.GameEnd) return;
        if (_bombs.Count > 0) MarkBombsOnMap();
        if (Input.GetKeyDown(placeBombKey) && _bombsLeft > 0) StartCoroutine(PlaceBomb());
    }
    
    private IEnumerator PlaceBomb()
    {
        Vector2 offset = playerCollider.offset;
        Vector2 pos = new (Mathf.Round(transform.position.x + offset.x), Mathf.Round(transform.position.y + offset.y));
        Vector2Int intPos = new(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
        if (_bombs.Any(b => b.Item2 == pos)) yield break;
        GameObject bomb = Instantiate(bombPrefab, pos, Quaternion.identity);
        _bombs.Add((bomb, intPos));
        _tilemapManager.MarkBombOnMap(intPos);
        _bombsLeft--;
        yield return new WaitForSeconds(fuseTimeSeconds);
        _bombsLeft++;
        _bombs.RemoveAll(b => b.Item1 == bomb);
        pos = new(Mathf.Round(bomb.transform.position.x), Mathf.Round(bomb.transform.position.y));
        intPos = new(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
        _tilemapManager.RemoveBombFromMap(intPos);
        Destroy(bomb);
        GameObject explosion = Instantiate(explosionPrefab, pos, Quaternion.identity);
        ExplosionController explosionScript = explosion.GetComponent<ExplosionController>();
        explosionScript.ExplosionSize = _gameManager.ExplosionSize;
        explosionScript.Depth = 0;
        explosionScript.Explode(_tilemapManager);
    }

    private void MarkBombsOnMap()
    {
        for (int i = 0; i < _bombs.Count; i++)
        {
            Vector2Int pos = new()
            {
                y = Mathf.RoundToInt(_bombs[i].Item1.transform.position.y),
                x = Mathf.RoundToInt(_bombs[i].Item1.transform.position.x)
            };
            if (_bombs[i].Item2 != pos)
            {
                _tilemapManager.RemoveBombFromMap(_bombs[i].Item2);
                _tilemapManager.MarkBombOnMap(pos);
                _bombs[i] = new(_bombs[i].Item1, pos);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D bombCollider)
    {
        if (bombCollider.gameObject.CompareTag("Bomb")) bombCollider.isTrigger = false; 
    }

    public void AddBomb()
    {
        if (_gameManager.BombsCount < GameManager.MAX_BOMB)
        {
            _gameManager.BombsCount++;
            _bombsLeft++;
        }
    }

    public void ExpandExplosion()
    {
        if (_gameManager.ExplosionSize >= GameManager.MAX_EXPLOSION) return;
        _gameManager.ExplosionSize++;
    }
}
