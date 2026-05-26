using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    public int currentLevel = 1;

    public void SetLevel(int level)
    {
        currentLevel = level;
        Debug.Log("Player level is now: " + currentLevel);
    }
}