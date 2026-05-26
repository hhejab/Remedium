using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth;
    public int MaxHealth = 100;
    public Image healthBarFill;

    private PlayerStats playerStats;
    private bool isDead;
    private Animator animator;

    [Header("Hurt / Death Settings")]
    [SerializeField] private float hurtCooldown = 2f; // seconds of invincibility after taking damage
    [SerializeField] private float deathDespawnDelay = 0.6f; // seconds to wait for death animation before despawn/respawn
    private float lastHurtTime = -999f;

    void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        animator = GetComponent<Animator>();
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

        int appliedAmount = amount;

        // Damage handling with invincibility window and mitigation
        if (amount < 0)
        {
            if (Time.time < lastHurtTime + hurtCooldown) return; // still invulnerable

            if (playerStats != null)
            {
                int incomingDamage = Mathf.Abs(amount);
                int reducedDamage = playerStats.ReduceDamage(incomingDamage);
                appliedAmount = -reducedDamage;
            }
        }

        currentHealth += appliedAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, finalMaxHealth);

        UpdateUI();

        // If died, start death sequence (plays animation then despawn/respawn)
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // If we actually took damage (and didn't die), trigger hurt animation and start invincibility
        if (appliedAmount < 0)
        {
            lastHurtTime = Time.time;
            if (animator != null)
            {
                animator.ResetTrigger("Hurt");
                animator.SetTrigger("Hurt");
            }
        }
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

        if (animator != null)
        {
            animator.ResetTrigger("Death");
            animator.SetTrigger("Death");
        }

        StartCoroutine(DeathDelayCoroutine());
    }

    private IEnumerator DeathDelayCoroutine()
    {
        yield return new WaitForSeconds(deathDespawnDelay);

        PlayerRespawnManager respawn = GetComponent<PlayerRespawnManager>();

        if (respawn != null)
        {
            respawn.RespawnFromDeath();
            yield break;
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