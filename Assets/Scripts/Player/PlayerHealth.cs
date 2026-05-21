using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth;
    public int MaxHealth = 100;
    public Image healthBarFill;

    private PlayerStats playerStats;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        currentHealth = GetFinalMaxHealth();
        UpdateUI();
    }

    public int GetFinalMaxHealth()
    {
        int finalMaxHealth = MaxHealth;

        if (playerStats != null)
        {
             finalMaxHealth += playerStats.maxHealthBonus;
            finalMaxHealth += Mathf.RoundToInt(MaxHealth * (playerStats.maxHealthPercentBonus / 100f));
        }
        return finalMaxHealth;
    }

    public void ChangeHealth(int amount)
    {
        int finalMaxHealth = GetFinalMaxHealth();

        if (amount < 0 && playerStats != null)
        {
            int incomingDamage = Mathf.Abs(amount);
            int reducedDamage = playerStats.ReduceDamage(incomingDamage);
            amount = -reducedDamage;
        }

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, finalMaxHealth);

        UpdateUI();

        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }

        public void AddMaxHealthAndHeal(int amount)
    {
        currentHealth += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
       int finalMaxHealth = GetFinalMaxHealth();

        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / finalMaxHealth;
        }
    }
}


