using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Boss Info")]
    public string bossName = "Beholder";
    public int maxHealth = 150;

    [Header("UI")]
    public BossUI bossUI;

    private int currentHealth;
    private BossAI bossAI;

    private void Awake()
    {
        currentHealth = maxHealth;
        bossAI = GetComponent<BossAI>();
    }

    private void Start()
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

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Boss took damage: " + damage + " HP: " + currentHealth + " / " + maxHealth);

        if (bossUI != null)
            bossUI.SetHealth(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        if (bossAI != null)
            bossAI.PlayHurt();
    }

  private void Die()
{
    if (bossUI != null)
        bossUI.Hide();

    if (bossAI != null)
        bossAI.Die();
}
}