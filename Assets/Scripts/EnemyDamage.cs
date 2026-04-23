using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damage = 10;
    private float lastDamageTime;
    private float cooldown = 0.5f; // Cannot hit again for 0.5 seconds

    void OnTriggerEnter2D(Collider2D col)
    {
        // 1. Check if it's the player
        if (col.CompareTag("Player"))
        {
            // 2. Check if the cooldown has passed
            if (Time.time > lastDamageTime + cooldown)
            {
                col.GetComponent<PlayerHealth>().ChangeHealth(-damage);
                lastDamageTime = Time.time; // Reset the timer
                
                // Optional: Deactivate immediately after successful hit
                gameObject.SetActive(false); 
            }
        }
    }
}