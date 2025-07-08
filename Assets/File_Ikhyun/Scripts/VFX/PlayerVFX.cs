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
    private Dissolve dissolve;

    private void Awake() {
        dissolve = GetComponentInParent<Dissolve>();
    }

    void Start()
    {
        _child = transform.Find("Arrow_Charge");
        Instance.OnEffectStateChanged += HandleEffectChange;
        
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
                VFXList[1].Reinit();
                dissolve.DieEffect();
                VFXList[2].Play();
                break;
            case PlayerEffectState.Dash:

                break;
            case PlayerEffectState.SpearAirSkill:

                break;
            default:
                break;
        }
    }
    
}