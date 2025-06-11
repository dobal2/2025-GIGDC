using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Dissolve : MonoBehaviour {
    [SerializeField] private float _dissolveTIme = 0.75f;
    [SerializeField] private VisualEffect VFX;
    
    private SpriteRenderer _spriteRenderers;
    private Material _materials;
    
    

    private int _dissolveAmount = Shader.PropertyToID("_DissolveAmount");

    private void Start() {
        _spriteRenderers = GetComponent<SpriteRenderer>();
        _materials = _spriteRenderers.material;
        
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R))
            StartCoroutine(Vanish());
        if (Input.GetKeyDown(KeyCode.T))
            StartCoroutine(Appear());
    }


    private IEnumerator Vanish() {
        float elapsedTime = 0f;
        VFX.Play();
        
        while (elapsedTime < _dissolveTIme) {
            elapsedTime += Time.deltaTime;

            float learpedDissolve = Mathf.Lerp(0.2f, 1f, (elapsedTime / _dissolveTIme));
            
            _materials.SetFloat(_dissolveAmount, learpedDissolve);
            
            yield return null;
        }
    }
    
    private IEnumerator Appear() {
        float elapsedTime = 0f;

        while (elapsedTime < _dissolveTIme) {
            elapsedTime += Time.deltaTime;

            float learpedDissolve = Mathf.Lerp(1f, 0f, (elapsedTime / _dissolveTIme));
            
            _materials.SetFloat(_dissolveAmount, learpedDissolve);
            
            yield return null;
        }
    }
}
