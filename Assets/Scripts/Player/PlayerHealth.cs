using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth;
    public int MaxHealth = 100;
    public Image healthBarFill;

    private PlayerStats playerStats;
    private bool isDead;

    void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    void Start()
    {
        if (currentHealth <= 0)
            FullHeal();
        else
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
        if (isDead) return;

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
            Die();
    }

    public void FullHeal()
    {
        isDead = false;
        currentHealth = GetFinalMaxHealth();
        UpdateUI();
    }

    public void AddMaxHealthAndHeal(int amount)
    {
        isDead = false;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, GetFinalMaxHealth());
        UpdateUI();
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        PlayerRespawnManager respawn = GetComponent<PlayerRespawnManager>();

        if (respawn != null)
        {
            respawn.RespawnFromDeath();
            return;
        }

        gameObject.SetActive(false);
    }

    void UpdateUI()
    {
        int finalMaxHealth = GetFinalMaxHealth();

        if (healthBarFill != null)
            healthBarFill.fillAmount = (float)currentHealth / finalMaxHealth;
    }
}