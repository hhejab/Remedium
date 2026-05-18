using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    public Light2D globalLight;
    public Color dayColor = Color.white;
    public Color nightColor = new Color(0.1f, 0.15f, 0.4f);
    
    public float cycleDuration = 30f;
    private float timer = 0f;

    void Update()
    {
        if (globalLight == null) return;

        timer += Time.deltaTime;
        float t = Mathf.PingPong(timer / (cycleDuration / 2), 1f);
        
        globalLight.color = Color.Lerp(dayColor, nightColor, t);

        if (timer >= cycleDuration)
        {
            timer = 0f;
        }
    }
}

