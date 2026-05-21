using System.Collections;
using UnityEngine;

public class NPC_Wander : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;
    public float reachDistance = 0.2f;
    public float wanderRadius = 4f;

    [Header("Idle")]
    public float minIdleTime = 1f;
    public float maxIdleTime = 3f;

    [Header("Collision Avoidance")]
    public LayerMask obstacleLayer;
    public float checkDistance = 0.25f;
    public float pointCheckRadius = 0.25f;

    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D boxCollider;

    private Vector2 target;
    private bool isIdle;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        PickNewTarget();
    }

    void FixedUpdate()
    {
        if (isIdle) return;

        Vector2 direction = target - rb.position;

        if (direction.magnitude <= reachDistance)
        {
            StartCoroutine(IdleThenPickNewTarget());
            return;
        }

        Vector2 moveDirection = direction.normalized;

        if (IsBlocked(moveDirection))
        {
            StartCoroutine(IdleThenPickNewTarget());
            return;
        }

        rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);

        animator.SetBool("isMoving", true);
        animator.SetFloat("moveX", moveDirection.x);
        animator.SetFloat("moveY", moveDirection.y);
    }

    private bool IsBlocked(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            rb.position + boxCollider.offset,
            boxCollider.size * 0.85f,
            0f,
            direction,
            checkDistance,
            obstacleLayer
        );

        return hit.collider != null;
    }

    private IEnumerator IdleThenPickNewTarget()
    {
        isIdle = true;

        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isMoving", false);

        float waitTime = Random.Range(minIdleTime, maxIdleTime);
        yield return new WaitForSeconds(waitTime);

        PickNewTarget();

        isIdle = false;
    }

    private void PickNewTarget()
    {
        for (int i = 0; i < 30; i++)
        {
            Vector2 randomPoint = rb.position + Random.insideUnitCircle * wanderRadius;

            bool pointBlocked = Physics2D.OverlapCircle(
                randomPoint,
                pointCheckRadius,
                obstacleLayer
            );

            if (!pointBlocked)
            {
                target = randomPoint;
                return;
            }
        }

        target = rb.position;
    }
}