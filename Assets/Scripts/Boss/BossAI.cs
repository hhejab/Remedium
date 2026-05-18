using System.Collections;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public Rigidbody2D rb;

    [Header("Wander / Searching")]
    public float wanderSpeed = 1.4f;
    public float wanderRadius = 4f;
    public float wanderPointWaitTime = 1f;
    public bool alwaysSearchForPlayer = true;

    [Header("Movement")]
    public float detectionRange = 999f;
    public float attackRange = 1.7f;
    public float moveSpeed = 2.1f;
    public float runSpeed = 3.2f;

    [Header("Boss Attack")]
    public float attackCooldown = 1.4f;
    public float attackAnimTime = 0.45f;
    public float telegraphTime = 0.35f;

    [Header("Attack Hitbox")]
    public GameObject attackHitbox;
    public float hitboxDelay = 0.15f;
    public float hitboxActiveTime = 0.15f;

    [Header("Dash Attack")]
    public float dashRange = 5f;
    public float dashSpeed = 8f;
    public float dashDuration = 0.18f;
    public float dashCooldown = 4f;

    [Header("Death")]
    public float deathFallbackDelay = 1.2f;

    private bool isAttacking;
    private bool isHurt;
    private bool isDead;

    private float nextAttackTime;
    private float nextDashTime;

    private Vector2 spawnPosition;
    private Vector2 wanderTarget;
    private bool hasWanderTarget;
    private float nextWanderTime;

    private Vector2 lastDirection = Vector2.down;

    private Coroutine currentActionRoutine;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
                animator = GetComponentInChildren<Animator>();
        }

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        spawnPosition = transform.position;

        if (attackHitbox != null)
            attackHitbox.SetActive(false);

        if (animator != null)
            SetAnimatorDirection(lastDirection);
    }

    private void FixedUpdate()
    {
        if (isDead)
            return;

        if (isAttacking || isHurt)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (player == null)
        {
            Wander();
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (alwaysSearchForPlayer || distance <= detectionRange)
        {
            FacePlayer();

            if (distance <= attackRange && Time.time >= nextAttackTime)
            {
                currentActionRoutine = StartCoroutine(ChooseAttackPattern());
            }
            else if (distance <= dashRange && Time.time >= nextDashTime && Time.time >= nextAttackTime)
            {
                currentActionRoutine = StartCoroutine(DashAttack());
            }
            else
            {
                ChasePlayer(distance);
            }
        }
        else
        {
            Wander();
        }
    }

    private void Wander()
    {
        if (!hasWanderTarget || Vector2.Distance(transform.position, wanderTarget) < 0.2f)
        {
            if (Time.time < nextWanderTime)
            {
                rb.linearVelocity = Vector2.zero;
                animator.SetBool("isMoving", false);
                animator.SetBool("isRunning", false);
                return;
            }

            Vector2 randomPoint = Random.insideUnitCircle * wanderRadius;
            wanderTarget = spawnPosition + randomPoint;
            hasWanderTarget = true;
            nextWanderTime = Time.time + wanderPointWaitTime;
        }

        Vector2 direction = (wanderTarget - (Vector2)transform.position).normalized;
        lastDirection = GetCardinalDirection(direction);

        rb.linearVelocity = direction * wanderSpeed;

        animator.SetBool("isMoving", true);
        animator.SetBool("isRunning", false);

        SetAnimatorDirection(lastDirection);
    }

    private void ChasePlayer(float distance)
    {
        Vector2 direction = (player.position - transform.position).normalized;
        lastDirection = GetCardinalDirection(direction);

        bool shouldRun = distance > 4f;
        float speed = shouldRun ? runSpeed : moveSpeed;

        rb.linearVelocity = direction * speed;

        animator.SetBool("isMoving", true);
        animator.SetBool("isRunning", shouldRun);

        SetAnimatorDirection(lastDirection);
    }

    private IEnumerator ChooseAttackPattern()
    {
        if (isDead)
            yield break;

        isAttacking = true;

        rb.linearVelocity = Vector2.zero;

        animator.SetBool("isMoving", false);
        animator.SetBool("isRunning", false);

        FacePlayer();

        int random = Random.Range(0, 100);

        if (random < 35)
            yield return StartCoroutine(DoubleAttack());
        else
            yield return StartCoroutine(SingleAttack());

        nextAttackTime = Time.time + attackCooldown;

        isAttacking = false;
    }

    private IEnumerator SingleAttack()
    {
        yield return new WaitForSeconds(telegraphTime);

        if (isDead)
            yield break;

        FacePlayer();

        animator.SetBool("isAttacking", true);

        StartCoroutine(EnableAttackHitbox());

        yield return new WaitForSeconds(attackAnimTime);

        animator.SetBool("isAttacking", false);

        yield return new WaitForSeconds(0.2f);
    }

    private IEnumerator DoubleAttack()
    {
        yield return StartCoroutine(SingleAttack());

        if (isDead)
            yield break;

        yield return new WaitForSeconds(0.25f);

        FacePlayer();

        yield return StartCoroutine(SingleAttack());
    }

    private IEnumerator EnableAttackHitbox()
    {
        yield return new WaitForSeconds(hitboxDelay);

        if (isDead)
            yield break;

        if (attackHitbox != null)
            attackHitbox.SetActive(true);

        yield return new WaitForSeconds(hitboxActiveTime);

        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }

    private IEnumerator DashAttack()
    {
        if (isDead)
            yield break;

        isAttacking = true;

        nextDashTime = Time.time + dashCooldown;
        nextAttackTime = Time.time + attackCooldown;

        rb.linearVelocity = Vector2.zero;

        animator.SetBool("isMoving", false);
        animator.SetBool("isRunning", false);

        FacePlayer();

        yield return new WaitForSeconds(0.45f);

        if (isDead)
            yield break;

        Vector2 dashDirection = (player.position - transform.position).normalized;
        lastDirection = GetCardinalDirection(dashDirection);
        SetAnimatorDirection(lastDirection);

        animator.SetBool("isAttacking", true);

        StartCoroutine(EnableAttackHitbox());

        float timer = 0f;

        while (timer < dashDuration)
        {
            if (isDead)
                yield break;

            rb.linearVelocity = dashDirection * dashSpeed;
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(attackAnimTime);

        animator.SetBool("isAttacking", false);

        yield return new WaitForSeconds(0.3f);

        isAttacking = false;
    }

    public void PlayHurt()
    {
        if (isDead || isAttacking || isHurt)
            return;

        currentActionRoutine = StartCoroutine(Hurt());
    }

    private IEnumerator Hurt()
    {
        isHurt = true;

        rb.linearVelocity = Vector2.zero;

        animator.SetBool("isMoving", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isHurt", true);

        yield return new WaitForSeconds(0.25f);

        animator.SetBool("isHurt", false);

        isHurt = false;
    }

    public void Die()
{
    if (isDead) return;

    isDead = true;

    rb.linearVelocity = Vector2.zero;

    animator.SetBool("isMoving", false);
    animator.SetBool("isRunning", false);
    animator.SetBool("isAttacking", false);
    animator.SetBool("isHurt", false);
    animator.SetBool("isDead", true);

    if (attackHitbox != null)
        attackHitbox.SetActive(false);

    Collider2D col = GetComponent<Collider2D>();
    if (col != null)
        col.enabled = false;

    StartCoroutine(PlayDeathAndDespawn());
}

private IEnumerator PlayDeathAndDespawn()
{
    if (animator == null)
    {
        Debug.LogWarning("PlayDeathAndDespawn: animator is null, destroying instantly.");
        Destroy(gameObject);
        yield break;
    }

    animator.SetBool("isDead", true);

    // Force Death state immediately
    animator.Play("Death", 0, 0f);
    animator.Update(0f);

    // Debug info to help diagnose why animation isn't playing
    var controller = animator.runtimeAnimatorController;
    Debug.Log($"PlayDeathAndDespawn: animator.enabled={animator.enabled}, controller={(controller != null ? controller.name : "null")} ");

    // Let animator update into Death state
    yield return null;

    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
    Debug.Log($"PlayDeathAndDespawn: currentStateIsDeath={stateInfo.IsName("Death")}, normalizedTime={stateInfo.normalizedTime}");

    // Wait for death animation (fallback)
    yield return new WaitForSeconds(0.85f);

    Destroy(gameObject);
}

    private IEnumerator DeathRoutine()
    {
        isDead = true;
        isAttacking = false;
        isHurt = false;

        if (attackHitbox != null)
            attackHitbox.SetActive(false);

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        animator.SetBool("isMoving", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isHurt", false);
        animator.SetBool("isDead", true);

        // Important for Death blend tree direction.
        SetAnimatorDirection(lastDirection);

        // This forces the Animator state called "Death".
        animator.Play("Death", 0, 0f);
        animator.Update(0f);

        Debug.Log("Boss death animation forced.");

        yield return null;

        float timer = 0f;

        while (timer < deathFallbackDelay)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("Death") && stateInfo.normalizedTime >= 0.95f)
                break;

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    private void FacePlayer()
    {
        if (player == null)
            return;

        Vector2 direction = (player.position - transform.position).normalized;
        lastDirection = GetCardinalDirection(direction);

        SetAnimatorDirection(lastDirection);
    }

    private Vector2 GetCardinalDirection(Vector2 direction)
    {
        if (direction == Vector2.zero)
            return lastDirection;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
                return Vector2.right;
            else
                return Vector2.left;
        }
        else
        {
            if (direction.y > 0)
                return Vector2.up;
            else
                return Vector2.down;
        }
    }

    private void SetAnimatorDirection(Vector2 direction)
    {
        animator.SetFloat("moveX", direction.x);
        animator.SetFloat("moveY", direction.y);
    }
}