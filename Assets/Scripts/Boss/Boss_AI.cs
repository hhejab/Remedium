using System.Collections;
using UnityEngine;

public class Boss_AI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public Rigidbody2D rb;

    [Header("Movement")]
    public float moveSpeed = 2.2f;

    [Header("Attack")]
    public float attackDistance = 1.8f;
    public float attackCooldown = 1.4f;
    public float attackAnimTime = 0.7f;
    public float telegraphTime = 0.25f;

    [Header("Directional Attack Hitboxes")]
    public GameObject attackHitboxFront;
    public GameObject attackHitboxBack;
    public GameObject attackHitboxLeft;
    public GameObject attackHitboxRight;

    [Header("Hitbox Timing")]
    public float hitboxDelay = 0.15f;
    public float hitboxActiveTime = 0.45f;

    [Header("Death")]
    public float deathAnimationTime = 3;
    
    [Header("NPC Spawn")]
    [Tooltip("Disabled NPC in-scene to enable when the boss dies. Assign the disabled NPC GameObject here.")]
    public GameObject npcToEnableOnDeath;
    public Vector2 npcSpawnOffset = Vector2.zero;
    [Tooltip("If true, will instantiate npcPrefab instead of enabling an existing disabled NPC.")]
    public bool instantiateNpcFromPrefab = false;
    public GameObject npcPrefab;
    [Tooltip("Delay in seconds before the NPC appears after the boss dies.")]
    public float npcSpawnDelay = 3f;

    protected bool isAttacking;
    protected bool isHurt;
    protected bool isDead;

    protected float nextAttackTime;
    protected Vector2 lastDirection = Vector2.down;

    protected virtual void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
                animator = GetComponentInChildren<Animator>();
        }

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        DisableAllAttackHitboxes();
        SetAnimatorDirection(lastDirection);
    }

    protected virtual void FixedUpdate()
    {
        if (isDead)
            return;

        if (player == null)
            return;

        if (isAttacking || isHurt)
        {
            StopMoving();
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        FacePlayer();

        if (distance <= attackDistance && Time.time >= nextAttackTime)
        {
            StartCoroutine(AttackRoutine());
        }
        else
        {
            WalkToPlayer();
        }
    }

    protected virtual void WalkToPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        lastDirection = GetCardinalDirection(direction);
        SetAnimatorDirection(lastDirection);

        rb.linearVelocity = direction * moveSpeed;

        animator.SetBool("isMoving", true);
        animator.SetBool("isRunning", false);
    }

    protected virtual IEnumerator AttackRoutine()
    {
        isAttacking = true;

        StopMoving();

        animator.SetBool("isMoving", false);
        animator.SetBool("isRunning", false);

        FacePlayer();

        yield return new WaitForSeconds(telegraphTime);

        animator.SetBool("isAttacking", true);

        StartCoroutine(EnableAttackHitbox());

        yield return new WaitForSeconds(attackAnimTime);

        animator.SetBool("isAttacking", false);

        nextAttackTime = Time.time + attackCooldown;

        isAttacking = false;
    }

    protected virtual IEnumerator EnableAttackHitbox()
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

    protected virtual GameObject GetDirectionalAttackHitbox()
    {
        if (Mathf.Abs(lastDirection.x) > Mathf.Abs(lastDirection.y))
        {
            if (lastDirection.x < 0)
                return attackHitboxLeft;

            return attackHitboxRight;
        }

        if (lastDirection.y > 0)
            return attackHitboxBack;

        return attackHitboxFront;
    }

    protected virtual void DisableAllAttackHitboxes()
    {
        if (attackHitboxFront != null)
            attackHitboxFront.SetActive(false);

        if (attackHitboxBack != null)
            attackHitboxBack.SetActive(false);

        if (attackHitboxLeft != null)
            attackHitboxLeft.SetActive(false);

        if (attackHitboxRight != null)
            attackHitboxRight.SetActive(false);
    }

    public virtual void PlayHurt()
    {
        if (isDead || isAttacking || isHurt)
            return;

        StartCoroutine(HurtRoutine());
    }

    protected virtual IEnumerator HurtRoutine()
    {
        isHurt = true;

        StopMoving();

        animator.SetBool("isMoving", false);
        animator.SetBool("isRunning", false);
        animator.SetTrigger("Hurt");

        yield return new WaitForSeconds(0.25f);

        isHurt = false;
    }

