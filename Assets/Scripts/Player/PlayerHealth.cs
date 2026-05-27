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
    private Coroutine deathCoroutine;

    [Header("Hurt / Death Settings")]
    [SerializeField] private float hurtCooldown = 2f;
    [SerializeField] private float deathDespawnDelay = 1.0f;

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

        if (amount < 0)
        {
            if (Time.time < lastHurtTime + hurtCooldown) return;

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

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

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

        if (deathCoroutine != null)
        {
            StopCoroutine(deathCoroutine);
            deathCoroutine = null;
        }

        if (animator != null)
        {
            animator.SetBool("isDead", false);
            animator.SetBool("isMoving", false);
            animator.SetBool("isAttackin", false);
            animator.SetBool("isWalkAttc", false);
            animator.SetBool("isRunning", false);
            animator.ResetTrigger("Hurt");
            animator.Play("idle", 0, 0f);
        }

        Movement movement = GetComponent<Movement>();
        if (movement != null)
            movement.enabled = true;

        PlayerCombat combat = GetComponent<PlayerCombat>();
        if (combat != null)
            combat.enabled = true;

        currentHealth = GetFinalMaxHealth();
        UpdateUI();
    }

    public void AddMaxHealthAndHeal(int amount)
    {
        isDead = false;

        if (animator != null)
            animator.SetBool("isDead", false);

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, GetFinalMaxHealth());
        UpdateUI();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        Movement movement = GetComponent<Movement>();
        if (movement != null)
            movement.enabled = false;

        PlayerCombat combat = GetComponent<PlayerCombat>();
        if (combat != null)
            combat.enabled = false;

        if (animator != null)
        {
            animator.ResetTrigger("Hurt");
            animator.SetBool("isMoving", false);
            animator.SetBool("isAttackin", false);
            animator.SetBool("isWalkAttc", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isDead", true);
        }

        deathCoroutine = StartCoroutine(DeathDelayCoroutine());
    }

    private IEnumerator DeathDelayCoroutine()
    {
        yield return new WaitForSecondsRealtime(deathDespawnDelay);

        PlayerRespawnManager respawn = GetComponent<PlayerRespawnManager>();

        if (respawn != null)
            respawn.RespawnFromDeath();
        else
            gameObject.SetActive(false);
    }

    void UpdateUI()
    {
        int finalMaxHealth = GetFinalMaxHealth();

        if (healthBarFill != null)
            healthBarFill.fillAmount = (float)currentHealth / finalMaxHealth;
    }
}