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

    private bool isAttacking;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerStats = GetComponent<PlayerStats>();

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
        if (isAttacking)
            return;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        float moveX = anim.GetFloat("moveX");
        float moveY = anim.GetFloat("moveY");

        // IMPORTANT:
        // Do NOT use rb.linearVelocity here because your Movement script uses rb.MovePosition.
        // So velocity can stay 0 even while moving.
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

        if (activeHitbox != null)
            activeHitbox.SetActive(true);

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
}