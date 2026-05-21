using UnityEngine;

public enum SkillStatType
{
    AttackSpeed,
    AttackDamagePercent,

    CritChance,
    CritDamage,

    MaxHealthFlat,
    MaxHealthPercent,

    Defense,
    DamageReductionPercent,

    WalkSpeed,
    RunSpeed,
    MaxStamina
}

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill Book/Stat Skill")]
public class SkillData : ScriptableObject
{
    [Header("Info")]
    public string skillName;

    [TextArea(2, 5)]
    public string description;

    public int cost = 1;

    [Header("Upgrade")]
    public SkillStatType statType;
    public float upgradeAmount;

    [Header("Sequential Requirement")]
    public SkillData requiredSkill;

    [Header("UI")]
    public Sprite icon;
}