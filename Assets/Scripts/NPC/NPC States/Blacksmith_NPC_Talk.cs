using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Blacksmith_NPC_Talk : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    public float interactionDistance = 1.5f;

    [Header("Interaction Bubble")]
    public GameObject interactionObject;
    public Animator interactionAnim;
    public float closeAnimationTime = 0.25f;

    [Header("Input")]
    public InputActionReference interactAction; // Drag PlayerActions -> Interaction here

    private Animator npcAnim;
    private Rigidbody2D rb;

    private bool playerInRange;
    private bool isTalking;
    private Coroutine hideRoutine;

    private void Awake()
    {
        npcAnim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (interactionObject != null)
            interactionObject.SetActive(false);
    }

    private void Start()
    {
        // Default: blacksmith is working
        if (npcAnim != null)
            npcAnim.SetBool("isTalking", false);
    }

    private void OnEnable()
    {
        if (interactAction != null)
            interactAction.action.Enable();
    }

    private void OnDisable()
    {
        if (interactAction != null)
            interactAction.action.Disable();
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Player comes close
        if (distance <= interactionDistance && !playerInRange)
        {
            playerInRange = true;
            ShowInteraction();
        }

        // Player walks away
        else if (distance > interactionDistance && playerInRange)
        {
            playerInRange = false;
            HideInteraction();

            if (isTalking)
                StopTalking();
        }

        // Press interaction button
        if (playerInRange && InteractPressed())
        {
            if (!isTalking)
                StartTalking();
            else
                StopTalking();
        }
    }

    private bool InteractPressed()
    {
        if (interactAction == null) return false;
        return interactAction.action.WasPressedThisFrame();
    }

    private void ShowInteraction()
    {
        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
            hideRoutine = null;
        }

        if (interactionObject != null)
            interactionObject.SetActive(true);

        if (interactionAnim != null)
            interactionAnim.Play("Open", 0, 0f);
    }

    private void HideInteraction()
    {
        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        hideRoutine = StartCoroutine(HideInteractionRoutine());
    }

    private IEnumerator HideInteractionRoutine()
    {
        if (interactionAnim != null)
            interactionAnim.Play("Close", 0, 0f);

        yield return new WaitForSeconds(closeAnimationTime);

        if (interactionObject != null)
            interactionObject.SetActive(false);

        hideRoutine = null;
    }

    private void StartTalking()
    {
        isTalking = true;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        // Stop forging and go to Forge Idle
        if (npcAnim != null)
            npcAnim.SetBool("isTalking", true);

        Debug.Log("Talking to Blacksmith");
    }

    private void StopTalking()
    {
        isTalking = false;

        // Go back to Forge Working
        if (npcAnim != null)
            npcAnim.SetBool("isTalking", false);

        Debug.Log("Stopped talking to Blacksmith");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}