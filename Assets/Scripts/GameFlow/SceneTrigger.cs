// SceneTrigger.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    public string sceneToLoad;

    [Header("Requirement")]
    public bool requireLevel;
    public int requiredLevel = 1;

    public bool requireProgress;
    public GameProgress neededProgress;

    private bool playerInside;

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
            TryLoad();
    }

    private void TryLoad()
    {
        if (requireLevel && GameManager.Instance.playerLevel < requiredLevel)
        {
            Debug.Log("Need level " + requiredLevel);
            return;
        }

        if (requireProgress && GameManager.Instance.progress < neededProgress)
        {
            Debug.Log("Not unlocked yet");
            return;
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log("Press E");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }
}