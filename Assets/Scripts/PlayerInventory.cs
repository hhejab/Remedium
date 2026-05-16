using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public List<Image> hotbarSlots;
    public Color highlightColor = Color.yellow;
    
    private int selectedSlotIndex = 0;

    public void AddToHotbar(Item item)
{
    // Debug to see how many slots the script actually sees
    Debug.Log("Checking " + hotbarSlots.Count + " slots...");

    for (int i = 0; i < hotbarSlots.Count; i++)
    {
        // A slot is truly empty if it has no sprite assigned
        if (hotbarSlots[i].sprite == null)
        {
            hotbarSlots[i].sprite = item.itemIcon;
            hotbarSlots[i].enabled = true; // Ensure the Image component is turned on
            
            // Set the color to solid white so the icon isn't transparent
            hotbarSlots[i].color = Color.white; 

            item.PickUp(); // Destroy the object in the world
            UpdateSlotVisuals();
            return; // Exit the function once we find a spot
        }
    }

    // If we get through the whole loop without returning, the bar is actually full
    Debug.Log("Hotbar is actually full!");
}

    // Keep your existing UpdateSlotVisuals and HandleSlotSelection here...
    void UpdateSlotVisuals()
{
    for (int i = 0; i < hotbarSlots.Count; i++)
    {
        bool isSelected = (i == selectedSlotIndex);
        
        if (hotbarSlots[i].sprite == null)
        {
            // If slot is empty, make it transparent or a faint highlight
            hotbarSlots[i].color = isSelected ? new Color(1, 1, 1, 0.3f) : new Color(1, 1, 1, 0f);
        }
        else
        {
            // If there is an item, keep it white (original icon color)
            // ONLY tint the border or use a slight glow if you want to show selection
            hotbarSlots[i].color = Color.white; 
            
            // If you want a highlight, apply it to a SEPARATE border object 
            // instead of the icon itself.
        }

        // Scale up the selected slot so the player knows which one is active
        float scale = isSelected ? 1.25f : 1.0f;
        hotbarSlots[i].rectTransform.localScale = new Vector3(scale, scale, 1);
    }
}
}