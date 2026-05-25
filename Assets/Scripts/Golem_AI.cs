using UnityEngine;
using System.Collections;

public class Golem_AI : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Movement / Range")]
    public float moveSpeed = 2f;
    public float aggroRange = 8f;
    public float attackRange = 1.5f;

    [Header("Attack Timing")]
    public float attackCooldown = 1.5f;
    public float attackWindup = 0.25f;
    public float hitboxActiveTime = 0.25f;

    [Header("Attack Hitboxes")]
    public GameObject hitBoxFront;
    public GameObject hitBoxBack;
    public GameObject hitBoxLeft;
    public GameObject hitBoxRight;

    [Header("Health")]
    public int maxHealth = 100;

    [Header("Sorting")]
    public int sortingOffset = 0;
    public string sortingLayerName = "Player";

    private int currentHealth;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    private bool isAttacking;
    private bool isDead;
    private bool isHurt;

    private Vector2 lastDirection = Vector2.down;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        currentHealth = maxHealth;

        DisableAllHitboxes();
    }

    private void FixedUpdate()
    {
        if (isDead || isAttacking || isHurt || player == null)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            StopMoving();
            StartCoroutine(AttackRoutine());
        }
        else if (distance <= aggroRange)
        {
            ChasePlayer();
        }
        else
        {
            StopMoving();
        }
    }

    private void LateUpdate()
    {
        UpdateSorting();
    }

    private void ChasePlayer()
    {
        Vector2 direction = ((Vector2)player.position - rb.position).normalized;

        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            lastDirection = new Vector2(Mathf.Sign(direction.x), 0);
        else
            lastDirection = new Vector2(0, Mathf.Sign(direction.y));

        animator.SetFloat("moveX", lastDirection.x);
        animator.SetFloat("moveY", lastDirection.y);
        animator.SetBool("isMoving", true);
        animator.SetBool("isRunning", false);
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        StopMoving();

        Vector2 directionToPlayer = ((Vector2)player.position - rb.position).normalized;

        if (Mathf.Abs(directionToPlayer.x) > Mathf.Abs(directionToPlayer.y))
            lastDirection = new Vector2(Mathf.Sign(directionToPlayer.x), 0);
        else
            lastDirection = new Vector2(0, Mathf.Sign(directionToPlayer.y));

        animator.SetFloat("moveX", lastDirection.x);
        animator.SetFloat("moveY", lastDirection.y);

        animator.SetBool("isAttacking", true);

        yield return new WaitForSeconds(attackWindup);

        GameObject activeHitbox = GetDirectionalHitbox();

        if (activeHitbox != null)
            activeHitbox.SetActive(true);

        yield return new WaitForSeconds(hitboxActiveTime);

        if (activeHitbox != null)
            activeHitbox.SetActive(false);

        animator.SetBool("isAttacking", false);

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
    }

    private GameObject GetDirectionalHitbox()
    {
        if (lastDirection.y < 0) return hitBoxFront;
        if (lastDirection.y > 0) return hitBoxBack;
        if (lastDirection.x < 0) return hitBoxLeft;
        if (lastDirection.x > 0) return hitBoxRight;

        return hitBoxFront;
    }

    private void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;

        animator.SetBool("isMoving", false);
        animator.SetBool("isRunning", false);
    }

    private void UpdateSorting()
    {
        if (spriteRenderer == null)
            return;

        float sortY;

        if (boxCollider != null)
        {
            // Uses the bottom of the collider as the feet/base point
            sortY = boxCollider.bounds.min.y;
        }
        else
        {
            // Fallback if no collider exists
            sortY = transform.position.y;
        }

        spriteRenderer.sortingLayerName = sortingLayerName;
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-sortY * 100) + sortingOffset;
    }

    private void DisableAllHitboxes()
    {
        if (hitBoxFront != null) hitBoxFront.SetActive(false);
        if (hitBoxBack != null) hitBoxBack.SetActive(false);
        if (hitBoxLeft != null) hitBoxLeft.SetActive(false);
        if (hitBoxRight != null) hitBoxRight.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(HurtRoutine());
    }

    private IEnumerator HurtRoutine()
    {
        isHurt = true;

        StopMoving();

        animator.SetBool("isHurt", true);

        yield return new WaitForSeconds(0.35f);

        animator.SetBool("isHurt", false);

        isHurt = false;
    }

    private void Die()
    {
        isDead = true;

        StopMoving();
        DisableAllHitboxes();

        animator.SetBool("isDead", true);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        rb.simulated = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}