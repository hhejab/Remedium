using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Tooltip("Prefabs to spawn (random selection)")]
    public List<GameObject> enemyPrefabs = new List<GameObject>();

    [Tooltip("Transforms used as spawn points; if empty, spawns near this object's position")]
    public List<Transform> spawnPoints = new List<Transform>();

    [Tooltip("If true, spawn once at Start")] 
    public bool spawnAtStart = true;

    [Tooltip("Number of enemies to spawn per spawn point")] 
    public int perPoint = 1;

    [Tooltip("Random offset radius around spawn point")] 
    public float spawnOffsetRadius = 0.3f;

    [Tooltip("Parent for spawned enemies; if empty uses DungeonManager's parent or creates a new one")]
    public Transform enemiesParent;

    [Tooltip("If true, spawned enemies will be parented under the chosen Enemies Parent. If false they will be created at root.")]
    public bool parentToEnemiesParent = true;

    [Header("Limits")]
    [Tooltip("Maximum number of enemies this spawner will keep active (0 = no per-spawner limit)")]
    public int spawnerLimit = 0;

    List<GameObject> spawnedLocal = new List<GameObject>();
    float[] nextAvailableTime;

    [Header("Respawn")]
    [Tooltip("Delay (seconds) after a spawn point's enemies are defeated before that point will respawn")]
    public float respawnDelay = 5f;
    [Tooltip("If true, spawner will automatically refill defeated spawn points up to the spawner limit")]
    public bool autoRespawn = true;

    void Start()
    {
        InitializeSpawnTimers();
        if (spawnAtStart) SpawnAll();
    }

    void InitializeSpawnTimers()
    {
        int points = (spawnPoints == null || spawnPoints.Count == 0) ? 1 : spawnPoints.Count;
        nextAvailableTime = new float[points];
        for (int i = 0; i < points; i++) nextAvailableTime[i] = Time.time + respawnDelay;
    }

    void Update()
    {
        if (!autoRespawn) return;
        if (spawnerLimit > 0 && spawnedLocal.Count >= spawnerLimit) return;

        int points = (spawnPoints == null || spawnPoints.Count == 0) ? 1 : spawnPoints.Count;
        for (int i = 0; i < points; i++)
        {
            if (Time.time < nextAvailableTime[i]) continue;
            if (spawnerLimit > 0 && spawnedLocal.Count >= spawnerLimit) break;

            int canSpawn = perPoint;
            if (spawnerLimit > 0) canSpawn = Mathf.Min(canSpawn, spawnerLimit - spawnedLocal.Count);
            if (canSpawn <= 0) continue;

            Vector2 basePos = (spawnPoints == null || spawnPoints.Count == 0) ? (Vector2)transform.position : (Vector2)spawnPoints[i].position;
            for (int s = 0; s < canSpawn; s++)
            {
                Vector2 pos = basePos + Random.insideUnitCircle * spawnOffsetRadius;
                TrySpawnAt(pos, i);
            }

            nextAvailableTime[i] = Time.time + respawnDelay;
        }
    }

    [ContextMenu("Spawn All")]
    public void SpawnAll()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("EnemySpawner: no enemyPrefabs assigned.", this);
            return;
        }

        if (enemiesParent == null && DungeonManager.Instance != null)
        {
            enemiesParent = DungeonManager.Instance.enemiesParent;
        }

        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            for (int i = 0; i < perPoint; i++)
            {
                TrySpawnAt(transform.position, 0);
            }
            return;
        }

        for (int idx = 0; idx < spawnPoints.Count; idx++)
        {
            var sp = spawnPoints[idx];
            if (sp == null) continue;
            for (int i = 0; i < perPoint; i++)
            {
                Vector2 pos = (Vector2)sp.position + Random.insideUnitCircle * spawnOffsetRadius;
                TrySpawnAt(pos, idx);
            }
        }
    }

    void TrySpawnAt(Vector2 pos, int spawnIndex = -1)
    {
        if (spawnerLimit > 0 && spawnedLocal.Count >= spawnerLimit) return;

        var prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        if (prefab == null) return;

        GameObject go = null;
        if (DungeonManager.Instance != null && parentToEnemiesParent)
        {
            go = DungeonManager.Instance.SpawnEnemy(prefab, pos);
            if (go == null) return; // global cap prevented spawn
        }
        else
        {
            // if a global manager exists, try to reserve a slot first
            if (DungeonManager.Instance != null)
            {
                if (!DungeonManager.Instance.TryReserveSlot()) return;
            }

            Transform parent = (parentToEnemiesParent) ? enemiesParent : null;
            if (parent == null && parentToEnemiesParent && DungeonManager.Instance != null)
            {
                parent = DungeonManager.Instance.enemiesParent;
            }
            if (parent == null && parentToEnemiesParent)
            {
                var goParent = new GameObject("_Enemies");
                parent = goParent.transform;
                enemiesParent = parent;
            }

            go = Instantiate(prefab, pos, Quaternion.identity, parent);
        }

        if (go != null)
        {
            spawnedLocal.Add(go);
            var reg = go.AddComponent<SpawnerLocalRegistration>();
            reg.spawner = this;
            reg.spawnIndex = spawnIndex;
            if (DungeonManager.Instance != null)
            {
                var ereg = go.GetComponent<EnemyRegistration>();
                if (ereg == null) go.AddComponent<EnemyRegistration>();
            }
        }
    }

    internal void NotifyLocalDestroyed(GameObject go, int spawnIndex)
    {
        spawnedLocal.Remove(go);
        int points = (spawnPoints == null || spawnPoints.Count == 0) ? 1 : spawnPoints.Count;
        if (spawnIndex >= 0 && spawnIndex < points)
        {
            nextAvailableTime[spawnIndex] = Time.time + respawnDelay;
        }
    }
}
