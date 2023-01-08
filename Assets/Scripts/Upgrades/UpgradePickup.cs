using UnityEngine;

public class UpgradePickup : MonoBehaviour
{
    public enum UpgradeType
    {
        Blast = 0,
        Bomb = 1,
        Hp = 2,
        Speed = 3
    }

    [Header("Upgrade Type")]
    [SerializeField] UpgradeType type;

    private void OnTriggerEnter2D(Collider2D playerCollider)
    {
        if (!playerCollider.CompareTag("Player")) return;
        GameObject player = playerCollider.gameObject;
        switch (type)
        {
            case UpgradeType.Blast:
                player.GetComponent<PlayerBombControl>().ExpandExplosion();
                break;
            case UpgradeType.Bomb:
                player.GetComponent<PlayerBombControl>().AddBomb();
                break;
            case UpgradeType.Hp:
                player.GetComponent<PlayerHealthControl>().RestoreHealth();
                break;
            case UpgradeType.Speed:
                player.GetComponent<PlayerMovement>().SpeedUpBy(0.5f);
                break;
        }
        Destroy(gameObject);
    }
}
