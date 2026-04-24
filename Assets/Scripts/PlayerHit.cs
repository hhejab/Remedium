using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    public int damage = 1;
    private bool hasHit = false;

    void OnEnable() => hasHit = false;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (hasHit) return;

        if (col.CompareTag("Enemy"))
        {
            SlimeAI enemy = col.GetComponentInParent<SlimeAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                hasHit = true;
            }
        }
    }
}