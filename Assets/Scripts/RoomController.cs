using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RoomController : MonoBehaviour
{
    [Tooltip("Spawn prefabs for this room's enemies")]
    public List<GameObject> enemyPrefabs = new List<GameObject>();

    [Tooltip("Transforms used as spawn points; if empty, room origin will be used")]
    public List<Transform> spawnPoints = new List<Transform>();

    [Tooltip("Should enemies spawn when player enters the room?")]
    public bool spawnOnEnter = true;

    [Tooltip("Deactivate room when player leaves (stops enemies)")]
    public bool deactivateOnLeave = false;

    List<GameObject> spawned = new List<GameObject>();
    bool active = false;

    void Reset()
    {
        // ensure collider is trigger
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    public void Activate(Transform parentForEnemies = null)
    {
        if (active) return;
        active = true;

        if (enemyPrefabs.Count == 0) return;

        if (spawnPoints.Count == 0)
        {
            // fallback to room center
            var go = SpawnRandomEnemy(transform.position, parentForEnemies);
            if (go) spawned.Add(go);
            return;
        }

        foreach (var sp in spawnPoints)
        {
            if (sp == null) continue;
            var go = SpawnRandomEnemy(sp.position, parentForEnemies);
            if (go) spawned.Add(go);
        }
    }

    public void Deactivate()
    {
        if (!active) return;
        active = false;
        // simple approach: destroy spawned enemies
        foreach (var e in spawned) if (e != null) Destroy(e);
        spawned.Clear();
    }

    GameObject SpawnRandomEnemy(Vector2 pos, Transform parentForEnemies)
    {
        if (enemyPrefabs.Count == 0) return null;
        var prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        if (prefab == null) return null;
        if (DungeonManager.Instance != null)
        {
            var go = DungeonManager.Instance.SpawnEnemy(prefab, pos);
            return go;
        }
        else
        {
            return Instantiate(prefab, pos, Quaternion.identity, parentForEnemies);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!spawnOnEnter) return;
        if (!other.CompareTag("Player")) return;
        DungeonManager.Instance?.ActivateRoom(this);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!deactivateOnLeave) return;
        if (!other.CompareTag("Player")) return;
        Deactivate();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider2D>().bounds.size);
        Gizmos.color = Color.red;
        foreach (var sp in spawnPoints) if (sp != null) Gizmos.DrawSphere(sp.position, 0.08f);
    }
}
