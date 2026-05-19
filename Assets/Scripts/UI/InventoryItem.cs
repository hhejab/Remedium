using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image itemImage;
    public TextMeshProUGUI quantityText;
    public int maxStackSize = 64;

    [Header("Equipment Rules")]
    public bool isEquipmentSlot = false;
    public EquipSlot designatedSlot = EquipSlot.None;

    [Header("Current Data")]
    public string itemID = ""; 
    public int currentQuantity = 0;
    public Sprite itemIcon;

    private static InventoryItem draggedItem; 
    private Vector3 originalPosition;
    private Transform originalParent;

    private void Awake() => UpdateUI();

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(itemID)) return;
        draggedItem = this;
        originalPosition = itemImage.transform.position;
        originalParent = itemImage.transform.parent;

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            itemImage.transform.SetParent(canvas.transform);
            itemImage.transform.SetAsLastSibling();
        }
        if (itemImage != null) itemImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedItem != null) 
            itemImage.transform.position = Mouse.current.position.ReadValue();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedItem == null) return;
        draggedItem = null;
        itemImage.transform.SetParent(originalParent);
        itemImage.transform.position = originalPosition;
        if (itemImage != null) itemImage.raycastTarget = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventoryItem dropped = eventData.pointerDrag?.GetComponent<InventoryItem>();
        if (dropped == null || dropped == this) return;

        // --- NEW: EQUIPMENT VALIDATION ---
        EquipmentManager eqManager = FindFirstObjectByType<EquipmentManager>();
        if (eqManager != null)
        {
            // If dropping INTO an equipment slot
            if (this.isEquipmentSlot && !string.IsNullOrEmpty(dropped.itemID))
                if (!eqManager.CanEquip(dropped.itemID, this.designatedSlot)) return;

            // If swapping an equipment item BACK into a taken inventory slot
            if (dropped.isEquipmentSlot && !string.IsNullOrEmpty(this.itemID))
                if (!eqManager.CanEquip(this.itemID, dropped.designatedSlot)) return;
        }

        // --- STANDARD DROP LOGIC ---
        if (dropped.itemID == this.itemID && !string.IsNullOrEmpty(itemID))
        {
            int total = this.currentQuantity + dropped.currentQuantity;
            if (total <= maxStackSize)
            {
                this.currentQuantity = total;
                dropped.ResetData();
            }
            else
            {
                this.currentQuantity = maxStackSize;
                dropped.currentQuantity = total - maxStackSize;
                dropped.UpdateUI();
            }
            this.UpdateUI();
        }
        else 
        {
            string oldID = this.itemID;
            Sprite oldIcon = this.itemIcon;
            int oldQty = this.currentQuantity;

            this.SetData(dropped.itemID, dropped.itemIcon, dropped.currentQuantity);
            
            if (!string.IsNullOrEmpty(oldID)) dropped.SetData(oldID, oldIcon, oldQty);
            else dropped.ResetData();
        }
    }

    public void SetData(string id, Sprite sprite, int qty)
    {
        itemID = id; itemIcon = sprite; currentQuantity = qty;
        UpdateUI();
        NotifyManager();
    }

    public void ResetData()
    {
        itemID = ""; itemIcon = null; currentQuantity = 0;
        UpdateUI();
        NotifyManager();
    }

    public void UpdateUI()
    {
        bool hasItem = !string.IsNullOrEmpty(itemID);
        if (itemImage != null)
        {
            itemImage.gameObject.SetActive(true); 
            if (hasItem) {
                itemImage.sprite = itemIcon;
                itemImage.color = Color.white;
            } else {
                itemImage.sprite = null;
                itemImage.color = new Color(0, 0, 0, 0.01f); 
            }
        }
        if (quantityText != null) {
            quantityText.gameObject.SetActive(hasItem && currentQuantity > 1);
            quantityText.text = currentQuantity.ToString();
        }
    }

    private void NotifyManager()
    {
        if (isEquipmentSlot)
        {
            EquipmentManager eqManager = FindFirstObjectByType<EquipmentManager>();
            if (eqManager != null) eqManager.UpdateBuffs();
        }
    }
}