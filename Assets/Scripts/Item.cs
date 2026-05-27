using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [Header("Item Configuration")]
    public ItemData myItemData; // <-- This replaces itemID and itemIcon!

    private bool pickedUp = false;

    public void Interact()
    {
        if (pickedUp) return;
        PickUp();
    }

    public void PickUp()
    {
        // Safety check: Don't pick up if there's no data assigned
        if (pickedUp || myItemData == null) return;

        PlayerInventory hotbar = FindFirstObjectByType<PlayerInventory>();

        // Note: We now pass the whole 'myItemData' instead of separate strings and sprites
        if (hotbar != null && hotbar.TryAddToHotbar(myItemData))
        {
            pickedUp = true;
            Destroy(gameObject);
            return;
        }

        InventoryPage inventory = FindFirstObjectByType<InventoryPage>(FindObjectsInactive.Include);

        if (inventory != null && inventory.TryAddToInventory(myItemData))
        {
            pickedUp = true;
            Destroy(gameObject);
            return;
        }

        Debug.LogWarning("Pickup failed. Inventory might be full! Item: " + myItemData.itemName);
    }
}