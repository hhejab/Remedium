using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillBookManager : MonoBehaviour
{
    [Header("Player")]
    public PlayerStats playerStats;
    public PlayerHealth playerHealth;
    public PlayerSkillPointWallet wallet;

    [Header("Skill Points UI")]
    public TMP_Text skillPointsValueText;

    [Header("Right Page Text")]
    public TMP_Text playerStatsText;
    public TMP_Text skillDescriptionText;

    [Header("Skill Buttons")]
    public SkillButtonUI[] skillButtons;

    private HashSet<SkillData> unlockedSkills = new HashSet<SkillData>();

    private void Awake()
    {
        FindReferences();
    }

    private void Start()
    {
        FindReferences();

        foreach (SkillButtonUI skillButton in skillButtons)
        {
            if (skillButton != null)
                skillButton.Setup(this);
        }

        RefreshAll();
        ClearSkillDescription();
    }

    private void Update()
    {
        FindReferences();
        UpdatePlayerStatsText();
        UpdateSkillPointsText();
    }

    private void FindReferences()
    {
        if (playerStats == null)
            playerStats = FindFirstObjectByType<PlayerStats>();

        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (wallet == null)
            wallet = FindFirstObjectByType<PlayerSkillPointWallet>();
    }

    public bool IsUnlocked(SkillData skill)
    {
        return skill != null && unlockedSkills.Contains(skill);
    }

    public bool CanUnlock(SkillData skill)
    {
        FindReferences();

        if (skill == null) return false;

        if (IsUnlocked(skill))
            return false;

        if (wallet == null || wallet.skillPoints < skill.cost)
            return false;

        if (skill.requiredSkill != null && !IsUnlocked(skill.requiredSkill))
            return false;

        return true;
    }

    public void UnlockSkill(SkillData skill)
    {
        FindReferences();

        if (!CanUnlock(skill))
        {
            ShowSkillDescription(skill);

            if (skill != null)
                Debug.Log("Cannot unlock: " + skill.skillName);

            return;
        }

        if (wallet == null || !wallet.SpendSkillPoints(skill.cost))
            return;

        unlockedSkills.Add(skill);

        ApplySkill(skill);

        RefreshAll();
        ShowSkillDescription(skill);

        Debug.Log("Unlocked skill: " + skill.skillName);
    }

    private void ApplySkill(SkillData skill)
    {
        FindReferences();

        if (playerStats == null || skill == null)
        {
            Debug.LogWarning("PlayerStats or SkillData missing in SkillBookManager.");
            return;
        }

        switch (skill.statType)
        {
            case SkillStatType.AttackSpeed:
                playerStats.attackSpeed += skill.upgradeAmount;
                break;

            case SkillStatType.AttackDamagePercent:
                playerStats.attackDamagePercent += skill.upgradeAmount;
                break;

            case SkillStatType.CritChance:
                playerStats.critChance += skill.upgradeAmount;
                playerStats.critChance = Mathf.Clamp(playerStats.critChance, 0f, 100f);
                break;

            case SkillStatType.CritDamage:
                playerStats.critDamageMultiplier += skill.upgradeAmount;
                break;

            case SkillStatType.MaxHealthFlat:
                playerStats.maxHealthBonus += Mathf.RoundToInt(skill.upgradeAmount);

                if (playerHealth != null)
                    playerHealth.AddMaxHealthAndHeal(Mathf.RoundToInt(skill.upgradeAmount));
                break;

            case SkillStatType.MaxHealthPercent:
                playerStats.maxHealthPercentBonus += skill.upgradeAmount;

                if (playerHealth != null)
                {
                    int healAmount = Mathf.RoundToInt(playerHealth.MaxHealth * (skill.upgradeAmount / 100f));
                    playerHealth.AddMaxHealthAndHeal(healAmount);
                }
                break;

            case SkillStatType.Defense:
                playerStats.defense += Mathf.RoundToInt(skill.upgradeAmount);
                break;

            case SkillStatType.DamageReductionPercent:
                playerStats.damageReductionPercent += skill.upgradeAmount;
                break;

            case SkillStatType.WalkSpeed:
                playerStats.walkSpeedBonus += skill.upgradeAmount;
                break;

            case SkillStatType.RunSpeed:
                playerStats.runSpeedBonus += skill.upgradeAmount;
                break;

            case SkillStatType.MaxStamina:
                playerStats.maxStaminaBonus += Mathf.RoundToInt(skill.upgradeAmount);
                break;
        }
    }

    public void RefreshAll()
    {
        FindReferences();

        UpdateSkillPointsText();
        UpdatePlayerStatsText();

        foreach (SkillButtonUI skillButton in skillButtons)
        {
            if (skillButton != null)
                skillButton.Refresh();
        }
    }

    private void UpdateSkillPointsText()
    {
        FindReferences();

        if (skillPointsValueText != null)
            skillPointsValueText.text = wallet != null ? wallet.skillPoints.ToString() : "0";
    }

    private void UpdatePlayerStatsText()
    {
        if (playerStats == null || playerStatsText == null) return;

        string healthText = "N/A";

        if (playerHealth != null)
            healthText = playerHealth.currentHealth + " / " + playerHealth.GetFinalMaxHealth();

        playerStatsText.text =
            "Attack Damage %: " + playerStats.attackDamagePercent +
            " Attack Speed: " + playerStats.attackSpeed + "\n" +
            "Crit Chance: " + playerStats.critChance +
            " Crit Damage: " + playerStats.critDamageMultiplier + "x\n" +
            "Health: " + healthText + "\n" +
            "Defense: " + playerStats.defense +
            " Damage Reduction: " + playerStats.damageReductionPercent + "%\n" +
            "Walk Speed Bonus: " + playerStats.walkSpeedBonus +
            " Run Speed Bonus: " + playerStats.runSpeedBonus +
            " Stamina Bonus: " + playerStats.maxStaminaBonus;
    }

    public void ShowSkillDescription(SkillData skill)
    {
        if (skillDescriptionText == null || skill == null) return;

        string requirementText = "";

        if (skill.requiredSkill != null && !IsUnlocked(skill.requiredSkill))
            requirementText = "\nRequires: " + skill.requiredSkill.skillName;

        string statusText = "";

        if (IsUnlocked(skill))
            statusText = "\nStatus: Unlocked";
        else if (CanUnlock(skill))
            statusText = "\nStatus: Available";
        else
            statusText = "\nStatus: Locked";

        skillDescriptionText.text =
            skill.skillName + "\n" +
            skill.description + "\n" +
            "Cost: " + skill.cost +
            requirementText +
            statusText;
    }

    public void ClearSkillDescription()
    {
        if (skillDescriptionText != null)
            skillDescriptionText.text = "Hover over a skill to view details.";
    }
}