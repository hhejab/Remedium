using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth;
    public int MaxHealth = 100;
    public Image healthBarFill;

    [Header("Animation")]
    public Animator animator;
    [Tooltip("Seconds to wait while death animation plays before disabling player")]
    public float deathAnimationTime = 1f;

    [Header("Invulnerability")]
    [Tooltip("Seconds of invulnerability after taking damage")]
    public float invulnerabilityTime = 2f;
    private bool isInvulnerable = false;

    private bool isDead = false;

    private PlayerStats playerStats;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        currentHealth = GetFinalMaxHealth();
        if (animator == null)
            animator = GetComponent<Animator>();
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

        // If currently invulnerable, ignore incoming damage
        if (amount < 0 && isInvulnerable)
            return;

        if (amount < 0 && playerStats != null)
        {
            int incomingDamage = Mathf.Abs(amount);
            int reducedDamage = playerStats.ReduceDamage(incomingDamage);
            amount = -reducedDamage;
        }

        // Play hurt animation 
        if (amount < 0 && animator != null && !isDead)
        {
            try { animator.SetTrigger("Hurt"); } catch { }
        }

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, finalMaxHealth);

        UpdateUI();

        // Start invulnerability after taking damage 
        if (amount < 0 && !isDead)
        {
            StartCoroutine(InvulnerabilityRoutine());
        }

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            if (animator != null)
            {
                try
                {
                    animator.SetBool("isDead", true);
                    animator.Play("Death");
                    StartCoroutine(DeathAndDisable());
                }
                catch
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
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

    private IEnumerator DeathAndDisable()
    {
        yield return new WaitForSeconds(deathAnimationTime);
        gameObject.SetActive(false);
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityTime);
        isInvulnerable = false;
    }
}


