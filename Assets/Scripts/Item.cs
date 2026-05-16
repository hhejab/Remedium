using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [Header("Item Data")]
    public string itemName;
    public Sprite itemIcon; 

    public void Interact()
    {
        // Look for the Hotbar
        PlayerInventory hotbar = Object.FindFirstObjectByType<PlayerInventory>();

        if (hotbar != null)
        {
            hotbar.AddToHotbar(this);
            Debug.Log(itemName + " sent to hotbar.");
        }
    }

    public void PickUp()
    {
        // Called by PlayerInventory ONLY after successfully adding to a slot
        Destroy(gameObject);
    }
}