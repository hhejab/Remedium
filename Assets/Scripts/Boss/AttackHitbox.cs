using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
public int damage = 15;

    private bool hasHit;

    private void OnEnable()
    {
        hasHit = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            Movement movement = other.GetComponent<Movement>();

            if (playerHealth != null)
            {
                playerHealth.ChangeHealth(-damage);
                hasHit = true;
            }

            if (movement != null)
            {
                movement.TriggerHurt(0.3f);
            }
        }
    }
}
