using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    public float roamRadius = 3f;
    public float roamInterval = 2f;
    public float roamSpeed = 0.8f;
    public float chaseSpeed = 2.0f;
    public float detectionRadius = 5f;
    public float attackRange = 0.6f;
    public LayerMask obstacleMask; // not used now, reserved for future raycast checks

    Rigidbody2D rb;
    Transform player;
    Vector2 roamOrigin;
    Vector2 roamTarget;
    float nextRoamTime;
    bool isChasing;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        roamOrigin = transform.position;
        ChooseNewRoamTarget();

        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        // update whether we should chase
        if (player != null)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            isChasing = dist <= detectionRadius;
        }
        else
        {
            isChasing = false;
        }

        if (!isChasing)
        {
            // roaming logic
            if (Time.time >= nextRoamTime || Vector2.Distance(transform.position, roamTarget) < 0.2f)
            {
                ChooseNewRoamTarget();
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 pos = rb.position;
        Vector2 desiredVelocity;

        if (isChasing && player != null)
        {
            Vector2 toPlayer = (Vector2)player.position - pos;
            float dist = toPlayer.magnitude;
            if (dist > attackRange)
            {
                desiredVelocity = toPlayer.normalized * chaseSpeed;
            }
            else
            {
                desiredVelocity = Vector2.zero; // in range to attack; keep still or play attack animation
            }
        }
        else
        {
            Vector2 toTarget = roamTarget - pos;
            desiredVelocity = toTarget.normalized * roamSpeed;
        }

        rb.linearVelocity = desiredVelocity;
    }

    void ChooseNewRoamTarget()
    {
        Vector2 rand = Random.insideUnitCircle * roamRadius;
        roamTarget = roamOrigin + rand;
        nextRoamTime = Time.time + roamInterval;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, roamRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
