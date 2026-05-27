using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawnManager : MonoBehaviour
{
    public static bool isRespawning;

    public void RespawnFromDeath()
    {
        isRespawning = true;

        HidePlayer();

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

    private void HidePlayer()
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer sr in renderers)
            sr.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }

    public static void FinishRespawn(GameObject player)
    {
        if (player == null) return;

        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
            health.FullHeal();

        SpriteRenderer[] renderers = player.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer sr in renderers)
            sr.enabled = true;

        isRespawning = false;
    }
}