using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int slotIndex;
    public PlayerInventory inventory;
    private Vector3 startPosition;
    private Image img;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        img = GetComponent<Image>();
        inventory = FindFirstObjectByType<PlayerInventory>();
        
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (img.sprite == null || img.color.a == 0) return;
        
        startPosition = transform.position;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (img.sprite == null || img.color.a == 0) return;
        
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (img.sprite == null || img.color.a == 0) return;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            inventory.RemoveFromHotbar(slotIndex);
        }
        
        transform.position = startPosition;
    }
}

