using UnityEngine;

public class TileLayer_ColliderDepthSwitch : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Collider Source")]
    public Transform colliderRoot; // Drag the Colliders parent here
    public float checkRadius = 1.5f;
    public float yOffset = 0f;

    [Header("Sorting Layers")]
    public string normalLayer = "Base";
    public string behindLayer = "AbovePlayer";

    [Header("Sorting Orders")]
    public int normalOrder = 4;
    public int behindOrder = 10;

    private Renderer rend;
    private Collider2D[] colliders;

    private void Awake()
    {
        rend = GetComponent<Renderer>();

        if (colliderRoot != null)
            colliders = colliderRoot.GetComponentsInChildren<Collider2D>();

        if (rend == null)
            Debug.LogWarning(gameObject.name + " has no Renderer.");

        if (colliderRoot == null)
            Debug.LogWarning(gameObject.name + " needs Collider Root assigned.");

        if (colliders == null || colliders.Length == 0)
            Debug.LogWarning(gameObject.name + " found no Collider2D children under Collider Root.");
    }

    private void LateUpdate()
    {
        if (player == null || rend == null || colliders == null || colliders.Length == 0)
            return;

        bool shouldBeAbovePlayer = false;

        foreach (Collider2D col in colliders)
        {
            if (col == null) continue;

            Bounds b = col.bounds;

            // closest point on this collider to the player
            Vector2 closestPoint = col.ClosestPoint(player.position);
            float distance = Vector2.Distance(player.position, closestPoint);

            if (distance <= checkRadius)
            {
                // bottom/base of collider
                float switchY = b.min.y + yOffset;

                // player is behind / above the object
                if (player.position.y > switchY)
                {
                    shouldBeAbovePlayer = true;
                    break;
                }
            }
        }

        if (shouldBeAbovePlayer)
        {
            rend.sortingLayerName = behindLayer;
            rend.sortingOrder = behindOrder;
        }
        else
        {
            rend.sortingLayerName = normalLayer;
            rend.sortingOrder = normalOrder;
        }
    }
}