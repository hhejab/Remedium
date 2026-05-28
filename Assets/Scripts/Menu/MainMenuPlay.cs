using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuPlay : MonoBehaviour
{
    public string sceneToLoad = "Village";
    public string spawnPointName = "VillageSpawn";

    public void PlayGame()
    {
        Time.timeScale = 1f;
        SceneSpawnManager.nextSpawnPointName = spawnPointName;
        SceneManager.LoadScene(sceneToLoad);
    }
}