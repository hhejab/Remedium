using UnityEngine;

public class NPC_Patrol : MonoBehaviour
{
    public Vector2[] patrolPoints;
    public float speed = 2f;
    public float reachDistance = 0.2f;

    [Header("Collision Avoidance")]
    public LayerMask obstacleLayer;
    public float checkDistance = 0.15f;

    private int currentPatrolIndex = 0;
    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D boxCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        if (patrolPoints.Length > 0)
        {
            rb.position = patrolPoints[0];
            currentPatrolIndex = 1;
        }
    }

    void FixedUpdate()
    {
        if (patrolPoints.Length == 0)
        {
            SetIdle();
            return;
        }

        Vector2 target = patrolPoints[currentPatrolIndex];
        Vector2 direction = target - rb.position;

        // Reached patrol point
        if (direction.magnitude <= reachDistance)
        {
            GoToNextPoint();
            SetIdle();
            return;
        }

        Vector2 moveDirection = direction.normalized;

        // Check if collider is blocking the NPC
        bool blocked = IsBlocked(moveDirection);

        if (blocked)
        {
            SetIdle();

            // Try another patrol point instead of forcing through the collider
            GoToNextPoint();
            return;
        }

        // Move
        rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);

        // Animation
        animator.SetBool("isMoving", true);
        animator.SetFloat("moveX", moveDirection.x);
        animator.SetFloat("moveY", moveDirection.y);
    }

    private bool IsBlocked(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            rb.position + boxCollider.offset,
            boxCollider.size * 0.9f,
            0f,
            direction,
            checkDistance,
            obstacleLayer
        );

        return hit.collider != null;
    }

    private void GoToNextPoint()
    {
        currentPatrolIndex++;

        if (currentPatrolIndex >= patrolPoints.Length)
            currentPatrolIndex = 0;
    }

    private void SetIdle()
    {
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isMoving", false);
    }
}