using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneConfirmTrigger : MonoBehaviour
{
    [Header("Scene")]
    public string sceneToLoad;
    public string spawnPointName = "DefaultSpawn";

    [Header("Lock Rule")]
    public bool requireFirstBossDefeated = false;
    public string lockedMessage = "You need to defeat the first boss before entering the Blacksmith.";

    [Header("Level Lock")]
    public bool requirePlayerLevel = false;
    public int requiredPlayerLevel = 2;
    public string levelLockedMessage = "You need to upgrade yourself first.";

    [Header("UI")]
    [TextArea]
    public string message = "Do you want to enter?";

    private bool triggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (triggered) return;

        triggered = true;

        if (TriggerUIManager.Instance == null)
        {
            Debug.LogError("TriggerUIManager not found.");
            return;
        }

        if (requireFirstBossDefeated)
        {
            if (BossProgress.Instance == null || !BossProgress.Instance.firstBossDefeated)
            {
                TriggerUIManager.Instance.Show(lockedMessage, null);
                return;
            }
        }

       if (requirePlayerLevel)
        {
            PlayerLevel playerLevel = other.GetComponent<PlayerLevel>();

            if (playerLevel == null || playerLevel.currentLevel < requiredPlayerLevel)
            {
                TriggerUIManager.Instance.Show(levelLockedMessage, null);
                return;
            }
        }
        TriggerUIManager.Instance.Show(message, () =>
        {
            SceneSpawnManager.nextSpawnPointName = spawnPointName;
            SceneManager.LoadScene(sceneToLoad);
        });
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        triggered = false;
    }
}