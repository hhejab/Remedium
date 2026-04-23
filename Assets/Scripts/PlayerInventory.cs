using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public List<Image> hotbarSlots;
    public GameObject weaponPrefab;
    private Item closeItem;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame && closeItem != null)
        {
            AddToHotbar(closeItem);
        }
    }

    void AddToHotbar(Item item)
    {
        foreach (Image slot in hotbarSlots)
        {
            if (slot.sprite == null || slot.color.a == 0)
            {
                slot.sprite = item.itemIcon;
                slot.color = Color.white;
                item.PickUp();
                return;
            }
        }
    }

    public void RemoveFromHotbar(int slotIndex)
    {
        if (hotbarSlots[slotIndex].sprite != null)
        {
            Vector3 dropPosition = transform.position + (Vector3)Random.insideUnitCircle * 1.5f;
            GameObject droppedWeapon = Instantiate(weaponPrefab, dropPosition, Quaternion.identity);
            
            Sprite currentSprite = hotbarSlots[slotIndex].sprite;
            droppedWeapon.GetComponent<SpriteRenderer>().sprite = currentSprite;
            droppedWeapon.GetComponent<Item>().itemIcon = currentSprite;
            droppedWeapon.name = "DroppedItem"; 

            hotbarSlots[slotIndex].sprite = null;
            hotbarSlots[slotIndex].color = new Color(1, 1, 1, 0);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Weapon"))
        {
            closeItem = other.GetComponent<Item>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Weapon"))
        {
            closeItem = null;
        }
    }
}
