using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneConfirmTrigger : MonoBehaviour
{
    [Header("Scene")]
    public string sceneToLoad;
    public string spawnPointName = "DefaultSpawn";

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