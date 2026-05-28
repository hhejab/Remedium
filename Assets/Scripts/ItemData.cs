using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string itemID;
    public Sprite icon;
    [TextArea(2, 4)]
    public string description;

    // Add things like 'public int healAmount;' later if needed
}