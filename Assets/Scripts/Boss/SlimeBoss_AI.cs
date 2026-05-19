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

    [Header("Attack Settings")]
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

        isVulnerable = true;
        DisableAllAttackHitboxes();
    }

    protected override void FixedUpdate()
    {
        if (isDead)
            return;

        if (player == null)
            return;

        CleanDeadSlimes();

        // During the 50% phase, boss waits until small slimes are dead.
        if (isHalfHealthPhaseActive)
        {
            isVulnerable = false;

            StopMoving();
            FacePlayer();

            animator.SetBool("isMoving", false);
            animator.SetBool("isRunning", false);

            // If all spawned slimes are dead, boss becomes vulnerable again.
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
            return;
        }

        FacePlayer();

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackDistance && Time.time >= nextAttackTime)
        {
            StartCoroutine(AttackRoutine());
        }
        else
        {
            MoveToPlayerWithCollision();
        }
    }

    // Call this from BossHealth when HP reaches 50%
    public void TriggerHalfHealthPhase()
    {
        if (hasTriggeredHalfHealthPhase || isDead)
            return;

        hasTriggeredHalfHealthPhase = true;
        isHalfHealthPhaseActive = true;
        isVulnerable = false;

        StopMoving();
        DisableAllAttackHitboxes();

        animator.SetBool("isMoving", false);
        animator.SetBool("isRunning", false);

        SpawnSmallSlimes();
    }

    protected override IEnumerator AttackRoutine()
    {
        isAttacking = true;

        StopMoving();

        animator.SetBool("isMoving", false);
        animator.SetBool("isRunning", false);

        FacePlayer();

        yield return new WaitForSeconds(telegraphTime);

        currentAttackType = Random.value <= attack2Chance ? attack2Type : attack1Type;

        animator.SetInteger("attackType", currentAttackType);
        animator.SetTrigger("Attack");

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
        if (currentAttackType == attack2Type)
            return attack2HitboxAOE;

        return GetAttack1DirectionalHitbox();
    }

    private GameObject GetAttack1DirectionalHitbox()
    {
        if (Mathf.Abs(lastDirection.x) > Mathf.Abs(lastDirection.y))
        {
            if (lastDirection.x < 0)
                return attack1HitboxLeft;

            return attack1HitboxRight;
        }

        if (lastDirection.y > 0)
            return attack1HitboxBack;

        return attack1HitboxFront;
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
        if (smallSlimePrefab == null)
        {
            Debug.LogWarning("Small Slime Prefab is missing.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("Spawn Points are missing.");
            return;
        }

        int amount = Mathf.Min(slimesToSpawnAtHalfHealth, spawnPoints.Length);

        for (int i = 0; i < amount; i++)
        {
            if (spawnPoints[i] == null)
                continue;

            GameObject slime = Instantiate(
                smallSlimePrefab,
                spawnPoints[i].position,
                Quaternion.identity
            );

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

        animator.SetFloat("moveX", direction.x);
        animator.SetFloat("moveY", direction.y);
        animator.SetBool("isMoving", true);

        RaycastHit2D hit = Physics2D.Raycast(
            rb.position,
            direction,
            wallCheckDistance,
            obstacleLayer
        );

        if (hit.collider != null)
        {
            StopMoving();
            animator.SetBool("isMoving", false);
            return;
        }

        rb.linearVelocity = direction * moveSpeed;
    }

    public override void Die()
    {
        isVulnerable = false;

        DisableAllAttackHitboxes();

        for (int i = spawnedSlimes.Count - 1; i >= 0; i--)
        {
            if (spawnedSlimes[i] != null)
                Destroy(spawnedSlimes[i]);
        }

        spawnedSlimes.Clear();

        base.Die();
    }
}