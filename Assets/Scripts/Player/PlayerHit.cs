using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    public int baseDamage = 1;
    private bool hasHit = false;
    private PlayerStats playerStats;

    private void Awake()
    {
        playerStats = GetComponentInParent<PlayerStats>();
    }

    void OnEnable() => hasHit = false;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (hasHit) return;

        if (col.CompareTag("Enemy"))
        {
            SlimeAI enemy = col.GetComponentInParent<SlimeAI>();
            if (enemy != null)
            {
                int damageToDeal = baseDamage;

                if (playerStats != null)
                    damageToDeal = playerStats.GetFinalAttackDamage();
                    
                enemy.TakeDamage(damageToDeal);
                hasHit = true;
            }
        }
    }
}