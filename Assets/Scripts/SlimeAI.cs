using UnityEngine;
using System.Collections;

public class SlimeAI : MonoBehaviour
{
    [Header("Settings")]
    public Transform player;
    public float speed = 2f;
    public float attackRange = 1.5f;
    public float aggroRange = 5f;
    public int health = 30;
    
    [Header("References")]
    public GameObject hitbox;
    public Animator animator;
    private Rigidbody2D rb;
    private bool isDead = false;

    void Start() 
{ 
    rb = GetComponent<Rigidbody2D>(); 
    
    // Safety check: Find components if you forgot to drag them in
    if (animator == null) animator = GetComponent<Animator>();
    if (player == null) 
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        else Debug.LogError("Slime cannot find the Player! Check the Tag.");
    }
}

    void Update()
    {
        if (isDead) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackRange) { Attack(); }
        else if (dist <= aggroRange) { MoveTowardsPlayer(); }
        else { StopMoving(); }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
        animator.SetBool("isMoving", true);
        animator.SetFloat("moveX", direction.x);
        animator.SetFloat("moveY", direction.y);
    }

    void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isMoving", false);
    }

    void Attack()
    {
        // Only attack if not already attacking
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) return;

        StopMoving();
        animator.SetTrigger("isAttacking");
        StartCoroutine(TimedHitbox());
    }

    IEnumerator TimedHitbox()
    {
        yield return new WaitForSeconds(0.50f); // Swing start
        if (!isDead) hitbox.SetActive(true);
        yield return new WaitForSeconds(0.25f); // Duration of swing
        hitbox.SetActive(false);
    }

    public float damageCooldown = 0.25f;
    private float lastDamageTime = -999f;

    public void TakeDamage(int amount)
    {
        Debug.Log($"Slime TakeDamage({amount}) at time {Time.time}", this);
        
        if (isDead) return;

        if (Time.time < lastDamageTime + damageCooldown)
            return;

        lastDamageTime = Time.time;

        health -= amount;

        if (health > 0)
            animator.SetTrigger("Hurt");
        else
            Die();
    }

    void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isDead", true);
        GetComponent<Collider2D>().enabled = false;
        hitbox.SetActive(false);
    }
}