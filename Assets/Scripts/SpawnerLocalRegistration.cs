using UnityEngine;

public class SpawnerLocalRegistration : MonoBehaviour
{
    public EnemySpawner spawner;
    public int spawnIndex = -1;

    void OnDestroy()
    {
        if (spawner != null) spawner.NotifyLocalDestroyed(gameObject, spawnIndex);
    }
}
