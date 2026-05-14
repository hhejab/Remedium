using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections; // Required for Coroutine

public class PlayerCombat : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    
    [Header("Combat Settings")]
    [Tooltip("Seconds between allowed attacks (matches attack animation length)")]
    public float attackCooldown = 1f;

    private float lastAttackTime = -Mathf.Infinity;
    
    [Tooltip("Seconds after clicking before the hitbox becomes active (timing of the strike)")]
    public float hitDelay = 0.5f;

    [Header("Hitboxes (Assign in Inspector)")]
    public GameObject hitboxUp;
    public GameObject hitboxDown;
    public GameObject hitboxLeft;
    public GameObject hitboxRight;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            PerformAttack();
        }
    }

    void PerformAttack()
    {
        // Enforce cooldown so the player can only attack once per animation
        if (Time.time - lastAttackTime < attackCooldown) return;
        lastAttackTime = Time.time;

        bool isMoving = rb.linearVelocity.magnitude > 0.1f;

        if (isMoving) anim.SetTrigger("isWalkAttacking");
        else anim.SetTrigger("isAttacking");

        // Determine direction based on Animator parameters (assuming you have these)
        float moveX = anim.GetFloat("moveX");
        float moveY = anim.GetFloat("moveY");

        // Select hitbox based on direction
        GameObject activeHitbox = null;

        if (Mathf.Abs(moveY) > Mathf.Abs(moveX)) // Vertical priority
        {
            activeHitbox = (moveY > 0) ? hitboxUp : hitboxDown;
        }
        else // Horizontal priority
        {
            activeHitbox = (moveX > 0) ? hitboxRight : hitboxLeft;
        }

        if (activeHitbox != null)
        {
            StartCoroutine(ActivateHitbox(activeHitbox, hitDelay));
        }
    }

    IEnumerator ActivateHitbox(GameObject hitbox, float delayBeforeActivate)
    {
        if (delayBeforeActivate > 0f) yield return new WaitForSeconds(delayBeforeActivate);
        hitbox.SetActive(true);
        yield return new WaitForSeconds(0.2f); // Swing duration
        hitbox.SetActive(false);
    }
}