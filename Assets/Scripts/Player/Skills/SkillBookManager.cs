using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillBookManager : MonoBehaviour
{
    [Header("Player")]
    public PlayerStats playerStats;
    public PlayerHealth playerHealth;

    [Header("Skill Points")]
    public int skillPoints = 10;
    public TMP_Text skillPointsValueText;

    [Header("Right Page Text")]
    public TMP_Text playerStatsText;
    public TMP_Text skillDescriptionText;

    [Header("Skill Buttons")]
    public SkillButtonUI[] skillButtons;

    private HashSet<SkillData> unlockedSkills = new HashSet<SkillData>();

    private void Start()
    {
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
        UpdatePlayerStatsText();
        UpdateSkillPointsText();
    }

    public bool IsUnlocked(SkillData skill)
    {
        return unlockedSkills.Contains(skill);
    }

    public bool CanUnlock(SkillData skill)
    {
        if (skill == null) return false;

        if (IsUnlocked(skill))
            return false;

        if (skillPoints < skill.cost)
            return false;

        if (skill.requiredSkill != null && !IsUnlocked(skill.requiredSkill))
            return false;

        return true;
    }

    public void UnlockSkill(SkillData skill)
    {
        if (!CanUnlock(skill))
        {
            ShowSkillDescription(skill);
            Debug.Log("Cannot unlock: " + skill.skillName);
            return;
        }

        skillPoints -= skill.cost;
        unlockedSkills.Add(skill);

        ApplySkill(skill);

        RefreshAll();
        ShowSkillDescription(skill);

        Debug.Log("Unlocked skill: " + skill.skillName);
    }

    private void ApplySkill(SkillData skill)
    {
        if (playerStats == null)
        {
            Debug.LogWarning("PlayerStats missing in SkillBookManager.");
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

    private void RefreshAll()
    {
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
        if (skillPointsValueText != null)
            skillPointsValueText.text = skillPoints.ToString();
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
            skill.skillName + 
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