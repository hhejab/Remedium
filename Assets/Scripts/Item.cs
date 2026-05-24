using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    public string itemID;
    public Sprite itemIcon;

    public void Interact()
    {
        if (string.IsNullOrEmpty(itemID))
            itemID = gameObject.name;

        InventoryPage inv = FindFirstObjectByType<InventoryPage>(FindObjectsInactive.Include);
        PlayerInventory hotbar = FindFirstObjectByType<PlayerInventory>();

        // Keys go to main inventory only
        if (itemID == "BossKey")
        {
            if (inv != null && inv.TryAddToInventory(itemID, itemIcon))
            {
                Debug.Log("Picked up key: " + itemID);
                Destroy(gameObject);
                return;
            }

            Debug.LogWarning("Pickup Failed: Inventory is full!");
            return;
        }

        // Normal items can go hotbar first
        if (hotbar != null && hotbar.TryAddToHotbar(itemID, itemIcon))
        {
            Destroy(gameObject);
        }
        else if (inv != null && inv.TryAddToInventory(itemID, itemIcon))
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("Pickup Failed: Both Hotbar and Inventory are full!");
        }
    }
}