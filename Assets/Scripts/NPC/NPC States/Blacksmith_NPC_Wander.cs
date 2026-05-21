using System.Collections;
using UnityEngine;

public class Blacksmith_NPC_Wander : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;
    public float reachDistance = 0.2f;
    public float wanderRadius = 4f;

    [Header("Idle")]
    public float minIdleTime = 1f;
    public float maxIdleTime = 3f;

    [Header("Forging")]
    public Transform forgePoint;
    public float minTimeBeforeForging = 8f;
    public float maxTimeBeforeForging = 15f;
    public float forgeDuration = 5f;
    public Vector2 forgeFacingDirection = Vector2.up;

    [Header("Collision Avoidance")]
    public LayerMask obstacleLayer;
    public float checkDistance = 0.25f;
    public float pointCheckRadius = 0.25f;

    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D boxCollider;

    private Vector2 target;
    private bool isIdle;
    private bool isBusy;
    private Coroutine idleRoutine;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        PickNewTarget();
        StartCoroutine(ForgingRoutine());
    }

    private void FixedUpdate()
    {
        if (isIdle || isBusy) return;

        MoveToTarget();
    }

    private void MoveToTarget()
    {
        Vector2 direction = target - rb.position;

        if (direction.magnitude <= reachDistance)
        {
            idleRoutine = StartCoroutine(IdleThenPickNewTarget());
            return;
        }

        Vector2 moveDirection = direction.normalized;

        if (IsBlocked(moveDirection))
        {
            idleRoutine = StartCoroutine(IdleThenPickNewTarget());
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

    private IEnumerator ForgingRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minTimeBeforeForging, maxTimeBeforeForging);
            yield return new WaitForSeconds(waitTime);

            if (forgePoint == null) continue;

            yield return GoToForgeAndWork();
        }
    }

    private IEnumerator GoToForgeAndWork()
    {
        isBusy = true;
        isIdle = false;

        if (idleRoutine != null)
        {
            StopCoroutine(idleRoutine);
            idleRoutine = null;
        }

        animator.SetBool("isForging", false);

        Vector2 forgeTarget = forgePoint.position;

        while (Vector2.Distance(rb.position, forgeTarget) > reachDistance)
        {
            Vector2 direction = forgeTarget - rb.position;
            Vector2 moveDirection = direction.normalized;

            if (IsBlocked(moveDirection))
            {
                break;
            }

            rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);

            animator.SetBool("isMoving", true);
            animator.SetFloat("moveX", moveDirection.x);
            animator.SetFloat("moveY", moveDirection.y);

            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isMoving", false);

        animator.SetFloat("moveX", forgeFacingDirection.x);
        animator.SetFloat("moveY", forgeFacingDirection.y);

        animator.SetBool("isForging", true);

        yield return new WaitForSeconds(forgeDuration);

        animator.SetBool("isForging", false);

        PickNewTarget();
        isBusy = false;
    }

    public void StopForInteraction()
    {
        isBusy = true;
        isIdle = false;

        if (idleRoutine != null)
        {
            StopCoroutine(idleRoutine);
            idleRoutine = null;
        }

        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isMoving", false);
        animator.SetBool("isForging", false);
    }

    public void ResumeAfterInteraction()
    {
        PickNewTarget();
        isBusy = false;
    }
}