using UnityEngine;
using System.Collections;

public class Slime_AI : Enemy_AI
{
    [Header("Slime Settings")]
    public GameObject hitbox; // single hitbox GameObject (enable/disable)
    public int attackDamage = 1;
    public float attackWindup = 0.5f;
    public float attackDuration = 0.25f;
    [Header("Stats")]
    public int health = 30;
    public float damageCooldown = 0.25f;
    private float lastDamageTime = -999f;

    protected override void Start()
    {
        base.Start();
        if (hitbox != null) hitbox.SetActive(false);
    }

    // Called by base when player is within attackRange
    protected override void OnAttackRange()
    {
        if (isDead) return;

        // Prevent spamming attack while already in Attack animation/state
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Attack") || state == State.Attack) return;

        StartCoroutine(PerformRangedAttack());
    }

    protected IEnumerator PerformRangedAttack()
    {
        state = State.Attack;
        StopMoving();
        animator.SetTrigger("isAttacking");

        yield return new WaitForSeconds(attackWindup);

        if (!isDead && hitbox != null)
        {
            hitbox.SetActive(true);
        }

        yield return new WaitForSeconds(attackDuration);

        if (hitbox != null) hitbox.SetActive(false);

        // After attack, resume chase if player still in range
        if (isDead) yield break;
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= attackRange) state = State.Chase; // allow immediate re-evaluation by base Update
        else if (dist <= aggroRange) state = State.Chase;
        else state = State.Patrol;
    }

    public override void TakeDamage(int amount)
    {
        Debug.Log($"Slime_AI TakeDamage({amount}) at time {Time.time}", this);

        if (isDead) return;
        if (Time.time < lastDamageTime + damageCooldown) return;
        lastDamageTime = Time.time;

        health -= amount;

        if (health > 0)
        {
            animator.SetTrigger("Hurt");
            state = State.Patrol; // pause attacks while hurt; base will handle movement next frame
            StopMoving();
        }
        else
        {
            Die();
        }
    }

    protected void Die()
    {
        if (isDead) return;
        isDead = true;
        StopMoving();
        animator.SetBool("isDead", true);
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;
        if (hitbox != null) hitbox.SetActive(false);
        StartCoroutine(PlayDeathAndDespawn());
    }

    protected IEnumerator PlayDeathAndDespawn()
    {
        if (animator == null)
        {
            Destroy(gameObject);
            yield break;
        }
        animator.SetBool("isDead", true);

        // Force the animator to play the Death state immediately so the
        // animation actually starts regardless of transition settings.
        animator.Play("Death", 0, 0f);
        yield return null; // allow the animator to update to the Death state

        // Wait a fixed amount of time (0.95s) before despawning so the
        // death animation plays up to the penultimate frame.
        yield return new WaitForSeconds(0.85f);

        Destroy(gameObject);
    }
}
