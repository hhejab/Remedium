using UnityEngine;
using System.Collections.Generic;

public class Chest : MonoBehaviour, IInteractable
{
    public enum ChestType { FixedLoot, RandomizedLoot }

    [Header("Loot Configuration")]
    public ChestType myChestType;
    public List<ItemData> guaranteedItems;
    public LootTable myLootTable;

    public List<ItemData> currentInventory = new List<ItemData>();
    private bool hasBeenGenerated = false;

    public void Interact() { /* Handled by ChestUIController */ }

    public void GenerateInventory()
    {
        if (hasBeenGenerated) return;

        if (myChestType == ChestType.FixedLoot)
            currentInventory = new List<ItemData>(guaranteedItems);
        else if (myChestType == ChestType.RandomizedLoot && myLootTable != null)
            currentInventory = myLootTable.GenerateRandomLoot();

        hasBeenGenerated = true;
    }
}