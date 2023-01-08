using UnityEngine;
using System.Collections;

public class PlayerHealthControl : MonoBehaviour
{
    public enum PlayerState
    {
        Normal = 0,
        Invincible = 1,
        Dead = 2
    }

    [Header("Sprite Renderer")]
    [SerializeField] SpriteRenderer spriteRenderer;

    [Header("Particle System prefab")]
    [SerializeField] GameObject deathParticlesPrefab;

    private GameManager _gameManager;
    private int _triggeredEnemies = 0;

    private static Color s_damageColor = new(255, 0, 0);
    private static Color s_defaultColor = new(255, 255, 255);

    private const int BLINKING_AMOUNT = 3;
    private const float BLINKING_TIME = 0.25f;
    private const float BLINKING_GAP_TIME = 0.25f;
    private const float DEATH_ANIMATION_TIME = 1f;
    public PlayerState State { get; private set; } = PlayerState.Normal;

    private void OnEnable() => _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Explosion"))
        {
            _triggeredEnemies++;
            if (State != PlayerState.Normal) return;
            if (_gameManager.Health != 0) StartCoroutine(InvincibleFrames());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Explosion")) _triggeredEnemies--;
    }

    private IEnumerator InvincibleFrames()
    {
        _gameManager.Health--;
        State = PlayerState.Invincible;
        for (int i = 0; i < BLINKING_AMOUNT; i++)
        {
            spriteRenderer.color = s_damageColor;
            yield return new WaitForSeconds(BLINKING_TIME);
            spriteRenderer.color = s_defaultColor;
            if (i != BLINKING_AMOUNT - 1) yield return new WaitForSeconds(BLINKING_GAP_TIME);
        }
        State = PlayerState.Normal;
        if (_triggeredEnemies != 0) StartCoroutine(InvincibleFrames());
    }

    public IEnumerator Death()
    {
        State = PlayerState.Dead;
        spriteRenderer.color = s_damageColor;
        GetComponent<PlayerMovement>().StopMovement();
        yield return new WaitForSeconds(DEATH_ANIMATION_TIME);
        Destroy(gameObject);
        GameObject particles = Instantiate(deathParticlesPrefab, gameObject.transform.position, Quaternion.identity);
        particles.GetComponent<ParticleSystem>().Play();
    }

    public void RestoreHealth() => _gameManager.Health++;
}
