using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Skill")]
    public SkillData skillData;

    [Header("UI")]
    public Button button;
    public Image iconImage;
    public TMP_Text costText;

    [Header("Tint Colors")]
    public Color unlockedColor = Color.white;
    public Color availableColor = Color.white;
    public Color lockedColor = new Color(0.25f, 0.25f, 0.25f, 1f);

    private SkillBookManager manager;

    public void Setup(SkillBookManager skillBookManager)
    {
        manager = skillBookManager;

        if (button == null)
            button = GetComponent<Button>();

        if (iconImage == null)
            iconImage = GetComponent<Image>();

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(ClickSkill);
        }

        Refresh();
    }

    public void Refresh()
    {
        if (skillData == null || manager == null) return;

        bool unlocked = manager.IsUnlocked(skillData);
        bool canUnlock = manager.CanUnlock(skillData);

        if (iconImage != null)
        {
            if (skillData.icon != null)
                iconImage.sprite = skillData.icon;

            if (unlocked)
                iconImage.color = unlockedColor;
            else if (canUnlock)
                iconImage.color = availableColor;
            else
                iconImage.color = lockedColor;
        }

        if (costText != null)
        {
            if (unlocked)
                costText.text = "";
            else
                costText.text = skillData.cost.ToString();
        }

        if (button != null)
            button.interactable = !unlocked && canUnlock;
    }

    private void ClickSkill()
    {
        if (manager != null)
            manager.UnlockSkill(skillData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (manager != null && skillData != null)
            manager.ShowSkillDescription(skillData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (manager != null)
            manager.ClearSkillDescription();
    }
}