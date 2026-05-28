using UnityEngine;
using System.Collections.Generic;

public class ChestUI : MonoBehaviour
{
    public ItemUI[] slots;

    public void RefreshUI(List<ItemData> items)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count && items[i] != null)
                slots[i].SetupSlot(items[i]);
            else
                slots[i].ClearSlot();
        }
    }
}