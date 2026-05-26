using UnityEngine;
using System.Collections;

public class Skeleton_AI : Enemy_AI
{
    [Header("Skeleton Settings")]
    public GameObject hitboxUp;
    public GameObject hitboxDown;
    public GameObject hitboxLeft;
    public GameObject hitboxRight;
    public int attackDamage = 1;
    public float attackWindup = 0.5f;
    public float attackDuration = 0.25f;

    [Header("Stats")]
    public int health = 50;
    public float damageCooldown = 0.25f;
    private float lastDamageTime = -999f;

    
    [Header("Audio")]
    public AudioClip attackSFX;
    public AudioClip deathSFX;
    private AudioSource audioSource;

    protected override void Start()
    {
        base.Start();
        if (hitboxUp != null) hitboxUp.SetActive(false);
        if (hitboxDown != null) hitboxDown.SetActive(false);
        if (hitboxLeft != null) hitboxLeft.SetActive(false);
        if (hitboxRight != null) hitboxRight.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Try to auto-load from Resources/SFX/Enemy if clips not assigned
        if (attackSFX == null)
        {
            attackSFX = Resources.Load<AudioClip>("SFX/Enemy/skeleton_swing");
        }
        if (deathSFX == null)
        {
            deathSFX = Resources.Load<AudioClip>("SFX/Enemy/skeleton_death");
        }
    }

    // Called by base when player is within attackRange
    protected override void OnAttackRange()
    {
        if (isDead) return;

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Attack") || state == State.Attack) return;

        StartCoroutine(PerformDirectionalAttack());
    }

    protected IEnumerator PerformDirectionalAttack()
    {
        state = State.Attack;
        StopMoving();
        animator.SetTrigger("isAttacking");

        yield return new WaitForSeconds(attackWindup);

        if (isDead) yield break;

        // determine primary direction towards the player
        Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
        GameObject active = null;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            active = dir.x > 0 ? hitboxRight : hitboxLeft;
        }
        else
        {
            active = dir.y > 0 ? hitboxUp : hitboxDown;
        }

        if (active != null)
        {
            active.SetActive(true);
            if (audioSource != null && attackSFX != null)
            {
                audioSource.PlayOneShot(attackSFX);
            }
        }

        yield return new WaitForSeconds(attackDuration);

        if (active != null) active.SetActive(false);

        if (isDead) yield break;
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= attackRange) state = State.Chase;
        else if (dist <= aggroRange) state = State.Chase;
        else state = State.Patrol;
    }

    public override void TakeDamage(int amount)
    {
        Debug.Log($"Skeleton_AI TakeDamage({amount}) at time {Time.time}", this);

        if (isDead) return;
        if (Time.time < lastDamageTime + damageCooldown) return;
        lastDamageTime = Time.time;

        health -= amount;

        if (health > 0)
        {
            animator.SetTrigger("Hurt");
            state = State.Patrol;
            StopMoving();
        }
        else
        {
            Die();
        }
    }

    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        GiveSkillXPReward();

        StopMoving();
        animator.SetBool("isDead", true);
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;
        if (hitboxUp != null) hitboxUp.SetActive(false);
        if (hitboxDown != null) hitboxDown.SetActive(false);
        if (hitboxLeft != null) hitboxLeft.SetActive(false);
        if (hitboxRight != null) hitboxRight.SetActive(false);
        // Play death sound at position so it isn't cut off by destruction
        if (deathSFX != null)
        {
            AudioSource.PlayClipAtPoint(deathSFX, transform.position);
        }
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
        animator.Play("Death", 0, 0f);
        yield return null;

        // fixed delay to let the death animation play (customizable per enemy)
        yield return new WaitForSeconds(0.64f);

        Destroy(gameObject);
    }
}
