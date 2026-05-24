using UnityEngine;

public class BossDoorTrigger : MonoBehaviour
{
    public LockedBossDoor door;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (door == null)
        {
            Debug.LogError("BossDoorTrigger: Door reference is missing.");
            return;
        }

        door.TryOpen();
    }
}