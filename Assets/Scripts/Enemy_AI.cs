using UnityEngine;
using System.Collections;
using System.Reflection;

public class Enemy_AI : MonoBehaviour
{
    [Header("Settings")]
    public Transform player;
    public float speed = 2f;
    public float attackRange = 1.5f;
    public float aggroRange = 5f;
    public float patrolRadius = 3f;
    

    [Header("References (set in child)")]
    public Animator animator;
    protected Rigidbody2D rb;
    protected bool isDead = false;

    protected Vector2 startPos;
    protected Vector2 patrolTarget;
    protected float waitAtPoint = 1f;
    protected float waitTimer = 0f;

    protected enum State { Patrol, Chase, Attack }
    protected State state = State.Patrol;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
            else Debug.LogError("Enemy_AI cannot find the Player! Check the Tag.");
        }

        startPos = transform.position;
        ChoosePatrolTarget();
    }

    protected virtual void Update()
    {
        if (isDead) return;

        float dist = Vector2.Distance(transform.position, player.position);

        switch (state)
        {
            case State.Patrol:
                if (dist <= attackRange) { OnAttackRange(); }
                else if (dist <= aggroRange) { state = State.Chase; }
                else { PatrolUpdate(); }
                break;
            case State.Chase:
                if (dist <= attackRange) { OnAttackRange(); }
                else if (dist > aggroRange) { state = State.Patrol; ChoosePatrolTarget(); }
                else { MoveTowards(player.position); }
                break;
            case State.Attack:
                // attack coroutine handles movement
                break;
        }
    }

    protected void PatrolUpdate()
    {
        if (Vector2.Distance(transform.position, patrolTarget) < 0.2f)
        {
            waitTimer += Time.deltaTime;
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isMoving", false);
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
        Vector2 rand = Random.insideUnitCircle * patrolRadius;
        patrolTarget = startPos + rand;
    }

    protected void MoveTowards(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * speed;
        animator.SetBool("isMoving", true);
        animator.SetFloat("moveX", direction.x);
        animator.SetFloat("moveY", direction.y);
    }

    protected void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isMoving", false);
    }

    // Called when the player is detected inside `attackRange`.
    // Child classes must override this to perform the attack animation
    // and manage hitboxes. The base class does not initiate attacks.
    protected virtual void OnAttackRange() { }



    // Register damage from player. Concrete enemy classes should
    // override this to implement health and death behaviour.
    public virtual void TakeDamage(int amount)
    {
        Debug.LogWarning($"{this.GetType().Name} received damage but has no TakeDamage implementation.", this);
    }

    // Death / despawn behaviour should be implemented by concrete
    // enemy classes (e.g. `Slime_AI`). The base class intentionally
    // does not provide any default death coroutine.
}
