using UnityEngine;
using System.Collections;

public class Goblin_AI : Enemy_AI
{
    [Header("Goblin Settings")]
    public GameObject hitboxUp;
    public GameObject hitboxDown;
    public GameObject hitboxLeft;
    public GameObject hitboxRight;
    public int attackDamage = 1;
    public float attackWindup = 0.4f;
    public float attackDuration = 0.2f;

    [Header("Stats")]
    public int health = 40;
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
            if (audioSource != null && attackSFX != null) audioSource.PlayOneShot(attackSFX);
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
        Debug.Log($"Goblin_AI TakeDamage({amount}) at time {Time.time}", this);

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

        yield return new WaitForSeconds(0.50f);

        Destroy(gameObject);
    }
}
