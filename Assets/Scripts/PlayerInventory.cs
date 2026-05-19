using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<InventoryItem> hotbarSlots; 

    public bool TryAddToHotbar(string id, Sprite icon)
    {
        foreach (var slot in hotbarSlots)
            if (slot != null && slot.itemID == id && slot.currentQuantity < slot.maxStackSize) {
                slot.currentQuantity++; slot.UpdateUI(); return true;
            }
        foreach (var slot in hotbarSlots)
            if (slot != null && string.IsNullOrEmpty(slot.itemID)) {
                slot.SetData(id, icon, 1); return true;
            }
        return false;
    }
}