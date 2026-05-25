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
        if (string.IsNullOrEmpty(nextSpawnPointName))
            return;

        GameObject spawn = GameObject.Find(nextSpawnPointName);

        if (spawn == null)
        {
            Debug.LogWarning("Spawn point not found: " + nextSpawnPointName);
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            player.transform.position = spawn.transform.position;
    }
}