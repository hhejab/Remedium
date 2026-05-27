using UnityEngine;

public class LockedBossDoor : MonoBehaviour
{
    public Animator animator;
    public Collider2D doorCollider;
    public SpriteRenderer doorRenderer;

    // TWEAK: Changed from string to ItemData
    [Header("Configuration")]
    public ItemData requiredKeyData;

    public string lockedMessage = "The door is locked. You need a key.";
    public string openMessage = "Use the boss key to open the door?";

    private bool opened = false;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (doorCollider == null) doorCollider = GetComponent<Collider2D>();
        if (doorRenderer == null) doorRenderer = GetComponent<SpriteRenderer>();
        SetClosedSorting();
    }

    public void TryOpen()
    {
        if (opened) return;

        InventoryPage inventory = FindFirstObjectByType<InventoryPage>(FindObjectsInactive.Include);
        PlayerInventory hotbar = FindFirstObjectByType<PlayerInventory>();

        // TWEAK: Pass the ItemData object instead of a string
        bool hasKey = HasKeyInInventory(inventory) || HasKeyInHotbar(hotbar);

        if (!hasKey)
        {
            TriggerUIManager.Instance.Show(lockedMessage, null);
            return;
        }

        TriggerUIManager.Instance.Show(openMessage, () =>
        {
            RemoveKey(inventory, hotbar);
            opened = true;
            SetOpenSorting();
            if (animator != null) animator.SetTrigger("Open");
            if (doorCollider != null) doorCollider.enabled = false;
        });
    }

    private void SetClosedSorting() { if (doorRenderer != null) doorRenderer.sortingLayerName = "Base"; }
    private void SetOpenSorting() { if (doorRenderer != null) doorRenderer.sortingLayerName = "AbovePlayer"; }

    private bool HasKeyInInventory(InventoryPage inventory)
    {
        // TWEAK: Ensure InventoryPage has the updated HasItem(ItemData data) method
        return inventory != null && inventory.HasItem(requiredKeyData);
    }

    private bool HasKeyInHotbar(PlayerInventory hotbar)
    {
        if (hotbar == null) return false;
        foreach (var slot in hotbar.hotbarSlots)
        {
            // TWEAK: Compare against the data name
            if (slot != null && slot.itemID == requiredKeyData.itemName && slot.currentQuantity > 0)
                return true;
        }
        return false;
    }

    private void RemoveKey(InventoryPage inventory, PlayerInventory hotbar)
    {
        // TWEAK: Use the data name for removal
        if (inventory != null && inventory.RemoveItem(requiredKeyData.itemName, 1)) return;

        if (hotbar == null) return;

        foreach (var slot in hotbar.hotbarSlots)
        {
            if (slot != null && slot.itemID == requiredKeyData.itemName && slot.currentQuantity > 0)
            {
                slot.currentQuantity--;
                if (slot.currentQuantity <= 0) slot.ResetData();
                else slot.UpdateUI();
                return;
            }
        }
    }
}