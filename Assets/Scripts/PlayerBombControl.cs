using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerBombControl : MonoBehaviour
{
    [SerializeField]
    private Collider2D playerCollider;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject bombPrefab;
    [SerializeField]
    private GameObject explosionPrefab;

    [Header("Keys")]
    [SerializeField]
    private KeyCode placeBombKey = KeyCode.Space;

    [Header("Characteristics")]
    [SerializeField]
    private int bombCount = 1;
    [SerializeField]
    private int bombsLeft;
    [Range(2f,10f)]
    [SerializeField]
    private int explosionSize = 2;
    [Range(0f,10f)]
    [SerializeField]
    private float fuseTimeSeconds = 5f;

    private void OnEnable()
    {
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
        explosionScript.Explode();
    }

    private void OnTriggerExit2D(Collider2D bombCollider)
    {
        if (bombCollider.gameObject.CompareTag("Bomb")) bombCollider.isTrigger = false; 
    }
}
