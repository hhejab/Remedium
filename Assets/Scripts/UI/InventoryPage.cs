using System.Collections.Generic;
using UnityEngine;

public class InventoryPage : MonoBehaviour
{
    [SerializeField] private InventoryItem itemPrefab;
    [SerializeField] private RectTransform contentPanel;
    private List<InventoryItem> uiItems = new List<InventoryItem>();

    public void InitializeInventoryUI(int size)
    {
        foreach(Transform child in contentPanel) Destroy(child.gameObject);
        uiItems.Clear();
        for (int i = 0; i < size; i++) {
            InventoryItem item = Instantiate(itemPrefab, contentPanel);
            item.ResetData();
            uiItems.Add(item);
        }
    }

    public bool TryAddToInventory(string id, Sprite icon)
    {
        foreach (var slot in uiItems)
            if (slot.itemID == id && slot.currentQuantity < slot.maxStackSize) {
                slot.currentQuantity++; slot.UpdateUI(); return true;
            }
        foreach (var slot in uiItems)
            if (string.IsNullOrEmpty(slot.itemID)) {
                slot.SetData(id, icon, 1); return true;
            }
        return false;
    }

    public bool HasItem(string id)
{
    foreach (var slot in uiItems)
        if (slot.itemID == id && slot.currentQuantity > 0)
            return true;

    return false;
}

public bool RemoveItem(string id, int amount = 1)
{
    foreach (var slot in uiItems)
    {
        if (slot.itemID == id && slot.currentQuantity >= amount)
        {
            slot.currentQuantity -= amount;

            if (slot.currentQuantity <= 0)
                slot.ResetData();
            else
                slot.UpdateUI();

            return true;
        }
    }

    return false;
}

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}