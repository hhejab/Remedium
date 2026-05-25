using UnityEngine;

public class CreditsAutoScroll : MonoBehaviour
{
    public float scrollSpeed = 40f; // سرعة الصعود
    public float stopPositionY = 1200f; // متى يتوقف النص (حسب طول نصك)

    private RectTransform rectTransform;
    private Vector3 startPosition;

    void OnEnable()
    {
        // أول ما تفتح الشاشة، النص يبدأ من تحت
        rectTransform = GetComponent<RectTransform>();
        startPosition = new Vector3(0, -700, 0); // قيمة تقريبية ليكون النص تحت الشاشة
        rectTransform.anchoredPosition = startPosition;
    }

    void Update()
    {
        // تحريك النص للأعلى
        if (rectTransform.anchoredPosition.y < stopPositionY)
        {
            rectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
        }
    }
}
