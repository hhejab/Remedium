using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Combat Stats")]
    public int attackDamage = 1;

    [Range(0, 100)]
    public float critChance = 5f;

    public float critDamageMultiplier = 1.5f;

    [Header("Extra Combat Stats")]
    public float attackSpeed = 1f;
    public float attackDamagePercent = 0f;

    [Header("Health Stats")]
    public int maxHealthBonus = 0;
    public float maxHealthPercentBonus = 0f;

    [Header("Defense Stats")]
    public int defense = 0;
    public float damageReductionPercent = 0f;

    [Header("Movement Stats")]
    public float walkSpeedBonus = 0f;
    public float runSpeedBonus = 0f;

    [Header("Stamina Stats")]
    public int maxStaminaBonus = 0;

    public int GetFinalAttackDamage()
    {
        float damage = attackDamage;

        damage += damage * (attackDamagePercent / 100f);

        bool isCrit = Random.Range(0f, 100f) <= critChance;

        if (isCrit)
        {
            damage *= critDamageMultiplier;
            Debug.Log("Critical Hit! Damage: " + damage);
        }

        return Mathf.RoundToInt(damage);
    }

    public int ReduceDamage(int incomingDamage)
    {
        float damage = incomingDamage;

        damage -= defense;
        damage -= damage * (damageReductionPercent / 100f);

        if (damage < 1)
            damage = 1;

        return Mathf.RoundToInt(damage);
    }
}