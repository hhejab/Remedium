using UnityEngine;
using UnityEngine.InputSystem;

public class Shop_NPC_Talk : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference interactAction;

    [Header("Interaction UI")]
    public GameObject interactionObject;
    public Animator interactionAnim;

    [Header("Dialogue UI")]
    public DialogueSO dialogueSO;

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
        if (!playerInRange) return;

        if (InteractPressed())
        {
            if (!isTalking)
            {
                StartTalking();

                if (DialogueManager.Instance != null)
                    DialogueManager.Instance.StartDialogue(dialogueSO);
                else
                    Debug.LogWarning("DialogueManager Instance is missing!");
            }
            else
            {
                if (DialogueManager.Instance != null)
                    DialogueManager.Instance.AdvanceDialogue();
                else
                    Debug.LogWarning("DialogueManager Instance is missing!");
            }
        }
    }

    private bool InteractPressed()
    {
        if (interactAction == null) return false;
        return interactAction.action.WasPressedThisFrame();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
        ShowInteraction();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        HideInteraction();

        if (isTalking)
            StopTalking();
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

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (npcAnim != null)
        {
            npcAnim.SetBool("isMoving", false);
            npcAnim.Play("Idle");
        }

        Debug.Log("Talking to Shop NPC");
    }

    private void StopTalking()
    {
        isTalking = false;

        if (DialogueManager.Instance != null)
            DialogueManager.Instance.CloseDialogue();

        Debug.Log("Stopped talking to Shop NPC");
    }
}