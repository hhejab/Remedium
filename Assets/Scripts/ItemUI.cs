using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public Image iconImage;

    public void SetupSlot(ItemData item)
    {
        if (iconImage != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = true;
        }
    }

    public void ClearSlot()
    {
        if (iconImage != null) iconImage.enabled = false;
    }
}