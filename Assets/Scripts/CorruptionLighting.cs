using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CorruptionLighting : MonoBehaviour
{
    [Header("Corruption Value")]
    [Range(0f, 1f)]
    public float corruptionAmount = 1f;

    [Header("Lighting")]
    public Light2D globalLight;
    public Color corruptedLightColor = new Color(0.45f, 0.05f, 0.85f);
    public Color normalLightColor = Color.white;
    public float corruptedIntensity = 0.6f;
    public float normalIntensity = 1f;

    [Header("Materials")]
    public Material[] corruptionMaterials;

    [Header("Smooth Change")]
    public float smoothSpeed = 2f;

    private float currentCorruption;

    void Start()
    {
        currentCorruption = corruptionAmount;
        ApplyCorruption();
    }

    void Update()
    {
        currentCorruption = Mathf.Lerp(currentCorruption, corruptionAmount, Time.deltaTime * smoothSpeed);
        ApplyCorruption();
    }

    void ApplyCorruption()
    {
        foreach (Material mat in corruptionMaterials)
        {
            if (mat != null)
                mat.SetFloat("_CorruptionAmount", currentCorruption);
        }

        if (globalLight != null)
        {
            globalLight.color = Color.Lerp(normalLightColor, corruptedLightColor, currentCorruption);
            globalLight.intensity = Mathf.Lerp(normalIntensity, corruptedIntensity, currentCorruption);
        }
    }

    public void ReduceCorruption(float amount)
    {
        corruptionAmount = Mathf.Clamp01(corruptionAmount - amount);
    }
}