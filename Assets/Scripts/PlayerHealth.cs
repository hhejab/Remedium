using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth;
    public int MaxHealth;
    public Image healthBarFill;

    void Start()
    {
        currentHealth = MaxHealth;
        UpdateUI();
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);
        UpdateUI();

        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    void UpdateUI()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / MaxHealth;
        }
    }
}


