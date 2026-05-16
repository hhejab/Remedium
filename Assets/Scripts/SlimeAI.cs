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
        StartCoroutine(PlayDeathAndDespawn());
    }

    IEnumerator PlayDeathAndDespawn()
    {
        if (animator == null)
        {
            Destroy(gameObject);
            yield break;
        }

        // Ensure the animator enters the dead state
        animator.SetBool("isDead", true);

        // Try to find a death clip in the animator controller
        string deathClipName = null;
        RuntimeAnimatorController controller = animator.runtimeAnimatorController;
        if (controller != null)
        {
            foreach (var clip in controller.animationClips)
            {
                var n = clip.name.ToLower();
                if (n.Contains("death") || n.Contains("die") || n.Contains("dead"))
                {
                    deathClipName = clip.name;
                    break;
                }
            }
        }

        // Wait for the animator to actually play a death clip, then wait until it finishes
        float timer = 0f;
        float timeout = 1.75f;
        bool deathClipPlaying = false;

        while (timer < timeout)
        {
            var clips = animator.GetCurrentAnimatorClipInfo(0);
            if (clips.Length > 0)
            {
                var clip = clips[0].clip;
                var name = clip.name.ToLower();
                if (name.Contains("death") || name.Contains("die") || name.Contains("dead"))
                {
                    deathClipPlaying = true;

                    // Wait until the current state's normalized time reaches or exceeds 1.0 (clip finished)
                    while (true)
                    {
                        var state = animator.GetCurrentAnimatorStateInfo(0);
                        if (state.length > 0f && state.normalizedTime >= 1.0f)
                        {
                            break;
                        }
                        // If animator stopped updating or returned to default, break after timeout
                        timer += Time.deltaTime;
                        if (timer >= timeout) break;
                        yield return null;
                    }

                    break;
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        if (!deathClipPlaying)
        {
            // As a fallback, if a death-named clip exists in the controller, try to crossfade to it
            if (!string.IsNullOrEmpty(deathClipName) && controller != null)
            {
                animator.CrossFade(deathClipName, 0.1f);
                // wait a frame for the crossfade to start
                yield return null;
                // try to get the clip length
                float clipLength = 0f;
                foreach (var clip in controller.animationClips)
                {
                    if (clip.name == deathClipName)
                    {
                        clipLength = clip.length;
                        break;
                    }
                }
                if (clipLength > 0f)
                {
                    yield return new WaitForSeconds(clipLength / Mathf.Max(0.01f, animator.speed));
                }
                else
                {
                    yield return new WaitForSeconds(0.5f);
                }
            }
            else
            {
                // No death clip found; wait briefly to allow any transition to play
                yield return new WaitForSeconds(0.5f);
            }
        }

        Destroy(gameObject);
    }
}