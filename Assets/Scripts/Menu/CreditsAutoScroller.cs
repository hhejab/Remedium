using UnityEngine;

public class CreditsAutoScroll : MonoBehaviour
{
    public float scrollSpeed = 40f; 
    public float stopPositionY = 1200f; 

    private RectTransform rectTransform;
    private Vector3 startPosition;

    void OnEnable()
    {
       
        rectTransform = GetComponent<RectTransform>();
        startPosition = new Vector3(0, -700, 0); 
        rectTransform.anchoredPosition = startPosition;
    }

    void Update()
    {
        
        if (rectTransform.anchoredPosition.y < stopPositionY)
        {
            rectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
        }
    }
}
