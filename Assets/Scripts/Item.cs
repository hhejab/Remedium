using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    public string itemName;
    public Sprite itemIcon;

    public void Interact()
    {
        PlayerInventory playerInv = Object.FindFirstObjectByType<PlayerInventory>();
        if (playerInv != null)
        {
            playerInv.AddToHotbar(this); // Tell inventory to take this item
        }
    }

    public void PickUp() => Destroy(gameObject);
}