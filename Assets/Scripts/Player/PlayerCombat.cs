using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections; // Required for Coroutine

public class PlayerCombat : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;

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
            StartCoroutine(ActivateHitbox(activeHitbox));
        }
    }

    IEnumerator ActivateHitbox(GameObject hitbox)
    {
        hitbox.SetActive(true);
        yield return new WaitForSeconds(0.2f); // Swing duration
        hitbox.SetActive(false);
    }
}