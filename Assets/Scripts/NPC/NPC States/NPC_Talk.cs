using UnityEngine;
using UnityEngine.InputSystem;

public class NPC_Talk : MonoBehaviour
{
    public Transform player;
    public float interactionDistance = 1.5f;

    public GameObject interactionObject;
    public Animator interactionAnim;

    public MonoBehaviour movementScript;

    public DialogueSO dialogue;

    public InputActionReference interactAction;

    private Rigidbody2D rb;
    private Animator npcAnim;

    private bool playerInRange;
    private bool isTalking;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        npcAnim = GetComponent<Animator>();

        if (interactionObject != null)
            interactionObject.SetActive(false);
    }

    private void Update()
    {
        if (player == null)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= interactionDistance && !playerInRange)
        {
            playerInRange = true;
            ShowInteraction();
        }

        if (distance > interactionDistance && playerInRange)
        {
            playerInRange = false;
            HideInteraction();

            if (isTalking)
                StopTalking();
        }

        if (playerInRange && InteractPressed())
        {
            if (!isTalking)
            {
                StartTalking();
            }
            else
            {
                if (DialogueManager.Instance != null &&
                    DialogueManager.Instance.isDialogueActive)
                {
                    DialogueManager.Instance.AdvanceDialogue();
                }
            }
        }
    }

    private bool InteractPressed()
    {
        if (interactAction == null)
            return false;

        return interactAction.action.WasPressedThisFrame();
    }

    private void ShowInteraction()
    {
        if (interactionObject != null)
            interactionObject.SetActive(true);

        if (interactionAnim != null)
            interactionAnim.Play("Open");
    }

    private void HideInteraction()
    {
        if (interactionAnim != null)
            interactionAnim.Play("Close");

        if (interactionObject != null)
            interactionObject.SetActive(false);
    }

    private void StartTalking()
    {
        isTalking = true;

        if (movementScript != null)
        {
            movementScript.SendMessage(
                "StopForInteraction",
                SendMessageOptions.DontRequireReceiver
            );

            movementScript.enabled = false;
        }

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (npcAnim != null)
        {
            npcAnim.SetBool("isMoving", false);
            npcAnim.SetBool("isForging", false);
            npcAnim.Play("Idle");
        }

        if (dialogue != null)
        {
            DialogueManager.Instance.StartDialogue(dialogue);
        }
    }

    private void StopTalking()
    {
        isTalking = false;

        if (movementScript != null)
        {
            movementScript.enabled = true;

            movementScript.SendMessage(
                "ResumeAfterInteraction",
                SendMessageOptions.DontRequireReceiver
            );
        }

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.CloseDialogue();
        }
    }
}