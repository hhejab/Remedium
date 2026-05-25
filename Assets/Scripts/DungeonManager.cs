using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance { get; private set; }

    [Header("Spawning")]
    [Tooltip("Maximum number of active enemies allowed in the dungeon (0 = no limit)")]
    public int maxEnemies = 50;

    // internal tracking
    int currentEnemies = 0;

    [Tooltip("Parent object under which spawned enemies will be placed")]
    public Transform enemiesParent;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        Instance = this;

        if (enemiesParent == null)
        {
            var go = new GameObject("_DungeonEnemies");
            enemiesParent = go.transform;
        }
    }

    void Start()
    {
        // Intentionally not auto-finding RoomControllers when using global spawners.
        // RoomController and ActivateRoom remain for compatibility but are optional.
    }

    public void ActivateRoom(RoomController room)
    {
        if (room == null) return;
        // Activate the room (spawn enemies, enable traps, etc.)
        room.Activate(enemiesParent);
        // Note: room-based activation is optional. If you don't use RoomController,
        // ignore this method.
    }

    // Spawn helper (centralized) so you can hook pooling later
    public GameObject SpawnEnemy(GameObject prefab, Vector2 position)
    {
        if (prefab == null) return null;
        if (maxEnemies > 0 && currentEnemies >= maxEnemies)
        {
            // reached global cap
            return null;
        }

        var go = Instantiate(prefab, position, Quaternion.identity, enemiesParent);

        // Attach registration helper so we know when it's destroyed
        var reg = go.GetComponent<EnemyRegistration>();
        if (reg == null) reg = go.AddComponent<EnemyRegistration>();

        currentEnemies++;
        return go;
    }

    // Called by EnemyRegistration when an enemy is destroyed to keep counts correct
    public void NotifyEnemyDestroyed()
    {
        currentEnemies = Mathf.Max(0, currentEnemies - 1);
    }

    // Try to reserve a global enemy slot. Returns true if reserved (you should ensure an EnemyRegistration is added to the spawned object).
    public bool TryReserveSlot()
    {
        if (maxEnemies <= 0) return true;
        if (currentEnemies < maxEnemies)
        {
            currentEnemies++;
            return true;
        }
        return false;
    }
}
