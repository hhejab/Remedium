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
            Enemy_AI enemy = col.GetComponentInParent<Enemy_AI>();
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