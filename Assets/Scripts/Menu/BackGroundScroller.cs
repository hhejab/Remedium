using UnityEngine;
using UnityEngine.UI;

public class BackgroundScroller : MonoBehaviour
{
    public RawImage img;
    public float xSpeed, ySpeed;

    void Update()
    {
        // هذا الكود يحرك "إحداثيات" الصورة داخل المربع، فيعطي إيحاء بالحركة المستمرة
        img.uvRect = new Rect(img.uvRect.position + new Vector2(xSpeed, ySpeed) * Time.deltaTime, img.uvRect.size);
    }
}
