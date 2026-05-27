using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLootTable", menuName = "Inventory/LootTable")]
public class LootTable : ScriptableObject
{
    public List<ItemData> possibleItems;
    public int minItemsToGenerate = 1;
    public int maxItemsToGenerate = 3;

    public List<ItemData> GenerateRandomLoot()
    {
        List<ItemData> generatedLoot = new List<ItemData>();

        if (possibleItems == null || possibleItems.Count == 0) return generatedLoot;

        int amountToSpawn = Random.Range(minItemsToGenerate, maxItemsToGenerate + 1);

        for (int i = 0; i < amountToSpawn; i++)
        {
            // Picks a random item from your pool
            ItemData randomItem = possibleItems[Random.Range(0, possibleItems.Count)];
            generatedLoot.Add(randomItem);
        }

        return generatedLoot;
    }
}