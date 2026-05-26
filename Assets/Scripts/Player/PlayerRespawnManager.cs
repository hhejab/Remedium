using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawnManager : MonoBehaviour
{
    public void RespawnFromDeath()
    {
        PlayerHealth health = GetComponent<PlayerHealth>();
        if (health != null)
            health.FullHeal();

        Time.timeScale = 1f;

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Dungeon")
        {
            SceneSpawnManager.nextSpawnPointName = "DungeonGateSpawn";
            SceneManager.LoadScene("Forest");
            return;
        }

        if (currentScene == "BossArena")
        {
            SceneSpawnManager.nextSpawnPointName = "NearBossRoomSpawn";
            SceneManager.LoadScene("Dungeon");
            return;
        }

        SceneSpawnManager.nextSpawnPointName = "DefaultSpawn";
        SceneManager.LoadScene(currentScene);
    }
}