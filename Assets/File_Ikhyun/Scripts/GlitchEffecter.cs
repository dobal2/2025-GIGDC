using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GlitchEffecter : MonoBehaviour
{
    [SerializeField] private Material glitchMaterial;
    
    [SerializeField] private float maxIntensity = 0.5f;
    [SerializeField] private float minIntensity = 0.1f;
    
    [SerializeField] private float maxTimer = 2f;
    [SerializeField] private float minTimer = 0.2f;
    [SerializeField] private float decayDuration = 3f;

    private static GlitchEffecter instance;
    private Dictionary<Graphic, Coroutine> activeEffects = new Dictionary<Graphic, Coroutine>();

    private void Awake()
    {
        instance = this;
    }

    
    public static void Active(Graphic target)
    {
        if (instance == null || target == null) return;

        
        if (instance.activeEffects.ContainsKey(target))
        {
            instance.StopCoroutine(instance.activeEffects[target]);
        }

        
        Material instanceMaterial = new Material(instance.glitchMaterial);
        target.material = instanceMaterial;

        
        Coroutine coroutine = instance.StartCoroutine(instance.LerpEffect(target, instanceMaterial));
        instance.activeEffects[target] = coroutine;
    }

    
    public static void Deactivate(Graphic target)
    {
        if (instance == null || target == null) return;

        if (instance.activeEffects.ContainsKey(target))
        {
            instance.StopCoroutine(instance.activeEffects[target]);
            instance.activeEffects.Remove(target);
        }

        
        target.material = null;
    }

    
    private IEnumerator LerpEffect(Graphic target, Material material)
    {
        float elapsedTime = 0f;

        // 시작 값 설정
        material.SetFloat("_Intensity", maxIntensity);
        material.SetFloat("_Timer", maxTimer);

        while (elapsedTime < decayDuration)
        {
            elapsedTime += Time.deltaTime;
            
            float t = elapsedTime / decayDuration;
            
            float intensity = Mathf.Lerp(maxIntensity, minIntensity, Mathf.SmoothStep(0f, 1f, t));
            float timer = Mathf.Lerp(maxTimer, minTimer, Mathf.SmoothStep(0f, 1f, t));

            material.SetFloat("_Intensity", intensity);
            material.SetFloat("_Timer", timer);

            yield return null;
        }
        
        material.SetFloat("_Intensity", minIntensity);
        material.SetFloat("_Timer", minTimer);
    }
}
