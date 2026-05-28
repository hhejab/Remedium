using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public List<InventoryItem> hotbarSlots;
    public List<Image> hotbarImages;
    public Color highlightColor = Color.yellow;
    public GameObject weaponPrefab;

    private int selectedSlotIndex = 0;
    private Item closeItem;
    private float holdTimer = 0f;
    private float requiredHoldTime = 2f;
    private bool hasDropped = false;

    void Start() { UpdateSlotVisuals(); }

    void Update()
    {
        if (Keyboard.current == null) return;
        HandleSlotSelection();

        if (Keyboard.current.eKey.wasPressedThisFrame && closeItem != null)
            AddToHotbar(closeItem);

        // Drop logic
        if (Keyboard.current.qKey.isPressed && selectedSlotIndex < hotbarImages.Count)
        {
            if (!hasDropped && hotbarImages[selectedSlotIndex].sprite != null)
            {
                holdTimer += Time.deltaTime;
                float t = holdTimer / requiredHoldTime;
                hotbarImages[selectedSlotIndex].color = Color.Lerp(Color.white, Color.red, t);
                if (holdTimer >= requiredHoldTime) { DropSelectedItem(); hasDropped = true; holdTimer = 0f; UpdateSlotVisuals(); }
            }
        }
        if (Keyboard.current.qKey.wasReleasedThisFrame) { holdTimer = 0f; hasDropped = false; UpdateSlotVisuals(); }
    }

    public void AddToHotbar(Item item)
    {
        if (hotbarImages == null || item == null) return;
        // Logic to update UI using item.myItemData
        TryAddToHotbar(item.myItemData);
        item.PickUp();
        UpdateSlotVisuals();
    }

    public bool TryAddToHotbar(ItemData data)
{
    foreach (var slot in hotbarSlots)
    {
        if (slot != null && slot.itemID == data.itemID && slot.currentQuantity < slot.maxStackSize)
        {
            slot.currentQuantity++;
            slot.UpdateUI();
            return true;
        }
    }

    foreach (var slot in hotbarSlots)
    {
        if (slot != null && string.IsNullOrEmpty(slot.itemID))
        {
            slot.SetData(data.itemID, data.icon, 1);
            return true;
        }
    }

    return false;
}

    void DropSelectedItem()
    {
        // Instantiates weaponPrefab and assigns data
        GameObject droppedWeapon = Instantiate(weaponPrefab, transform.position + (Vector3)Random.insideUnitCircle * 1.5f, Quaternion.identity);
        Item itemScript = droppedWeapon.GetComponent<Item>();
        if (itemScript != null) { /* Assign data here */ }
        // Clear hotbar slot...
    }

    void HandleSlotSelection() { /* ...Existing input logic... */ }
    void SetSelectedSlot(int index) { selectedSlotIndex = index; UpdateSlotVisuals(); }
    void UpdateSlotVisuals() { /* ...Existing visual logic... */ }
    void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Weapon")) closeItem = other.GetComponent<Item>(); }
    void OnTriggerExit2D(Collider2D other) { if (other.CompareTag("Weapon")) closeItem = null; }
}