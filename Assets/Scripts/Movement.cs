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

    private Vector2 movement;
    private Vector2 lastMoveDirection = Vector2.down;

    private bool isRunning;
    private bool isDead;
    private bool isHurt;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        StaminaSystem = GetComponent<StaminaSystem>();
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
    }

    void FixedUpdate()
    {
        if (isDead || isHurt) return;

        Vector2 input = movement.normalized;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
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
