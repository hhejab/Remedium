using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    public string itemID;
    public Sprite itemIcon;

    private bool pickedUp = false;

    public void Interact()
    {
        if (pickedUp) return;
        PickUp();
    }

    public void PickUp()
    {
        if (pickedUp) return;

        if (string.IsNullOrEmpty(itemID))
            itemID = gameObject.name;

        PlayerInventory hotbar = FindFirstObjectByType<PlayerInventory>();

        if (hotbar != null && hotbar.TryAddToHotbar(itemID, itemIcon))
        {
            pickedUp = true;
            Destroy(gameObject);
            return;
        }

        InventoryPage inventory = FindFirstObjectByType<InventoryPage>(FindObjectsInactive.Include);

        if (inventory != null && inventory.TryAddToInventory(itemID, itemIcon))
        {
            pickedUp = true;
            Destroy(gameObject);
            return;
        }

        Debug.LogWarning("Pickup failed: " + itemID);
    }
}