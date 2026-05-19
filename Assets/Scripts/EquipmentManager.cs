using UnityEngine;
using System.Collections.Generic;

public enum EquipSlot { None, Head, Chest, Legs, Weapon }

[System.Serializable]
public class EquipItemData
{
    [Tooltip("Must match the itemID exactly (e.g., 'IronSword')")]
    public string itemID; 
    public EquipSlot equipType;

    [Header("Buffs")]
    public int bonusAttack;
    public int bonusDefense;
    public int bonusHealth;
}

public class EquipmentManager : MonoBehaviour
{
    [Header("Item Database")]
    public List<EquipItemData> itemDatabase = new List<EquipItemData>();

    [Header("UI Slots")]
    public InventoryItem headSlot;
    public InventoryItem chestSlot;
    public InventoryItem legsSlot;
    public InventoryItem weaponSlot;

    // We track current bonuses to safely add/subtract deltas when swapping gear
    private int currentBonusAtk = 0;
    private int currentBonusDef = 0;
    private int currentBonusHp = 0;

    // Checks if an item is allowed in a specific slot
    public bool CanEquip(string id, EquipSlot slot)
    {
        if (string.IsNullOrEmpty(id)) return true;
        foreach (var item in itemDatabase)
        {
            if (item.itemID == id && item.equipType == slot) return true;
        }
        Debug.LogWarning($"Item '{id}' is not a valid {slot}!");
        return false;
    }

    // Called automatically by the UI slots when items change
    public void UpdateBuffs()
    {
        int newAtk = 0, newDef = 0, newHp = 0;

        TallyBuffs(headSlot, ref newAtk, ref newDef, ref newHp);
        TallyBuffs(chestSlot, ref newAtk, ref newDef, ref newHp);
        TallyBuffs(legsSlot, ref newAtk, ref newDef, ref newHp);
        TallyBuffs(weaponSlot, ref newAtk, ref newDef, ref newHp);

        ApplyToPlayerStats(newAtk, newDef, newHp);
    }

    private void TallyBuffs(InventoryItem slot, ref int atk, ref int def, ref int hp)
    {
        if (slot == null || string.IsNullOrEmpty(slot.itemID)) return;

        foreach (var item in itemDatabase)
        {
            if (item.itemID == slot.itemID)
            {
                atk += item.bonusAttack;
                def += item.bonusDefense;
                hp += item.bonusHealth;
                return;
            }
        }
    }

    private void ApplyToPlayerStats(int newAtk, int newDef, int newHp)
    {
        Debug.Log($"New Equipment Buffs -> ATK: +{newAtk} | DEF: +{newDef} | HP: +{newHp}");

        // --- CONNECT TO YOUR STATS SCRIPT HERE ---
        // Based on your scene setup, I've mapped this to your stat variables.
        // Uncomment the lines below and rename "YourPlayerStatsScript" to your actual script name!
        
        /*
        YourPlayerStatsScript stats = GetComponent<YourPlayerStatsScript>();
        if (stats != null)
        {
            // We use a delta (new - current) so taking off an item removes the specific buff
            stats.attackDamage += (newAtk - currentBonusAtk);
            stats.defense += (newDef - currentBonusDef);
            stats.maxHealthBonus += (newHp - currentBonusHp);
        }
        */

        currentBonusAtk = newAtk;
        currentBonusDef = newDef;
        currentBonusHp = newHp;
    }
}