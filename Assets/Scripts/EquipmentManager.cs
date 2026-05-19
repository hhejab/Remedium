using UnityEngine;
using System.Collections.Generic;

public enum EquipSlot { None, Head, Chest, Legs, Weapon }

[System.Serializable]
public class EquipItemData
{
    [Tooltip("Must match the itemID string used in your InventoryItem exactly (case-sensitive).")]
    public string itemID; 
    public EquipSlot equipType;

    [Header("Buffs / Modifiers")]
    public int bonusAttack;
    public int bonusDefense;
    public int bonusHealth;
}

[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerHealth))]
public class EquipmentManager : MonoBehaviour
{
    [Header("Item Database")]
    [Tooltip("Add rows here to define what stats your armor and weapon items grant.")]
    public List<EquipItemData> itemDatabase = new List<EquipItemData>();

    [Header("UI Equipment Slots")]
    [Tooltip("Drag your equipment UI Slot GameObjects (with the InventoryItem script) here.")]
    public InventoryItem headSlot;
    public InventoryItem chestSlot;
    public InventoryItem legsSlot;
    public InventoryItem weaponSlot;

    private PlayerStats playerStats;
    private PlayerHealth playerHealth;

    // Track current active bonuses to safely apply changes as deltas (differences)
    private int currentBonusAtk = 0;
    private int currentBonusDef = 0;
    private int currentBonusHp = 0;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    /// <summary>
    /// Validates if an item is allowed to be placed inside a specific equipment UI slot.
    /// </summary>
    public bool CanEquip(string id, EquipSlot slot)
    {
        if (string.IsNullOrEmpty(id)) return true; // Empty item slots are always valid

        foreach (var item in itemDatabase)
        {
            if (item.itemID == id)
            {
                return item.equipType == slot;
            }
        }
        
        Debug.LogWarning($"Item '{id}' is not configured in the Equipment Database!");
        return false;
    }

    /// <summary>
    /// Scans all active equipment slots, totals their buffs, and safely updates Player stats.
    /// Called automatically by InventoryItem when an item is placed or removed.
    /// </summary>
    public void UpdateBuffs()
    {
        int newAtk = 0;
        int newDef = 0;
        int newHp = 0;

        // Collect stats from all four slots
        TallySlotBuffs(headSlot, ref newAtk, ref newDef, ref newHp);
        TallySlotBuffs(chestSlot, ref newAtk, ref newDef, ref newHp);
        TallySlotBuffs(legsSlot, ref newAtk, ref newDef, ref newHp);
        TallySlotBuffs(weaponSlot, ref newAtk, ref newDef, ref newHp);

        ApplyDeltaToPlayer(newAtk, newDef, newHp);
    }

    private void TallySlotBuffs(InventoryItem slot, ref int atk, ref int def, ref int hp)
    {
        if (slot == null || string.IsNullOrEmpty(slot.itemID)) return;

        foreach (var item in itemDatabase)
        {
            if (item.itemID == slot.itemID)
            {
                atk += item.bonusAttack;
                def += item.bonusDefense;
                hp += item.bonusHealth;
                return; // Match found, exit loop for this slot
            }
        }
    }

    private void ApplyDeltaToPlayer(int newAtk, int newDef, int newHp)
    {
        if (playerStats != null)
        {
            // Calculate differences between the new gear totals and what was previously tracked
            int atkDelta = newAtk - currentBonusAtk;
            int defDelta = newDef - currentBonusDef;
            int hpDelta = newHp - currentBonusHp;

            // Apply directly to your actual PlayerStats variables
            playerStats.attackDamage += atkDelta;
            playerStats.defense += defDelta;
            playerStats.maxHealthBonus += hpDelta;

            Debug.Log($"[EquipmentManager] Buffs Updated! Deltas applied -> ATK: {atkDelta:+#;-#;0} | DEF: {defDelta:+#;-#;0} | HP: {hpDelta:+#;-#;0}");
        }

        if (playerHealth != null)
        {
            // Calling ChangeHealth(0) forces PlayerHealth to automatically run GetFinalMaxHealth(),
            // update its structural health bar fill parameters, and clamp seamlessly 
            // without harming or artificially resetting the player's current health.
            playerHealth.ChangeHealth(0);
        }

        // Cache the newly applied totals so they act as the base comparison for the next swap
        currentBonusAtk = newAtk;
        currentBonusDef = newDef;
        currentBonusHp = newHp;
    }
}