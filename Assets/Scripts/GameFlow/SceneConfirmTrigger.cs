using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneConfirmTrigger : MonoBehaviour
{
    [Header("Scene")]
    public string sceneToLoad;
    public string spawnPointName = "DefaultSpawn";

    [Header("UI")]
    [TextArea]
    public string message = "Do you want to enter?";

    [Header("Level Requirement")]
    public bool requirePlayerLevel = false;
    public int requiredPlayerLevel = 2;
    public string levelRequiredMessage = "You need to upgrade yourself at the blacksmith first.";

    private bool triggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (triggered) return;

        triggered = true;

        if (TriggerUIManager.Instance == null)
        {
            Debug.LogError("TriggerUIManager not found.");
            triggered = false;
            return;
        }

        if (requirePlayerLevel)
        {
            PlayerLevel playerLevel = other.GetComponent<PlayerLevel>();

            if (playerLevel == null || playerLevel.currentLevel < requiredPlayerLevel)
            {
                TriggerUIManager.Instance.Show(levelRequiredMessage, null);
                return;
            }
        }

        TriggerUIManager.Instance.Show(message, () =>
        {
            ResetPlayerBeforeSceneLoad(other.gameObject);

            SceneSpawnManager.nextSpawnPointName = spawnPointName;
            SceneManager.LoadScene(sceneToLoad);
        });
    }

    private void ResetPlayerBeforeSceneLoad(GameObject player)
    {
        Time.timeScale = 1f;

        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        if (playerInput != null)
            playerInput.ActivateInput();

        PlayerCombat combat = player.GetComponent<PlayerCombat>();
        if (combat != null)
            combat.enabled = true;

        Movement movement = player.GetComponent<Movement>();
        if (movement != null)
            movement.enabled = true;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        triggered = false;
    }
}