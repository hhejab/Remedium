using System;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Image itemImage;
    [SerializeField]
    private TMP_Text quantityTxt;

    [SerializeField]
    private UnityEngine.UI.Image borderImage;
    public event Action<InventoryItem> OnItemClicked, OnItemDroppedOn, OnItemBeginDrag, OnItemEndDrag, OnRightMouseBtnClick;

    private bool empty = true;
    public void Awake()
    {
        ResetData();
        Deselect();
    }
    public void ResetData()
    {
        this.itemImage.gameObject.SetActive(false);
        this.empty = true;
    }
    public void Deselect()
    {
        borderImage.enabled = false;
    }
    public void SetData(Sprite sprite, int quantity)
    {
        this.itemImage.gameObject.SetActive(true);
        this.itemImage.sprite = sprite;
        this.quantityTxt.text = quantity + "";
        this.empty = false;
    }
    public void Select()
    {
       borderImage.enabled = true; 
    }
    public void OnBeginDrag()
    {
        if (empty)
        return;
        OnItemBeginDrag?.Invoke(this);
    }
    public void OnDrop()
    {
        OnItemDroppedOn?.Invoke(this);
    }
    public void OnEndDrag()
    {
        OnItemEndDrag?.Invoke(this);
    }
    public void OnPointerClick(BaseEventData data)
    {
        if (empty)
        return;
        PointerEventData pointerData =(PointerEventData)data;
        if (pointerData.button == PointerEventData.InputButton.Right)
        {
            OnRightMouseBtnClick?.Invoke(this);
        }
        else
        {
            OnItemClicked?.Invoke(this);
        }
    }
}
