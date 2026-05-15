using UnityEngine;

public class Static_NPC_Sort_Y : MonoBehaviour
{
    [Header("Sorting")]
    public Transform sortPoint;
    public int sortingOffset = 0;

    private SpriteRenderer[] spriteRenderers;

    private void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        if (sortPoint == null)
            sortPoint = transform;
    }

    private void LateUpdate()
    {
        if (sortPoint == null) return;

        int order = Mathf.RoundToInt(-sortPoint.position.y * 100) + sortingOffset;

        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.sortingLayerName = "Base";
            sr.sortingOrder = order;
        }
    }
}