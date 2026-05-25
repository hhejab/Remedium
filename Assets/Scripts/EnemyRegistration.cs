using UnityEngine;

public class EnemyRegistration : MonoBehaviour
{
    void OnDestroy()
    {
        if (DungeonManager.Instance != null)
        {
            DungeonManager.Instance.NotifyEnemyDestroyed();
        }
    }
}
