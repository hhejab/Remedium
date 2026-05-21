using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneConfirmTrigger : MonoBehaviour
{
    public string sceneToLoad;
    public string spawnPointName;
    public string message = "Enter?";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        TriggerUIManager.Instance.Show(message, () =>
        {
            SceneSpawnManager.nextSpawnPointName = spawnPointName;
            SceneManager.LoadScene(sceneToLoad);
        });
    }
}