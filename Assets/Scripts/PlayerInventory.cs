
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

    public List<Image> hotbarSlots;
    public Color highlightColor = Color.yellow;
    public GameObject weaponPrefab;
    
    private int selectedSlotIndex = 0;
    private Item closeItem;
    private float holdTimer = 0f;
    private float requiredHoldTime = 2f;
    private bool hasDropped = false;
    private Vector3[] savedRealScales = new Vector3[10];
    private Sprite[] savedWorldSprites = new Sprite[10];

    void Start()
    {
        UpdateSlotVisuals();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        HandleSlotSelection();

        if (Keyboard.current.eKey.wasPressedThisFrame && closeItem != null)
        {
            AddToHotbar(closeItem);
        }

        if (Keyboard.current.qKey.isPressed)
        {
            if (!hasDropped && hotbarSlots[selectedSlotIndex] != null && hotbarSlots[selectedSlotIndex].sprite != null)
            {
                holdTimer += Time.deltaTime;
                float t = holdTimer / requiredHoldTime;
                hotbarSlots[selectedSlotIndex].color = Color.Lerp(Color.white, Color.red, t);

                if (holdTimer >= requiredHoldTime)
                {
                    DropSelectedItem();
                    hasDropped = true;
                    holdTimer = 0f;
                    UpdateSlotVisuals();
                }
            }
        }
        
        if (Keyboard.current.qKey.wasReleasedThisFrame)
        {
            holdTimer = 0f;
            hasDropped = false;
            UpdateSlotVisuals();
        }
    }

    void HandleSlotSelection()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SetSelectedSlot(0);
        else if (Keyboard.current.digit2Key.wasPressedThisFrame) SetSelectedSlot(1);
        else if (Keyboard.current.digit3Key.wasPressedThisFrame) SetSelectedSlot(2);
        else if (Keyboard.current.digit4Key.wasPressedThisFrame) SetSelectedSlot(3);
        else if (Keyboard.current.digit5Key.wasPressedThisFrame) SetSelectedSlot(4);
        else if (Keyboard.current.digit6Key.wasPressedThisFrame) SetSelectedSlot(5);
        else if (Keyboard.current.digit7Key.wasPressedThisFrame) SetSelectedSlot(6);
        else if (Keyboard.current.digit8Key.wasPressedThisFrame) SetSelectedSlot(7);
        else if (Keyboard.current.digit9Key.wasPressedThisFrame) SetSelectedSlot(8);
        else if (Keyboard.current.digit0Key.wasPressedThisFrame) SetSelectedSlot(9);
    }

    void SetSelectedSlot(int index)
    {
        if (index < hotbarSlots.Count)
        {
            selectedSlotIndex = index;
            holdTimer = 0f;
            UpdateSlotVisuals();
        }
    }

    public void AddToHotbar(Item item)
    {
        for (int i = 0; i < hotbarSlots.Count; i++)
        {
            if (hotbarSlots[i].sprite == null)
            {
                hotbarSlots[i].sprite = item.itemIcon;
                hotbarSlots[i].color = Color.white; 
                
                savedRealScales[i] = item.transform.localScale;

                SpriteRenderer itemSR = item.GetComponent<SpriteRenderer>();
                if (itemSR != null)
                {
                    savedWorldSprites[i] = itemSR.sprite;
                }

                item.PickUp();
                UpdateSlotVisuals();
                return;
            }
        }
    }

    void DropSelectedItem()
    {
        if (weaponPrefab == null) return;

        Image slot = hotbarSlots[selectedSlotIndex];
        if (slot != null && slot.sprite != null)
        {
            Vector2 randomCircle = Random.insideUnitCircle * 1.5f;
            Vector3 dropPosition = new Vector3(transform.position.x + randomCircle.x, transform.position.y + randomCircle.y, 0f);
            
            GameObject droppedWeapon = Instantiate(weaponPrefab, dropPosition, Quaternion.identity);
            droppedWeapon.name = "Dropped_Weapon";
            
            if (savedRealScales[selectedSlotIndex] != Vector3.zero)
            {
                droppedWeapon.transform.localScale = savedRealScales[selectedSlotIndex];
            }
            else
            {
                droppedWeapon.transform.localScale = weaponPrefab.transform.localScale;
            }

            Item newItemScript = droppedWeapon.GetComponent<Item>();
            if (newItemScript != null)
            {
                newItemScript.itemIcon = slot.sprite;
            }

            SpriteRenderer sr = droppedWeapon.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                if (savedWorldSprites[selectedSlotIndex] != null)
                {
                    sr.sprite = savedWorldSprites[selectedSlotIndex];
                }
                else
                {
                    sr.sprite = slot.sprite;
                }
                sr.sortingOrder = 5; 
                sr.color = Color.white;
            }

            slot.sprite = null;
            savedWorldSprites[selectedSlotIndex] = null;
        }
    }

    void UpdateSlotVisuals()
    {
        for (int i = 0; i < hotbarSlots.Count; i++)
        {
            bool isSelected = (i == selectedSlotIndex);
            
            if (hotbarSlots[i].sprite == null)
            {
                hotbarSlots[i].color = isSelected ? highlightColor : new Color(1, 1, 1, 0.1f);
            }
            else
            {
                hotbarSlots[i].color = isSelected ? highlightColor : Color.white; 
            }

            float scale = isSelected ? 1.2f : 1.0f;
            hotbarSlots[i].rectTransform.localScale = new Vector3(scale, scale, 1);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Weapon")) closeItem = other.GetComponent<Item>();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Weapon")) closeItem = null;
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