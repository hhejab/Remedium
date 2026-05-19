using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Boss Info")]
    public string bossName = "Boss";
    public int maxHealth = 150;

    [Header("UI")]
    public BossUI bossUI;

    protected int currentHealth;
    protected Boss_AI bossAI;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        bossAI = GetComponent<Boss_AI>();
    }

    protected virtual void Start()
    {
        if (bossUI != null)
        {
            bossUI.SetBossName(bossName);
            bossUI.SetHealth(currentHealth, maxHealth);
            bossUI.Show();
        }
        else
        {
            Debug.LogWarning("BossHealth: BossUI is not assigned.");
        }
    }

    public virtual void TakeDamage(int damage)
    {
        if (!CanTakeDamage())
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log(bossName + " took damage: " + damage + " HP: " + currentHealth + " / " + maxHealth);

        UpdateBossUI();

        OnAfterDamage(damage);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        if (bossAI != null)
            bossAI.PlayHurt();
    }

    protected virtual bool CanTakeDamage()
    {
        return currentHealth > 0;
    }

    protected virtual void OnAfterDamage(int damage)
    {
        // Child boss health scripts can override this.
    }

    protected virtual void UpdateBossUI()
    {
        if (bossUI != null)
            bossUI.SetHealth(currentHealth, maxHealth);
    }

    protected virtual void Die()
    {
        if (bossUI != null)
            bossUI.Hide();

        if (bossAI != null)
            bossAI.Die();
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetHealthPercent()
    {
        return (float)currentHealth / maxHealth;
    }
}