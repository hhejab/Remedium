using UnityEngine;
using UnityEngine.UI;

public class StaminaSystem : MonoBehaviour
{
    public float maxStamina = 50f; 
    public float currentStamina;
    public float staminaDrainPerSecond = 20f;
    public float staminaRegenPerSecond = 10f;
    
    public Image staminaBarFill;
    private RectTransform rectTransform;

    void Awake()
    {
        currentStamina = maxStamina;
        if (staminaBarFill != null)
        {
            rectTransform = staminaBarFill.GetComponent<RectTransform>();
        }
    }

    void Start()
    {
        UpdateUI();
    }

    public void UpgradeStamina(float percent)
    {
        maxStamina += maxStamina * (percent / 100f);
        currentStamina = maxStamina;
        UpdateUI();
    }

    public bool CanUseStamina(float cost)
    {
        return currentStamina >= cost;
    }

    public void DrainStaminaOverTime()
    {
        currentStamina -= staminaDrainPerSecond * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        UpdateUI();
    }

    public void RegenerateStamina()
    {
        currentStamina += staminaRegenPerSecond * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (staminaBarFill != null && rectTransform != null)
        {
            staminaBarFill.fillAmount = currentStamina / maxStamina;
            rectTransform.sizeDelta = new Vector2(maxStamina * 2f, rectTransform.sizeDelta.y);
        }
    }
}
