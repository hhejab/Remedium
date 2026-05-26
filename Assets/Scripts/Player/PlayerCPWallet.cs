using UnityEngine;

public class PlayerCPWallet : MonoBehaviour
{
    public int combatPoints = 0;

    public void AddCP(int amount)
    {
        combatPoints += amount;
        Debug.Log("CP +" + amount + " | Total CP: " + combatPoints);
    }

    public bool SpendCP(int amount)
    {
        if (combatPoints < amount)
        {
            Debug.Log("Not enough CP.");
            return false;
        }

        combatPoints -= amount;
        Debug.Log("Spent CP: " + amount + " | Remaining CP: " + combatPoints);
        return true;
    }
}