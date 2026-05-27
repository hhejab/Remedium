using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBoss_AI : Boss_AI
{
    [Header("Attack 1 Directional Hitboxes")]
    public GameObject attack1HitboxFront;
    public GameObject attack1HitboxBack;
    public GameObject attack1HitboxLeft;
    public GameObject attack1HitboxRight;

    [Header("Attack 2 AOE Hitbox")]
    public GameObject attack2HitboxAOE;

    [Header("50% Phase Slime Spawn")]
    public GameObject smallSlimePrefab;
    public Transform[] spawnPoints;
    public int slimesToSpawnAtHalfHealth = 2;

    [Header("Slime Boss Attack Settings")]
    public float closeAttackDistance = 0.95f;
    public int attack1Type = 1;
    public int attack2Type = 2;
    public float attack2Chance = 0.35f;

    [Header("Vulnerability")]
    public bool isVulnerable = true;

    [Header("Collision Movement")]
    public LayerMask obstacleLayer;
    public float wallCheckDistance = 0.15f;

    private readonly List<GameObject> spawnedSlimes = new List<GameObject>();

    private int currentAttackType = 1;
    private bool isHalfHealthPhaseActive = false;
    private bool hasTriggeredHalfHealthPhase = false;

    protected override void Awake()
    {
        base.Awake();

        FindPlayerIfMissing();

        isVulnerable = true;
        DisableAllAttackHitboxes();

        if (closeAttackDistance <= 0f)
            closeAttackDistance = 0.95f;
    }

    protected override void FixedUpdate()
    {
        if (isDead)
            return;

        FindPlayerIfMissing();

        if (player == null)
            return;

        CleanDeadSlimes();

        if (isHalfHealthPhaseActive)
        {
            isVulnerable = false;

            StopMoving();
            FacePlayer();

            if (animator != null)
            {
                animator.SetBool("isMoving", false);
                animator.SetBool("isRunning", false);
            }

            if (spawnedSlimes.Count == 0)
            {
                isHalfHealthPhaseActive = false;
                isVulnerable = true;
            }

            return;
        }

        isVulnerable = true;

        if (isAttacking || isHurt)
        {
            StopMoving();
            FacePlayer();
            return;
        }

        FacePlayer();

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= closeAttackDistance && Time.time >= nextAttackTime)
        {
            StartCoroutine(AttackRoutine());
            return;
        }

        MoveToPlayerWithCollision();
    }

    private void FindPlayerIfMissing()
    {
        if (player != null) return;

        GameObject p = GameObject.FindGameObjectWithTag("Player");

        if (p != null)
            player = p.transform;
    }

    public void TriggerHalfHealthPhase()
    {
        if (hasTriggeredHalfHealthPhase || isDead)
            return;

        hasTriggeredHalfHealthPhase = true;
        isHalfHealthPhaseActive = true;
        isVulnerable = false;

        StopMoving();
        DisableAllAttackHitboxes();

        if (animator != null)
        {
            animator.SetBool("isMoving", false);
            animator.SetBool("isRunning", false);
        }

        SpawnSmallSlimes();
    }

    protected override IEnumerator AttackRoutine()
    {
        isAttacking = true;

        StopMoving();
        FacePlayer();

        if (animator != null)
        {
            animator.SetBool("isMoving", false);
            animator.SetBool("isRunning", false);
        }

        yield return new WaitForSeconds(telegraphTime);

        currentAttackType = Random.value <= attack2Chance ? attack2Type : attack1Type;

        if (animator != null)
        {
            animator.SetInteger("attackType", currentAttackType);
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
        }

        StartCoroutine(EnableAttackHitbox());

        yield return new WaitForSeconds(attackAnimTime);

        nextAttackTime = Time.time + attackCooldown;
        isAttacking = false;
    }

    protected override IEnumerator EnableAttackHitbox()
    {
        yield return new WaitForSeconds(hitboxDelay);

        if (isDead)
            yield break;

        GameObject selectedHitbox = GetDirectionalAttackHitbox();

        if (selectedHitbox != null)
            selectedHitbox.SetActive(true);

        yield return new WaitForSeconds(hitboxActiveTime);

        if (selectedHitbox != null)
            selectedHitbox.SetActive(false);
    }

    protected override GameObject GetDirectionalAttackHitbox()
    {
        if (currentAttackType == attack2Type && attack2HitboxAOE != null)
            return attack2HitboxAOE;

        return GetAttack1DirectionalHitbox();
    }

    private GameObject GetAttack1DirectionalHitbox()
    {
        if (Mathf.Abs(lastDirection.x) > Mathf.Abs(lastDirection.y))
            return lastDirection.x < 0 ? attack1HitboxLeft : attack1HitboxRight;

        return lastDirection.y > 0 ? attack1HitboxBack : attack1HitboxFront;
    }

    protected override void DisableAllAttackHitboxes()
    {
        base.DisableAllAttackHitboxes();

        if (attack1HitboxFront != null) attack1HitboxFront.SetActive(false);
        if (attack1HitboxBack != null) attack1HitboxBack.SetActive(false);
        if (attack1HitboxLeft != null) attack1HitboxLeft.SetActive(false);
        if (attack1HitboxRight != null) attack1HitboxRight.SetActive(false);
        if (attack2HitboxAOE != null) attack2HitboxAOE.SetActive(false);
    }

    private void SpawnSmallSlimes()
    {
        if (smallSlimePrefab == null || spawnPoints == null || spawnPoints.Length == 0)
            return;

        int amount = Mathf.Min(slimesToSpawnAtHalfHealth, spawnPoints.Length);

        for (int i = 0; i < amount; i++)
        {
            if (spawnPoints[i] == null) continue;

            GameObject slime = Instantiate(smallSlimePrefab, spawnPoints[i].position, Quaternion.identity);
            spawnedSlimes.Add(slime);
        }
    }

    private void CleanDeadSlimes()
    {
        for (int i = spawnedSlimes.Count - 1; i >= 0; i--)
        {
            if (spawnedSlimes[i] == null)
                spawnedSlimes.RemoveAt(i);
        }
    }

    public bool CanTakeDamage()
    {
        return isVulnerable && !isDead;
    }

    private void MoveToPlayerWithCollision()
    {
        Vector2 direction = ((Vector2)player.position - rb.position).normalized;

        lastDirection = direction;

        if (animator != null)
        {
            animator.SetFloat("moveX", direction.x);
            animator.SetFloat("moveY", direction.y);
            animator.SetBool("isMoving", true);
            animator.SetBool("isRunning", false);
        }

        if (!Blocked(direction))
        {
            rb.linearVelocity = direction * moveSpeed;
            return;
        }

        Vector2 xOnly = new Vector2(direction.x, 0f).normalized;
        Vector2 yOnly = new Vector2(0f, direction.y).normalized;

        if (xOnly != Vector2.zero && !Blocked(xOnly))
        {
            lastDirection = xOnly;
            rb.linearVelocity = xOnly * moveSpeed;
            return;
        }

        if (yOnly != Vector2.zero && !Blocked(yOnly))
        {
            lastDirection = yOnly;
            rb.linearVelocity = yOnly * moveSpeed;
            return;
        }

        StopMoving();

        if (animator != null)
            animator.SetBool("isMoving", false);
    }

    private bool Blocked(Vector2 direction)
    {
        if (direction == Vector2.zero) return true;
        if (obstacleLayer.value == 0) return false;

        RaycastHit2D hit = Physics2D.Raycast(rb.position, direction, wallCheckDistance, obstacleLayer);
        return hit.collider != null;
    }

  public override void Die()
{
    if (isDead) return;

    isVulnerable = false;
    DisableAllAttackHitboxes();

    for (int i = spawnedSlimes.Count - 1; i >= 0; i--)
    {
        if (spawnedSlimes[i] != null)
            Destroy(spawnedSlimes[i]);
    }

    spawnedSlimes.Clear();

    StartCoroutine(DieAfterAnimation());
}

private IEnumerator DieAfterAnimation()
{
    base.Die();

    yield return new WaitForSeconds(deathAnimationTime);

    BossDefeatReward reward = GetComponent<BossDefeatReward>();

    if (reward != null)
        reward.GiveRewardAndReturn();
}
}