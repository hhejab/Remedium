using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemBoss_Health : BossHealth
{
    [Header("Golem Minion Phase")]
    [Range(0.1f, 1f)]
    public float minionPhaseHealthPercent = 0.7f;

    public GameObject[] goblinPrefabs;

    [Header("Spawn Points Optional")]
    public Transform[] goblinSpawnPoints;

    [Header("Arena Spawn Rules")]
    public Collider2D arenaBounds;
    public LayerMask obstacleLayer;
    public float spawnCheckRadius = 0.45f;
    public float minDistanceFromGolem = 2f;
    public float minDistanceFromPlayer = 1.5f;
    public int randomSpawnAttempts = 40;

    [Header("Timing")]
    public float phaseStartPause = 0.6f;
    public float afterMinionsPause = 1f;

    private bool minionPhaseStarted;
    private bool minionPhaseActive;

    private readonly List<GameObject> spawnedGoblins = new List<GameObject>();

    protected override bool CanTakeDamage()
    {
        if (minionPhaseActive)
        {
            Debug.Log("Golem is protected while goblins are alive.");
            return false;
        }

        return base.CanTakeDamage();
    }

    protected override void OnAfterDamage(int damage)
    {
        if (minionPhaseStarted)
            return;

        if (GetHealthPercent() <= minionPhaseHealthPercent && currentHealth > 0)
        {
            StartCoroutine(StartMinionPhase());
        }
    }

    private IEnumerator StartMinionPhase()
    {
        minionPhaseStarted = true;
        minionPhaseActive = true;

        Debug.Log("Golem reached 70%. Spawning goblins inside arena.");

        PauseGolem();

        yield return new WaitForSeconds(phaseStartPause);

        SpawnGoblins();

        yield return StartCoroutine(WaitUntilGoblinsAreDead());

        Debug.Log("All goblins defeated. Golem continues.");

        ResumeGolem();

        yield return new WaitForSeconds(afterMinionsPause);

        minionPhaseActive = false;
    }

    private void PauseGolem()
    {
        if (bossAI != null)
            bossAI.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("isMoving", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", false);
            animator.SetBool("isHurt", true);
        }
    }

    private void ResumeGolem()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("isHurt", false);
            animator.SetBool("isMoving", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", false);
        }

        if (bossAI != null)
            bossAI.enabled = true;
    }

    private void SpawnGoblins()
    {
        spawnedGoblins.Clear();

        if (goblinPrefabs == null || goblinPrefabs.Length == 0)
        {
            Debug.LogWarning("GolemBoss_Health: No goblin prefabs assigned.");
            return;
        }

        int amountToSpawn = 3;

        for (int i = 0; i < amountToSpawn; i++)
        {
            GameObject prefab = goblinPrefabs[i % goblinPrefabs.Length];

            if (prefab == null)
                continue;

            Vector3 spawnPosition;

            if (!TryGetSpawnPosition(i, out spawnPosition))
            {
                Debug.LogWarning("Could not find valid goblin spawn position. Using golem fallback.");
                spawnPosition = transform.position + new Vector3(i - 1, -2f, 0f);
            }

            GameObject goblin = Instantiate(prefab, spawnPosition, Quaternion.identity);
            spawnedGoblins.Add(goblin);
        }
    }

    private bool TryGetSpawnPosition(int index, out Vector3 spawnPosition)
    {
        // First try manual spawn points.
        if (goblinSpawnPoints != null &&
            index < goblinSpawnPoints.Length &&
            goblinSpawnPoints[index] != null)
        {
            Vector3 pointPosition = goblinSpawnPoints[index].position;

            if (IsSpawnPositionValid(pointPosition))
            {
                spawnPosition = pointPosition;
                return true;
            }
        }

        // Then try random positions inside arena bounds.
        if (arenaBounds != null)
        {
            Bounds bounds = arenaBounds.bounds;

            for (int attempt = 0; attempt < randomSpawnAttempts; attempt++)
            {
                float x = Random.Range(bounds.min.x, bounds.max.x);
                float y = Random.Range(bounds.min.y, bounds.max.y);

                Vector3 randomPosition = new Vector3(x, y, transform.position.z);

                if (IsSpawnPositionValid(randomPosition))
                {
                    spawnPosition = randomPosition;
                    return true;
                }
            }
        }

        spawnPosition = Vector3.zero;
        return false;
    }

    private bool IsSpawnPositionValid(Vector3 position)
    {
        // Must be inside arena.
        if (arenaBounds != null && !arenaBounds.OverlapPoint(position))
            return false;

        // Must not spawn inside wall/obstacle.
        Collider2D obstacle = Physics2D.OverlapCircle(position, spawnCheckRadius, obstacleLayer);
        if (obstacle != null)
            return false;

        // Not too close to golem.
        if (Vector2.Distance(position, transform.position) < minDistanceFromGolem)
            return false;

        // Not too close to player.
        if (bossAI != null && bossAI.player != null)
        {
            if (Vector2.Distance(position, bossAI.player.position) < minDistanceFromPlayer)
                return false;
        }

        return true;
    }

    private IEnumerator WaitUntilGoblinsAreDead()
    {
        while (true)
        {
            bool anyAlive = false;

            for (int i = spawnedGoblins.Count - 1; i >= 0; i--)
            {
                if (spawnedGoblins[i] == null)
                {
                    spawnedGoblins.RemoveAt(i);
                }
                else
                {
                    anyAlive = true;
                }
            }

            if (!anyAlive)
                break;

            yield return null;
        }
    }

    protected override void Die()
    {
        for (int i = spawnedGoblins.Count - 1; i >= 0; i--)
        {
            if (spawnedGoblins[i] != null)
                Destroy(spawnedGoblins[i]);
        }

        base.Die();
    }
}