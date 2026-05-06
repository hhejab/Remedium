using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public List<Image> hotbarSlots;
    public GameObject weaponPrefab;
    private Item closeItem;
    private float holdTimer = 0f;
    private float requiredHoldTime = 2f;
    private bool hasDropped = false;

    private int selectedSlotIndex = 0;
    public Color highlightColor = Color.yellow; 

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
            if (!hasDropped && hotbarSlots[selectedSlotIndex].sprite != null)
            {
                holdTimer += Time.deltaTime;
                float t = holdTimer / requiredHoldTime;
                hotbarSlots[selectedSlotIndex].color = Color.Lerp(highlightColor, Color.red, t);

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

    void UpdateSlotVisuals()
    {
        for (int i = 0; i < hotbarSlots.Count; i++)
        {
            if (hotbarSlots[i].sprite == null)
            {
                hotbarSlots[i].color = (i == selectedSlotIndex) ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 0f);
            }
            else
            {
                hotbarSlots[i].color = (i == selectedSlotIndex) ? highlightColor : Color.white;
            }

            float scale = (i == selectedSlotIndex) ? 1.25f : 1.0f;
            hotbarSlots[i].rectTransform.localScale = new Vector3(scale, scale, 1);
        }
    }

    void AddToHotbar(Item item)
    {
        foreach (Image slot in hotbarSlots)
        {
            if (slot.sprite == null)
            {
                slot.sprite = item.itemIcon;
                item.PickUp();
                UpdateSlotVisuals();
                return;
            }
        }
    }

    void DropSelectedItem()
    {
        Image slot = hotbarSlots[selectedSlotIndex];
        if (slot.sprite != null)
        {
            Vector3 dropPosition = transform.position + (Vector3)Random.insideUnitCircle * 1.5f;
            GameObject droppedWeapon = Instantiate(weaponPrefab, dropPosition, Quaternion.identity);
            droppedWeapon.transform.localScale = Vector3.one; 

            Item newItemScript = droppedWeapon.GetComponent<Item>();
            if (newItemScript != null) newItemScript.itemIcon = slot.sprite;

            slot.sprite = null;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Weapon")) closeItem = other.GetComponent<Item>();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Weapon")) closeItem = null;
    }
}