public virtual void Die()
{
    if (isDead) return;
    isDead = true;

    StopAllCoroutines();

    if (rb != null)
        rb.linearVelocity = Vector2.zero;

    DisableAllAttackHitboxes();

    float x = 0f;
    float y = -1f;

    if (player != null)
    {
        Vector2 dir = player.position - transform.position;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            x = dir.x > 0 ? 1f : -1f;
            y = 0f;
        }
        else
        {
            x = 0f;
            y = dir.y > 0 ? 1f : -1f;
        }
    }

    CreateDelayedNpcSpawner(npcSpawnDelay, x, y);

    if (animator == null)
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    if (animator != null)
    {
        animator.SetFloat("moveX", x);
        animator.SetFloat("moveY", y);

        animator.SetBool("isMoving", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isHurt", false);
        animator.SetBool("isDead", true);

        animator.Play("Death", 0, 0f);
        animator.Update(0f);

        var controller = animator.runtimeAnimatorController;
        Debug.Log($"Die(): animator.enabled={animator.enabled}, controller={(controller != null ? controller.name : "null")} ");
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        Debug.Log($"Die(): currentStateIsDeath={stateInfo.IsName("Death")}, normalizedTime={stateInfo.normalizedTime}");
    }

    StartCoroutine(DeathRoutine());
}

private IEnumerator DeathRoutine()
{
    yield return new WaitForSeconds(deathAnimationTime);
    Destroy(gameObject);
}

    private void CreateDelayedNpcSpawner(float delay, float facingX, float facingY)
    {
        var spawnerGO = new GameObject("DelayedNpcSpawner");
        var spawner = spawnerGO.AddComponent<DelayedNpcSpawner>();
        spawner.delay = delay;
        spawner.npcToEnable = npcToEnableOnDeath;
        spawner.instantiateFromPrefab = instantiateNpcFromPrefab;
        spawner.npcPrefab = npcPrefab;
        spawner.spawnOffset = npcSpawnOffset;
        spawner.spawnPosition = transform.position;
        spawner.facingX = facingX;
        spawner.facingY = facingY;
        spawner.StartSpawn();
    }

    private class DelayedNpcSpawner : MonoBehaviour
    {
        public float delay;
        public GameObject npcToEnable;
        public bool instantiateFromPrefab;
        public GameObject npcPrefab;
        public Vector2 spawnOffset;
        public Vector3 spawnPosition;
        public float facingX;
        public float facingY;

        public void StartSpawn()
        {
            StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            if (delay > 0f)
                yield return new WaitForSeconds(delay);

            Vector3 pos = spawnPosition + (Vector3)spawnOffset;

            if (npcToEnable != null && !instantiateFromPrefab)
            {
                npcToEnable.transform.position = pos;
                npcToEnable.SetActive(true);

                var npcAnim = npcToEnable.GetComponent<Animator>() ?? npcToEnable.GetComponentInChildren<Animator>();
                if (npcAnim != null)
                {
                    npcAnim.SetFloat("moveX", facingX);
                    npcAnim.SetFloat("moveY", facingY);
                    npcAnim.SetBool("isMoving", false);
                }
            }
            else if (instantiateFromPrefab && npcPrefab != null)
            {
                var newNpc = Instantiate(npcPrefab, pos, Quaternion.identity);
                var npcAnim = newNpc.GetComponent<Animator>() ?? newNpc.GetComponentInChildren<Animator>();
                if (npcAnim != null)
                {
                    npcAnim.SetFloat("moveX", facingX);
                    npcAnim.SetFloat("moveY", facingY);
                    npcAnim.SetBool("isMoving", false);
                }
            }

            Destroy(gameObject);
        }
    }
    protected virtual void FacePlayer()
    {
        if (player == null)
            return;

        Vector2 direction = (player.position - transform.position).normalized;
        lastDirection = GetCardinalDirection(direction);

        SetAnimatorDirection(lastDirection);
    }

    protected virtual Vector2 GetCardinalDirection(Vector2 direction)
    {
        if (direction == Vector2.zero)
            return lastDirection;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            return direction.x > 0 ? Vector2.right : Vector2.left;

        return direction.y > 0 ? Vector2.up : Vector2.down;
    }

    protected virtual void SetAnimatorDirection(Vector2 direction)
    {
        if (animator == null)
            return;

        animator.SetFloat("moveX", direction.x);
        animator.SetFloat("moveY", direction.y);
    }

    protected virtual void StopMoving()
    {
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }
}
