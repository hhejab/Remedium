using UnityEngine;

public class LockedBossDoor : MonoBehaviour
{
    public Animator animator;
    public Collider2D doorCollider;
    public SpriteRenderer doorRenderer; // Reference to the door's SpriteRenderer

    public string requiredKeyID = "BossKey";
    public string lockedMessage = "The door is locked. You need a key.";
    public string openMessage = "Use the boss key to open the door?";

    private bool opened = false;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (doorCollider == null)
            doorCollider = GetComponent<Collider2D>();

        if (doorRenderer == null)
            doorRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        UpdateSortingLayer();
    }

    public void TryOpen()
    {
        if (opened) return;

        InventoryPage inventory = FindFirstObjectByType<InventoryPage>(FindObjectsInactive.Include);
        PlayerInventory hotbar = FindFirstObjectByType<PlayerInventory>();

        bool hasKey =
            HasKeyInInventory(inventory) ||
            HasKeyInHotbar(hotbar);

        if (!hasKey)
        {
            TriggerUIManager.Instance.Show(lockedMessage, null);
            return;
        }

        TriggerUIManager.Instance.Show(openMessage, () =>
        {
            RemoveKey(inventory, hotbar);

            opened = true;

            if (animator != null)
                animator.SetTrigger("Open");

            if (doorCollider != null)
                doorCollider.enabled = false;
        });
    }

    private void UpdateSortingLayer()
{
    if (doorRenderer == null || animator == null)
        return;

    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

    if (stateInfo.IsName("DungeonDoor Closed"))
    {
        doorRenderer.sortingLayerName = "Base";
    }
    else if (stateInfo.IsName("DungeonDoor Open"))
    {
        doorRenderer.sortingLayerName = "AbovePlayer";
    }
}

    private bool HasKeyInInventory(InventoryPage inventory)
    {
        return inventory != null && inventory.HasItem(requiredKeyID);
    }

    private bool HasKeyInHotbar(PlayerInventory hotbar)
    {
        if (hotbar == null) return false;

        foreach (var slot in hotbar.hotbarSlots)
        {
            if (slot != null && slot.itemID == requiredKeyID && slot.currentQuantity > 0)
                return true;
        }

        return false;
    }

    private void RemoveKey(InventoryPage inventory, PlayerInventory hotbar)
    {
        if (inventory != null && inventory.RemoveItem(requiredKeyID, 1))
            return;

        if (hotbar == null) return;

        foreach (var slot in hotbar.hotbarSlots)
        {
            if (slot != null && slot.itemID == requiredKeyID && slot.currentQuantity > 0)
            {
                slot.currentQuantity--;

                if (slot.currentQuantity <= 0)
                    slot.ResetData();
                else
                    slot.UpdateUI();

                return;
            }
        }
    }
}