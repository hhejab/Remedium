using UnityEngine;

public class PlayerSkillPointWallet : MonoBehaviour
{
    public int skillPoints = 0;
    public int currentSkillXP = 0;
    public int xpNeededForOneSkillPoint = 5;

    public void AddSkillXP(int amount)
    {
        currentSkillXP += amount;

        while (currentSkillXP >= xpNeededForOneSkillPoint)
        {
            currentSkillXP -= xpNeededForOneSkillPoint;
            skillPoints++;
        }

        Debug.Log("Skill XP: " + currentSkillXP + "/" + xpNeededForOneSkillPoint + " | Skill Points: " + skillPoints);
        RefreshSkillBook();
    }

    public void AddSkillPoints(int amount)
    {
        skillPoints += amount;

        Debug.Log("Gained Skill Points: " + amount + " | Total: " + skillPoints);
        RefreshSkillBook();
    }

    public bool SpendSkillPoints(int amount)
    {
        if (skillPoints < amount) return false;

        skillPoints -= amount;
        RefreshSkillBook();
        return true;
    }

    private void RefreshSkillBook()
    {
        SkillBookManager book = FindFirstObjectByType<SkillBookManager>();
        if (book != null)
            book.RefreshAll();
    }
}