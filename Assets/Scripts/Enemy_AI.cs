using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy_AI : MonoBehaviour
{
    [Header("Settings")]
    public Transform player;
    public float speed = 2f;
    public float attackRange = 1.5f;
    public float aggroRange = 5f;
    public float patrolRadius = 3f;

    [Header("Obstacle Avoidance")]
    public LayerMask obstacleLayer;
    public float obstacleCheckDistance = 0.6f;
    public float sideCheckAngle = 45f;
    public float stuckCheckTime = 0.5f;

    [Header("References")]
    public Animator animator;

    [Header("Skill XP Reward")]
    public int skillXPReward = 1;

    protected Rigidbody2D rb;
    protected bool isDead = false;

    protected Vector2 startPos;
    protected Vector2 patrolTarget;
    protected float waitAtPoint = 1f;
    protected float waitTimer = 0f;

    private Vector2 lastPosition;
    private float stuckTimer;
    private int avoidSide = 1;

    protected enum State { Patrol, Chase, Attack }
    protected State state = State.Patrol;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (animator == null)
            animator = GetComponent<Animator>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");

            if (p != null)
                player = p.transform;
            else
                Debug.LogError("Enemy_AI cannot find the Player! Check the Tag.");
        }

        startPos = transform.position;
        lastPosition = transform.position;

        ChoosePatrolTarget();
    }

    protected virtual void Update()
    {
        if (isDead) return;
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        switch (state)
        {
            case State.Patrol:
                if (dist <= attackRange)
                {
                    OnAttackRange();
                }
                else if (dist <= aggroRange)
                {
                    state = State.Chase;
                }
                else
                {
                    PatrolUpdate();
                }
                break;

            case State.Chase:
                if (dist <= attackRange)
                {
                    OnAttackRange();
                }
                else if (dist > aggroRange)
                {
                    state = State.Patrol;
                    ChoosePatrolTarget();
                }
                else
                {
                    MoveTowards(player.position);
                }
                break;

            case State.Attack:
                StopMoving();
                break;
        }

        CheckIfStuck();
    }

    protected void PatrolUpdate()
    {
        if (Vector2.Distance(transform.position, patrolTarget) < 0.2f)
        {
            waitTimer += Time.deltaTime;
            StopMoving();

            if (waitTimer >= waitAtPoint)
            {
                waitTimer = 0f;
                ChoosePatrolTarget();
            }
        }
        else
        {
            MoveTowards(patrolTarget);
        }
    }

    protected void ChoosePatrolTarget()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 rand = Random.insideUnitCircle * patrolRadius;
            Vector2 newTarget = startPos + rand;

            if (!Physics2D.Linecast(transform.position, newTarget, obstacleLayer))
            {
                patrolTarget = newTarget;
                return;
            }
        }

        patrolTarget = startPos;
    }

    protected void MoveTowards(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;

        if (direction == Vector2.zero)
        {
            StopMoving();
            return;
        }

        direction = GetAvoidedDirection(direction);

        rb.linearVelocity = direction * speed;

        animator.SetBool("isMoving", true);
        animator.SetFloat("moveX", direction.x);
        animator.SetFloat("moveY", direction.y);
    }

    private Vector2 GetAvoidedDirection(Vector2 direction)
    {
        RaycastHit2D forwardHit = Physics2D.Raycast(
            transform.position,
            direction,
            obstacleCheckDistance,
            obstacleLayer
        );

        if (forwardHit.collider == null)
            return direction;

        Vector2 leftDirection = Quaternion.Euler(0, 0, sideCheckAngle) * direction;
        Vector2 rightDirection = Quaternion.Euler(0, 0, -sideCheckAngle) * direction;

        bool leftBlocked = Physics2D.Raycast(
            transform.position,
            leftDirection,
            obstacleCheckDistance,
            obstacleLayer
        );

        bool rightBlocked = Physics2D.Raycast(
            transform.position,
            rightDirection,
            obstacleCheckDistance,
            obstacleLayer
        );

        if (!leftBlocked && rightBlocked)
        {
            avoidSide = 1;
            return leftDirection.normalized;
        }

        if (leftBlocked && !rightBlocked)
        {
            avoidSide = -1;
            return rightDirection.normalized;
        }

        if (!leftBlocked && !rightBlocked)
        {
            if (avoidSide == 1)
                return leftDirection.normalized;
            else
                return rightDirection.normalized;
        }

        Vector2 sharperLeft = Quaternion.Euler(0, 0, 90) * direction;
        Vector2 sharperRight = Quaternion.Euler(0, 0, -90) * direction;

        if (avoidSide == 1)
            return sharperLeft.normalized;
        else
            return sharperRight.normalized;
    }

    private void CheckIfStuck()
    {
        if (Vector2.Distance(transform.position, lastPosition) < 0.02f)
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer >= stuckCheckTime)
            {
                avoidSide *= -1;
                stuckTimer = 0f;

                if (state == State.Patrol)
                    ChoosePatrolTarget();
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPosition = transform.position;
    }

    protected void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;

        if (animator != null)
            animator.SetBool("isMoving", false);
    }

    protected virtual void OnAttackRange() { }

    public virtual void TakeDamage(int amount)
    {
        Debug.LogWarning($"{GetType().Name} received damage but has no TakeDamage implementation.", this);
    }
    protected void GiveSkillXPReward()
{
    PlayerSkillPointWallet wallet = FindFirstObjectByType<PlayerSkillPointWallet>();

    if (wallet != null)
        wallet.AddSkillXP(skillXPReward);
}

    protected virtual void Die()
{
    PlayerSkillPointWallet wallet = FindFirstObjectByType<PlayerSkillPointWallet>();

    if (wallet != null)
        wallet.AddSkillXP(skillXPReward);

    Destroy(gameObject);
}
}