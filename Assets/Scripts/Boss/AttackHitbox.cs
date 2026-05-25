using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 15;

    private bool hasHit;

    private void OnEnable()
    {
        hasHit = false;
        Debug.Log(gameObject.name + " attack hitbox enabled");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHitPlayer(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryHitPlayer(other);
    }

    private void TryHitPlayer(Collider2D other)
    {
        if (hasHit) return;

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
        Movement movement = other.GetComponentInParent<Movement>();

        if (playerHealth == null)
            return;

        playerHealth.ChangeHealth(-damage);
        hasHit = true;

        Debug.Log("Boss hit player for: " + damage);

        if (movement != null)
        {
            movement.TriggerHurt(0.3f);
        }
    }
}