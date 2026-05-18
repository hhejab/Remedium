using UnityEngine;

public class Sort_Y_Collider : MonoBehaviour
{
     [Header("Renderer To Sort")]
    public Renderer targetRenderer;

    [Header("Collider Parent")]
    public Transform colliderRoot;

    [Header("Sorting")]
    public int sortingOffset = 0;
    public bool useLowestColliderPoint = true;

    private Collider2D[] childColliders;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        if (colliderRoot != null)
            childColliders = colliderRoot.GetComponentsInChildren<Collider2D>();

        if (targetRenderer == null)
            Debug.LogWarning(name + " has no Renderer assigned.");

        if (colliderRoot == null)
            Debug.LogWarning(name + " has no Collider Root assigned.");

        if (childColliders == null || childColliders.Length == 0)
            Debug.LogWarning(name + " found no child Collider2D under Collider Root.");
    }

    private void LateUpdate()
    {
        if (targetRenderer == null || childColliders == null || childColliders.Length == 0)
            return;

        float yPoint = useLowestColliderPoint ? GetLowestColliderY() : GetAverageColliderY();

        targetRenderer.sortingOrder = Mathf.RoundToInt(-yPoint * 100) + sortingOffset;
    }

    private float GetLowestColliderY()
    {
        float lowestY = float.MaxValue;

        foreach (Collider2D col in childColliders)
        {
            if (col == null) continue;

            if (col.bounds.min.y < lowestY)
                lowestY = col.bounds.min.y;
        }

        return lowestY;
    }

    private float GetAverageColliderY()
    {
        float totalY = 0f;
        int count = 0;

        foreach (Collider2D col in childColliders)
        {
            if (col == null) continue;

            totalY += col.bounds.center.y;
            count++;
        }

        if (count == 0)
            return transform.position.y;

        return totalY / count;
    }
}