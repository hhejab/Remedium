using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject bossHealthPanel;
    public Image healthBar;
    public TextMeshProUGUI bossNameText;

    public void SetBossName(string bossName)
    {
        if (bossNameText != null)
            bossNameText.text = bossName;
    }

    public void SetHealth(int currentHealth, int maxHealth)
    {
        if (healthBar == null)
        {
            Debug.LogWarning("BossUI: Health bar is not assigned.");
            return;
        }

        float fill = (float)currentHealth / maxHealth;
        healthBar.fillAmount = fill;

        Debug.Log("Boss UI updated: " + currentHealth + " / " + maxHealth + " Fill: " + fill);
    }

    public void Show()
    {
        if (bossHealthPanel != null)
            bossHealthPanel.SetActive(true);
    }

    public void Hide()
    {
        if (bossHealthPanel != null)
            bossHealthPanel.SetActive(false);
    }
}