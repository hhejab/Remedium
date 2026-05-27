using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSpawnManager : MonoBehaviour
{
    public static string nextSpawnPointName = "";

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (string.IsNullOrEmpty(nextSpawnPointName))
            return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("No Player found.");
            return;
        }

        Debug.Log("Trying to spawn at: " + nextSpawnPointName);

        GameObject spawnPoint = GameObject.Find(nextSpawnPointName);

        if (spawnPoint == null)
        {
            Debug.LogWarning("Spawn point not found: " + nextSpawnPointName);
            return;
        }

        player.transform.position = spawnPoint.transform.position;

        Debug.Log("Spawned player at: " + spawnPoint.name);

        PlayerRespawnManager.FinishRespawn(player);

        nextSpawnPointName = "";
    }
}