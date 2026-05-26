using UnityEngine;
using UnityEngine.InputSystem;

public class Boss_NPC_Talk : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference interactAction;

    [Header("Interaction UI")]
    public GameObject interactionObject;
    public Animator interactionAnim;

    [Header("Dialogue UI")]
    public DialogueSO dialogueSO;
    [Header("Special Dialogue")]
    [Tooltip("Plays if the player has item IDs '99' and '98' anywhere in hotbar or inventory")]
    public DialogueSO specialDialogueSO;

    [Header("Replacement NPC")]
    [Tooltip("Disabled NPC in-scene to enable after special dialogue (will destroy the current NPC)")]
    public GameObject replacementNpcToEnable;
    public Vector2 replacementSpawnOffset = Vector2.zero;
    [Tooltip("If true, will instantiate replacementNpcPrefab instead of enabling an existing disabled NPC.")]
    public bool instantiateReplacementFromPrefab = false;
    public GameObject replacementNpcPrefab;

    private Rigidbody2D rb;
    private Animator npcAnim;

    private bool playerInRange;
    private bool isTalking;
    private bool pendingConsumeSpecial = false;

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
        // If dialogue finished and we were waiting to consume special items, do it now
        if (pendingConsumeSpecial && DialogueManager.Instance != null && !DialogueManager.Instance.isDialogueActive)
        {
            pendingConsumeSpecial = false;
            ConsumeSpecialItems();
            // After consuming items for the special interaction, spawn the replacement NPC and remove this one
            SpawnReplacementAndDestroySelf();
        }

        if (!playerInRange) return;

        if (InteractPressed())
        {
            if (!isTalking)
            {
                StartTalking();

                if (DialogueManager.Instance != null)
                {
                    DialogueSO toPlay = GetDialogueForInventory();
                    // If we're about to play the special dialogue, mark items to be consumed after dialogue finishes
                    pendingConsumeSpecial = (toPlay == specialDialogueSO && specialDialogueSO != null);
                    DialogueManager.Instance.StartDialogue(toPlay);
                }
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

    private DialogueSO GetDialogueForInventory()
    {
        bool has99 = false;

        // Check hotbar via PlayerInventory if available
        PlayerInventory hotbar = FindFirstObjectByType<PlayerInventory>();
        if (hotbar != null && hotbar.hotbarSlots != null)
        {
            foreach (var slot in hotbar.hotbarSlots)
            {
                if (slot == null) continue;
                if (!string.IsNullOrEmpty(slot.itemID))
                {
                    if (slot.itemID == "99") has99 = true;
                }
            }
        }

        InventoryItem[] items = FindObjectsOfType<InventoryItem>(true);
        foreach (var it in items)
        {
            if (it == null) continue;
            if (string.IsNullOrEmpty(it.itemID)) continue;
            if (it.itemID == "99") has99 = true;
            if (has99) break;
        }

        if (has99 && specialDialogueSO != null)
            return specialDialogueSO;

        // fallback to configured dialogueSO if special conditions aren't met
        return dialogueSO;
    }

    private void ConsumeSpecialItems()
    {
        RemoveOneFromInventoryOrHotbar("99");
    }

    private void SpawnReplacementAndDestroySelf()
    {
        // Determine current facing from this NPC's animator if available
        float facingX = 0f;
        float facingY = -1f;
        if (npcAnim != null)
        {
            // Use animator parameters if present
            try
            {
                facingX = npcAnim.GetFloat("moveX");
                facingY = npcAnim.GetFloat("moveY");
            }
            catch { }
        }

        Vector3 spawnPos = transform.position + (Vector3)replacementSpawnOffset;

        if (replacementNpcToEnable != null && !instantiateReplacementFromPrefab)
        {
            replacementNpcToEnable.transform.position = spawnPos;
            replacementNpcToEnable.SetActive(true);

            var repAnim = replacementNpcToEnable.GetComponent<Animator>() ?? replacementNpcToEnable.GetComponentInChildren<Animator>();
            if (repAnim != null)
            {
                repAnim.SetFloat("moveX", facingX);
                repAnim.SetFloat("moveY", facingY);
                repAnim.SetBool("isMoving", false);
                repAnim.Play("Idle", 0, 0f);
                repAnim.Update(0f);
            }
        }
        else if (instantiateReplacementFromPrefab && replacementNpcPrefab != null)
        {
            var newNpc = Instantiate(replacementNpcPrefab, spawnPos, Quaternion.identity);
            var repAnim = newNpc.GetComponent<Animator>() ?? newNpc.GetComponentInChildren<Animator>();
            if (repAnim != null)
            {
                repAnim.SetFloat("moveX", facingX);
                repAnim.SetFloat("moveY", facingY);
                repAnim.SetBool("isMoving", false);
                repAnim.Play("Idle", 0, 0f);
                repAnim.Update(0f);
            }
        }

        Destroy(gameObject);
    }

    private void RemoveOneFromInventoryOrHotbar(string id)
    {
        // Try hotbar first
        PlayerInventory hotbar = FindFirstObjectByType<PlayerInventory>();
        if (hotbar != null && hotbar.hotbarSlots != null)
        {
            foreach (var slot in hotbar.hotbarSlots)
            {
                if (slot == null) continue;
                if (string.IsNullOrEmpty(slot.itemID)) continue;
                if (slot.itemID == id)
                {
                    if (slot.currentQuantity > 1)
                    {
                        slot.currentQuantity--;
                        slot.UpdateUI();
                    }
                    else
                    {
                        slot.SetData("", null, 0);
                    }
                    return;
                }
            }
        }

        // Then check inventory UI slots anywhere (include inactive)
        InventoryItem[] items = FindObjectsOfType<InventoryItem>(true);
        foreach (var it in items)
        {
            if (it == null) continue;
            if (string.IsNullOrEmpty(it.itemID)) continue;
            if (it.itemID == id)
            {
                if (it.currentQuantity > 1)
                {
                    it.currentQuantity--;
                    it.UpdateUI();
                }
                else
                {
                    it.SetData("", null, 0);
                }
                return;
            }
        }
    }

    private void StopTalking()
    {
        isTalking = false;

        if (DialogueManager.Instance != null)
            DialogueManager.Instance.CloseDialogue();

        Debug.Log("Stopped talking to NPC");
    }
}
