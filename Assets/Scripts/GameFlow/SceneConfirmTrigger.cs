using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneConfirmTrigger : MonoBehaviour
{
    public string sceneToLoad;
    public string spawnPointName;
    public string message = "Enter?";

    [Header("Boss Lock")]
    public bool lockAfterFirstBossDefeated;
    public string alreadyDefeatedMessage = "You already defeated this boss.";

    [Header("Require First Boss Defeated")]
    public bool requireFirstBossDefeated;
    public string firstBossRequiredMessage = "Defeat the first boss first.";

    [Header("Level Lock")]
    public bool requirePlayerLevel;
    public int requiredPlayerLevel = 2;
    public string levelLockedMessage = "You need to upgrade yourself first.";

    private bool triggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        if (lockAfterFirstBossDefeated && BossProgress.Instance != null && BossProgress.Instance.firstBossDefeated)
        {
            TriggerUIManager.Instance.Show(alreadyDefeatedMessage, null);
            return;
        }

        if (requireFirstBossDefeated && (BossProgress.Instance == null || !BossProgress.Instance.firstBossDefeated))
        {
            TriggerUIManager.Instance.Show(firstBossRequiredMessage, null);
            return;
        }

        if (requirePlayerLevel)
        {
            PlayerLevel level = other.GetComponent<PlayerLevel>();

            if (level != null && level.currentLevel < requiredPlayerLevel)
            {
                TriggerUIManager.Instance.Show(levelLockedMessage, null);
                return;
            }
        }

        TriggerUIManager.Instance.Show(message, () =>
        {
            triggered = true;
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