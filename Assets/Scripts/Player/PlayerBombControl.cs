using UnityEngine;
using System.Collections;

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
    [SerializeField] int bombCount = 1;
    [SerializeField] int bombsLeft;
    [Range(0f, 10f)]
    [SerializeField] float fuseTimeSeconds = 5f;
    [Range(2f,10f)]
    [SerializeField] int explosionSize = 2;

    private LevelManager _levelManagerScript;
    private TilemapManager _tilemapManagerScript;

    public int ExplosionSize
    {
        get { return explosionSize; }
        set { if (value > 2 && value < 10) explosionSize = value; }
    }

    private void OnEnable()
    {
        _levelManagerScript = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        _tilemapManagerScript = GameObject.Find("Map").GetComponent<TilemapManager>();
        if (_levelManagerScript.OriginalLevel)
        {
             //TODO: Load bombsCount and explosionSize from default   
        }
        bombsLeft = bombCount;
    }

    private void Update()
    {
        if (Input.GetKeyDown(placeBombKey) && bombsLeft > 0) StartCoroutine(PlaceBomb());
    }

    private IEnumerator PlaceBomb()
    {
        Vector2 offset = playerCollider.offset;
        Vector2 pos = new (Mathf.Round(transform.position.x + offset.x), Mathf.Round(transform.position.y + offset.y));
        GameObject bomb = Instantiate(bombPrefab, pos, Quaternion.identity);
        bombsLeft--;
        yield return new WaitForSeconds(fuseTimeSeconds);
        Destroy(bomb);
        bombsLeft++;
        pos = new(Mathf.Round(bomb.transform.position.x), Mathf.Round(bomb.transform.position.y));
        GameObject explosion = Instantiate(explosionPrefab, pos, Quaternion.identity);
        ExplosionController explosionScript = explosion.GetComponent<ExplosionController>();
        explosionScript.ExplosionSize = explosionSize;
        explosionScript.Depth = 0;
        explosionScript.Explode(_tilemapManagerScript);
    }

    private void OnTriggerExit2D(Collider2D bombCollider)
    {
        if (bombCollider.gameObject.CompareTag("Bomb")) bombCollider.isTrigger = false; 
    }

    public void AddBomb()
    {
        if (bombCount < 5)
        {
            bombCount++;
            bombsLeft++;
        }
    }
}
