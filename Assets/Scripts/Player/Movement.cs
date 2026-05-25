using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Movement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 8f;

    private Rigidbody2D rb;
    private Animator animator;
    private StaminaSystem StaminaSystem;

    private PlayerStats playerStats;

    private Vector2 movement;
    private Vector2 lastMoveDirection = Vector2.down;

    private bool isRunning;
    private bool isDead;
    private bool isHurt;

    [Header("Audio")]
    public AudioClip walkSFX;
    public float walkVolume = 1f;
    public float runPitch = 1.15f;
    private AudioSource walkAudioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        StaminaSystem = GetComponent<StaminaSystem>();
        playerStats = GetComponent<PlayerStats>();

        walkAudioSource = gameObject.AddComponent<AudioSource>();
        walkAudioSource.playOnAwake = false;
        walkAudioSource.loop = true;
        walkAudioSource.volume = walkVolume;
        if (walkSFX != null) walkAudioSource.clip = walkSFX;
    }

    public void OnMove(InputValue value)
    {
        if (isDead) return;
        movement = value.Get<Vector2>();
    }

    void Update()
    {
        if (isDead)
        {
            animator.SetBool("isMoving", false);
            animator.SetBool("isRunning", false);
            return;
        }

        Vector2 input = movement.normalized;
        bool isMoving = input != Vector2.zero;

        bool shiftPressed = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
        
        if (isMoving && shiftPressed && StaminaSystem != null && StaminaSystem.CanUseStamina(1f))
        {
            isRunning = true;
            StaminaSystem.DrainStaminaOverTime();
        }
        else
        {
            isRunning = false;
            if (StaminaSystem != null) StaminaSystem.RegenerateStamina();
        }

        if (isMoving)
        {
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                lastMoveDirection = new Vector2(Mathf.Sign(input.x), 0);
            else
                lastMoveDirection = new Vector2(0, Mathf.Sign(input.y));
        }

        animator.SetBool("isMoving", isMoving && !isHurt);
        animator.SetBool("isRunning", isRunning && isMoving && !isHurt);
        animator.SetFloat("moveX", lastMoveDirection.x);
        animator.SetFloat("moveY", lastMoveDirection.y);

        UpdateWalkAudio(isMoving && !isHurt);
    }

    private void UpdateWalkAudio(bool shouldPlay)
    {
        if (walkSFX == null || walkAudioSource == null) return;

        if (shouldPlay)
        {
            walkAudioSource.volume = walkVolume;
            walkAudioSource.pitch = isRunning ? runPitch : 1f;
            if (!walkAudioSource.isPlaying) walkAudioSource.Play();
        }
        else
        {
            if (walkAudioSource.isPlaying) walkAudioSource.Stop();
        }
    }

    void FixedUpdate()
    {
        if (isDead || isHurt) return;

        Vector2 input = movement.normalized;
        float finalWalkSpeed = walkSpeed;
        float finalRunSpeed = runSpeed;

            if (playerStats != null)
            {
                finalWalkSpeed += playerStats.walkSpeedBonus;
                finalRunSpeed += playerStats.runSpeedBonus;
            }

            float currentSpeed = isRunning ? finalRunSpeed : finalWalkSpeed;
        rb.MovePosition(rb.position + input * currentSpeed * Time.fixedDeltaTime);
    }

    public void TriggerHurt(float hurtDuration = 0.3f)
    {
        if (isDead) return;

        isHurt = true;
        movement = Vector2.zero;

        animator.SetBool("isMoving", false);
        animator.SetBool("isRunning", false);
        animator.SetTrigger("Hurt");

        CancelInvoke(nameof(EndHurt));
        Invoke(nameof(EndHurt), hurtDuration);
    }

    void EndHurt()
    {
        isHurt = false;
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        isHurt = false;
        isRunning = false;
        movement = Vector2.zero;

        animator.SetBool("isMoving", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isDead", true);
    }
}
