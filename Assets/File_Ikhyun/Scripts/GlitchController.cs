using UnityEngine;

public class GlitchController : MonoBehaviour
{
    [SerializeField] private Material glitchMaterial;
    [Range(0, 1)] public float glitchIntensity = 0.0f;
    [SerializeField] private float timeMultiplier = 1.0f;

    void Update()
    {
        if (glitchMaterial != null)
        {
            glitchMaterial.SetFloat("_GlitchIntensity", glitchIntensity);
            glitchMaterial.SetFloat("_TimeMultiplier", timeMultiplier);
        }
    }

    public void SetGlitchIntensity(float intensity)
    {
        glitchIntensity = intensity;
    }
}