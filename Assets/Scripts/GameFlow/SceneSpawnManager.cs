using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSpawnManager : MonoBehaviour
{
    public static string nextSpawnPointName = "DefaultSpawn";

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
        GameObject spawn = GameObject.Find(nextSpawnPointName);

        if (spawn != null && PersistentPlayer.Instance != null)
        {
            PersistentPlayer.Instance.transform.position = spawn.transform.position;
        }
    }
}