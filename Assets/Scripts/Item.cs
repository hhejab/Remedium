using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    public string itemID; 
    public Sprite itemIcon;

    public void Interact()
    {
        if (string.IsNullOrEmpty(itemID)) itemID = gameObject.name;

        // Fix for CS1503: Using FindObjectsInactive.Include
        InventoryPage inv = FindFirstObjectByType<InventoryPage>(FindObjectsInactive.Include);
        PlayerInventory hotbar = FindFirstObjectByType<PlayerInventory>();

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