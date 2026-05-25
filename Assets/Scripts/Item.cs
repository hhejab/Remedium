using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    public string itemID;
    public Sprite itemIcon;

    private bool pickedUp = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pickedUp) return;
        if (!other.CompareTag("Player")) return;

        PickUp();
    }

    public void Interact()
    {
        if (pickedUp) return;

        PickUp();
    }

    private void PickUp()
    {
        if (pickedUp) return;

        if (string.IsNullOrEmpty(itemID))
            itemID = gameObject.name;

        PlayerInventory hotbar = FindFirstObjectByType<PlayerInventory>();

        if (hotbar != null && hotbar.TryAddToHotbar(itemID, itemIcon))
        {
            pickedUp = true;
            Debug.Log("Picked up: " + itemID);
            Destroy(gameObject);
            return;
        }

        InventoryPage inventory = FindFirstObjectByType<InventoryPage>(FindObjectsInactive.Include);

        if (inventory != null && inventory.TryAddToInventory(itemID, itemIcon))
        {
            pickedUp = true;
            Debug.Log("Picked up to inventory: " + itemID);
            Destroy(gameObject);
            return;
        }

        Debug.LogWarning("Pickup failed: " + itemID);
    }
}