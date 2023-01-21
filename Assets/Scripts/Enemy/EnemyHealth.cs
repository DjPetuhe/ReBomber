using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Particle System prefab")]
    [SerializeField] GameObject deathParticlesPrefab;

    [field:Header("ID")]
    [field:SerializeField] public int ID { get; set; }

    private bool _died = false;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (_died) return;
        if (collider.gameObject.CompareTag("Explosion"))
        {
            Destroy(gameObject);
            GameObject particles = Instantiate(deathParticlesPrefab, gameObject.transform.position, Quaternion.identity);
            particles.GetComponent<ParticleSystem>().Play();
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().Score += 100;
            _died = true;
        }
    }
}
