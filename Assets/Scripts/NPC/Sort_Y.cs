using UnityEngine;

public class Sort_Y : MonoBehaviour
{
    public Transform sortPoint;
    public int sortingOffset = 0;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        if (sortPoint == null)
            sortPoint = transform;
    }

    private void LateUpdate()
    {
        sr.sortingOrder = Mathf.RoundToInt(-sortPoint.position.y * 100) + sortingOffset;
    }
}