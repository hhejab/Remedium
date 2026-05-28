using System.Collections.Generic;
using UnityEngine;

public class InventoryPage : MonoBehaviour
{
    [SerializeField] private InventoryItem itemPrefab;
    [SerializeField] private RectTransform contentPanel;
    private List<InventoryItem> uiItems = new List<InventoryItem>();

    public void InitializeInventoryUI(int size)
    {
        foreach (Transform child in contentPanel) Destroy(child.gameObject);
        uiItems.Clear();
        for (int i = 0; i < size; i++)
        {
            InventoryItem item = Instantiate(itemPrefab, contentPanel);
            item.ResetData();
            uiItems.Add(item);
        }
    }

    public bool TryAddToInventory(ItemData data)
    {
        foreach (var slot in uiItems)
            if (slot.itemID == data.itemName && slot.currentQuantity < slot.maxStackSize)
            {
                slot.currentQuantity++; slot.UpdateUI(); return true;
            }
        foreach (var slot in uiItems)
            if (string.IsNullOrEmpty(slot.itemID))
            {
                slot.SetData(data.itemName, data.icon, 1); return true;
            }
        return false;
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
    public bool HasItem(ItemData itemData)
    {
        // Check your list of UI slots
        foreach (var slot in uiItems)
        {
            // Compare the itemID of the slot to the name in the ScriptableObject
            if (slot.itemID == itemData.itemName && slot.currentQuantity > 0)
                return true;
        }
        return false;
    }

    public bool RemoveItem(string itemName, int amount)
    {
        foreach (var slot in uiItems)
        {
            if (slot.itemID == itemName && slot.currentQuantity >= amount)
            {
                slot.currentQuantity -= amount;
                if (slot.currentQuantity <= 0) slot.ResetData();
                else slot.UpdateUI();
                return true;
            }
        }
        return false;
    }
}