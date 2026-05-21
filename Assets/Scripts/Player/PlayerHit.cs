using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    [Header("Fallback only if PlayerStats is missing")]
    public int baseDamage = 1;

    private bool hasHit = false;
    private PlayerStats playerStats;

    private void Awake()
    {
        playerStats = GetComponentInParent<PlayerStats>();
    }

    private void OnEnable()
    {
        hasHit = false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (hasHit)
            return;

        int damageToDeal = GetDamage();

        Debug.Log("Sword touched: " + col.name);

        // 1. Damage normal enemies first.
        // Goblin_AI, Slime_AI, Skeleton_AI, Zombie_AI all inherit from Enemy_AI.
        Enemy_AI enemy = col.GetComponentInParent<Enemy_AI>();

        if (enemy != null)
        {
            Debug.Log("Player hit enemy/goblin for: " + damageToDeal);
            enemy.TakeDamage(damageToDeal);
            hasHit = true;
            return;
        }

        // 2. Damage boss after enemies.
        BossHealth bossHealth = col.GetComponentInParent<BossHealth>();

        if (bossHealth != null)
        {
            Debug.Log("Player hit boss for: " + damageToDeal);
            bossHealth.TakeDamage(damageToDeal);
            hasHit = true;
            return;
        }
    }

    private int GetDamage()
    {
        if (playerStats != null)
            return playerStats.GetFinalAttackDamage();

        return baseDamage;
    }
}