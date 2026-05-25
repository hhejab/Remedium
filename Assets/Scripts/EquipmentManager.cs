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

[System.Serializable]
public class StartingGearItem
{
    public string itemID;
    public Sprite itemIcon;
}

[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerHealth))]
public class EquipmentManager : MonoBehaviour
{
    [Header("Item Database")]
    [Tooltip("Add rows here to define all items across all 3 tiers (Basic + Tier 1 + Tier 2).")]
    public List<EquipItemData> itemDatabase = new List<EquipItemData>();

    [Header("Starting Gear (Basic Tier)")]
    [Tooltip("Configure the item details the player will start the game wearing.")]
    public StartingGearItem startingHead;
    public StartingGearItem startingChest;
    public StartingGearItem startingLegs;
    public StartingGearItem startingWeapon;

    [Header("UI Equipment Slots")]
    public InventoryItem headSlot;
    public InventoryItem chestSlot;
    public InventoryItem legsSlot;
    public InventoryItem weaponSlot;

    private PlayerStats playerStats;
    private PlayerHealth playerHealth;

    // Track active bonuses to safely apply changes as differences (deltas)
    private int currentBonusAtk = 0;
    private int currentBonusDef = 0;
    private int currentBonusHp = 0;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Start()
    {
        // Automatically give the player their basic gear at startup
        AutoEquipStartingItem(headSlot, startingHead);
        AutoEquipStartingItem(chestSlot, startingChest);
        AutoEquipStartingItem(legsSlot, startingLegs);
        AutoEquipStartingItem(weaponSlot, startingWeapon);

        // Force a buff check calculation for starting gear parameters
        UpdateBuffs();
    }

    private void AutoEquipStartingItem(InventoryItem slot, StartingGearItem gear)
    {
        if (slot != null && gear != null && !string.IsNullOrEmpty(gear.itemID))
        {
            // Initializing a default single count stack in the equipment slot
            slot.SetData(gear.itemID, gear.itemIcon, 1);
        }
    }

    public bool CanEquip(string id, EquipSlot slot)
    {
        if (string.IsNullOrEmpty(id)) return true; 

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

    public void UpdateBuffs()
    {
        int newAtk = 0;
        int newDef = 0;
        int newHp = 0;

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
                return; 
            }
        }
    }

    private void ApplyDeltaToPlayer(int newAtk, int newDef, int newHp)
    {
        if (playerStats != null)
        {
            int atkDelta = newAtk - currentBonusAtk;
            int defDelta = newDef - currentBonusDef;
            int hpDelta = newHp - currentBonusHp;

            playerStats.attackDamage += atkDelta;
            playerStats.defense += defDelta;
            playerStats.maxHealthBonus += hpDelta;

            Debug.Log($"[EquipmentManager] Buffs Updated! Deltas: ATK: {atkDelta:+#;-#;0} | DEF: {defDelta:+#;-#;0} | HP: {hpDelta:+#;-#;0}");
        }

        if (playerHealth != null)
        {
            playerHealth.ChangeHealth(0); // Recalculates Max Health UI safely
        }

        currentBonusAtk = newAtk;
        currentBonusDef = newDef;
        currentBonusHp = newHp;
    }
}