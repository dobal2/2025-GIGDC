using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static PlayerController;



public class PlayerVFX : MonoBehaviour {
    [SerializeField] private List<VisualEffect> VFXList;

    //Charge
    private Transform _child;
    
    [Header("Crunkcy")]
    [SerializeField] private float dissolveTime = 1.5f;
    private int dissolveAmount = Shader.PropertyToID("_DissolveAmount");
    private SpriteRenderer _spriteRenderers;
    private Material _materials;
    
    void Start()
    {
        _child = transform.Find("Arrow_Charge");
        Instance.OnEffectStateChanged += HandleEffectChange;
        
        _spriteRenderers = GetComponent<SpriteRenderer>();
        _materials = _spriteRenderers.material;
    }

    private void Update() {
        VFXList[1].SetVector3("Attractor", _child.position);
    }

    void HandleEffectChange(PlayerEffectState state)
    {
        switch (state)
        {
            case PlayerEffectState.BowSkillCharging:
                VFXList[1].Play();
                break;
            case PlayerEffectState.BowSkillFullChargeRelease:
                VFXList[1].Reinit();
                VFXList[0].Play();
                break;
            case PlayerEffectState.BowSkillRelease:
                VFXList[1].Reinit();
                break;
            case PlayerEffectState.Dying:
                StartCoroutine(Vanish());
                break;
            case PlayerEffectState.Dash:

                break;
            case PlayerEffectState.SpearAirSkill:

                break;
            default:
                break;
        }
    }
    
    private IEnumerator Vanish() {
        float elapsedTime = 0f;
        VFXList[2].Play();
        
        while (elapsedTime < dissolveTime) {
            elapsedTime += Time.deltaTime;

            float learpedDissolve = Mathf.Lerp(0.2f, 1f, (elapsedTime / dissolveTime));
            
            _materials.SetFloat(dissolveAmount, learpedDissolve);
            
            yield return null;
        }
    }
}