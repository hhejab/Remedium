using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class ExpManager : MonoBehaviour
{
    public int level;
    public int currentExp;
    public int expToLevel = 10;
    public float expGrowthMultiplier = 1.2f;
    public Slider expSlider;
    public TMP_Text currentLevelText;

    public void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            GainExperience(2);
        }
    }

    public void GainExperience(int amount)
    {
        currentExp += amount;
        if (currentExp >= expToLevel)
        {
            LevelUp();
        }
        UpdateUI();
    }

    private void LevelUp()
    {
        level++;
        currentExp -= expToLevel;
        expToLevel = Mathf.RoundToInt(expToLevel * expGrowthMultiplier);
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (expSlider != null)
        {
            expSlider.maxValue = expToLevel;
            expSlider.value = currentExp;
        }

        if (currentLevelText != null)
        {
            currentLevelText.text = "Level: " + level;
        }
    }
}
