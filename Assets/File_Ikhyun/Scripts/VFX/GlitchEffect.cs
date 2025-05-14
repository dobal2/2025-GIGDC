using UnityEngine;
using UnityEngine.VFX;

public class GlitchEffect : IVFXEffect
{
    private VisualEffect vfxComponent;

    public void Initialize(GameObject prefab)
    {
        if (prefab == null) return;

        GameObject instance = GameObject.Instantiate(prefab);
        instance.SetActive(false);
        vfxComponent = instance.GetComponent<VisualEffect>();
    }

    public void Play(Vector3 position, float intensity)
    {
        if (vfxComponent == null) return;

        vfxComponent.transform.position = position;
        vfxComponent.SetFloat("glitchIntensity", intensity);
        vfxComponent.gameObject.SetActive(true);
    }

    public void Stop()
    {
        if (vfxComponent == null) return;
        vfxComponent.gameObject.SetActive(false);
    }
}