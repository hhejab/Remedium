using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private PlayerStats playerStats;

    [Header("Input")]
    public InputActionReference attackAction;

    [Header("Hitboxes")]
    public GameObject hitboxUp;
    public GameObject hitboxDown;
    public GameObject hitboxLeft;
    public GameObject hitboxRight;

    [Header("Attack Settings")]
    public float attackDuration = 0.25f;
    public float attackCooldown = 1f;
    private float lastAttackTime = -999f;

    public AudioClip swingSFX;
    private AudioSource audioSource;

    private bool isAttacking;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerStats = GetComponent<PlayerStats>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        DisableAllHitboxes();
    }

    private void OnEnable()
    {
        if (attackAction != null)
            attackAction.action.Enable();
    }

    private void OnDisable()
    {
        if (attackAction != null)
            attackAction.action.Disable();

        DisableAllHitboxes();
    }

    private void Update()
    {
        if (attackAction != null && attackAction.action.WasPressedThisFrame())
        {
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;
        lastAttackTime = Time.time;
        if (isAttacking) return;
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        float moveX = anim.GetFloat("moveX");
        float moveY = anim.GetFloat("moveY");

        bool isMoving = anim.GetBool("isMoving") || anim.GetBool("isRunning");

        if (isMoving)
        {
            anim.ResetTrigger("isAttacking");
            anim.SetTrigger("isWalkAttacking");
        }
        else
        {
            anim.ResetTrigger("isWalkAttacking");
            anim.SetTrigger("isAttacking");
        }

        GameObject activeHitbox = GetDirectionalHitbox(moveX, moveY);

        float hitboxDuration = attackDuration;
        if (playerStats != null && playerStats.attackSpeed > 0)
            hitboxDuration = attackDuration / playerStats.attackSpeed;

        // play swing audio slightly delayed to match animation
        StartCoroutine(PlaySwingDelayed(0.5f));

        // activate hitbox after attack windup (delay) so damage registers later
        if (activeHitbox != null)
            StartCoroutine(ActivateHitboxDelayed(activeHitbox, 1f, hitboxDuration));

        float finalAttackDuration = attackDuration;

        if (playerStats != null && playerStats.attackSpeed > 0)
        {
            finalAttackDuration = attackDuration / playerStats.attackSpeed;
        }

        yield return new WaitForSeconds(finalAttackDuration);

        if (activeHitbox != null)
            activeHitbox.SetActive(false);

        isAttacking = false;
    }

    private GameObject GetDirectionalHitbox(float moveX, float moveY)
    {
        if (Mathf.Abs(moveY) > Mathf.Abs(moveX))
        {
            if (moveY > 0)
                return hitboxUp;
            else
                return hitboxDown;
        }

        if (moveX > 0)
            return hitboxRight;
        else
            return hitboxLeft;
    }

    private void DisableAllHitboxes()
    {
        if (hitboxUp != null)
            hitboxUp.SetActive(false);

        if (hitboxDown != null)
            hitboxDown.SetActive(false);

        if (hitboxLeft != null)
            hitboxLeft.SetActive(false);

        if (hitboxRight != null)
            hitboxRight.SetActive(false);
    }

    private IEnumerator ActivateHitboxDelayed(GameObject hitbox, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);
        if (hitbox == null) yield break;
        hitbox.SetActive(true);
        yield return new WaitForSeconds(duration);
        if (hitbox != null) hitbox.SetActive(false);
    }

    private IEnumerator PlaySwingDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (audioSource != null && swingSFX != null) audioSource.PlayOneShot(swingSFX);
    }
}