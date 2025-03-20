using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnRainEffect : MonoBehaviour
{
    ParticleSystem ps;
    List<ParticleSystem.Particle> inside = new List<ParticleSystem.Particle>();

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void OnParticleTrigger()
    {
        Debug.Log("Particle Trigger");
        
        ps.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, inside);

        foreach (var v in inside)
        {
            Debug.Log("Effect Triggered 2");
        }
    }
}
