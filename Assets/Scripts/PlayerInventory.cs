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
        if (Keyboard.current == null) return;

        if (Keyboard.current.eKey.wasPressedThisFrame && closeItem != null)
        {
            AddToHotbar(closeItem);
        }

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            DropFirstItem();
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

    
          void DropFirstItem()
    {
        for (int i = 0; i < hotbarSlots.Count; i++)
        {
            if (hotbarSlots[i].sprite != null && hotbarSlots[i].color.a > 0)
            {
                Vector3 dropPosition = transform.position + (Vector3)Random.insideUnitCircle * 1.5f;
                GameObject droppedWeapon = Instantiate(weaponPrefab, dropPosition, Quaternion.identity);
                
                droppedWeapon.transform.localScale = Vector3.one; 

                Item newItemScript = droppedWeapon.GetComponent<Item>();
                if (newItemScript != null)
                {
                    newItemScript.itemIcon = hotbarSlots[i].sprite;
                }

                hotbarSlots[i].sprite = null;
                hotbarSlots[i].color = new Color(1, 1, 1, 0);
                return; 
            }
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

