using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public Sprite itemIcon;

    public void PickUp()
    {
        Destroy(gameObject);
    }
}
