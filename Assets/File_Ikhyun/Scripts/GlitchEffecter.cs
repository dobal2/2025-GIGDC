using System;
using UnityEngine;
using TMPro;

public class GlitchEffecter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmpText;
    [SerializeField] private float noiseScale = 2f;

    private Material tmpMaterial;

    private void Awake() {
        SetNoiseScale(0);
        tmpMaterial = tmpText.fontMaterial;
    }

    private void Update() {
        tmpMaterial.SetFloat("_NoiseScale", noiseScale);
    }

    public void SetNoiseScale(float newScale)
    {
        noiseScale = newScale;

        if (tmpMaterial != null)
        {
            tmpMaterial.SetFloat("_NoiseScale", noiseScale);
        }
    }
    
    public void StopSelectEffect()
    {
        SetNoiseScale(0);
    }

    public void PlayHoverEffect()
    {
        if (!isActiveAndEnabled) return;

        SetNoiseScale(2);
    }

    // public void PlayClickEffect()
    // {
    //     if (!isActiveAndEnabled) return;
    //
    //     StopAllCoroutines();
    //     SetTextMaterial(glitchMaterial);
    // }

    private void OnDisable()
    {
        StopAllCoroutines();
        StopSelectEffect();
    }

    
}