using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static PlayerController;



public class VFXController : MonoBehaviour {
    [SerializeField] private List<VisualEffect> VFXList;

    private Transform child;
    
    void Start()
    {
        child = transform.Find("Arrow_Charge");
        Instance.OnEffectStateChanged += HandleEffectChange;
    }

    private void Update() {
        VFXList[1].SetVector3("Attractor", child.position);
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
            default:
                break;
        }
    }
}
