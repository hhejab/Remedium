using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections; // Required for Coroutine

public class PlayerCombat : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;

    [Header("Audio")]
    public AudioClip swingSFX;
    private AudioSource audioSource;
    public float attackCooldown = 1f;
    private float lastAttackTime = -999f;

    [Header("Hitboxes (Assign in Inspector)")]
    public GameObject hitboxUp;
    public GameObject hitboxDown;
    public GameObject hitboxLeft;
    public GameObject hitboxRight;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
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
        if (Time.time < lastAttackTime + attackCooldown) return;
        lastAttackTime = Time.time;

        bool isMoving = rb.linearVelocity.magnitude > 0.1f;

        if (isMoving) anim.SetTrigger("isWalkAttacking");
        else anim.SetTrigger("isAttacking");

        StartCoroutine(PlaySwingDelayed(0.5f));

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
            StartCoroutine(ActivateHitboxDelayed(activeHitbox, 1f));
        }
    }

    IEnumerator ActivateHitboxDelayed(GameObject hitbox, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (hitbox == null) yield break;
        hitbox.SetActive(true);
        yield return new WaitForSeconds(0.2f); // Swing duration
        hitbox.SetActive(false);
    }

    IEnumerator PlaySwingDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (audioSource != null && swingSFX != null) audioSource.PlayOneShot(swingSFX);
    }

    public void PlaySwingSFX()
    {
        if (audioSource != null && swingSFX != null) audioSource.PlayOneShot(swingSFX);
    }
}