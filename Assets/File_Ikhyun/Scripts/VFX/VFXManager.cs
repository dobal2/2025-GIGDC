using System;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance { get; private set; }

    private Dictionary<Type, List<IVFXEffect>> vfxRegistry = new Dictionary<Type, List<IVFXEffect>>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Register(Type type, GameObject prefab)
    {
        if (!typeof(IVFXEffect).IsAssignableFrom(type)) return;

        if (!vfxRegistry.ContainsKey(type))
        {
            vfxRegistry[type] = new List<IVFXEffect>();
        }

        IVFXEffect effectInstance = (IVFXEffect)Activator.CreateInstance(type);
        effectInstance.Initialize(prefab);
        vfxRegistry[type].Add(effectInstance);
    }

    public void Play(Type type, Vector3 position, float intensity)
    {
        if (!vfxRegistry.ContainsKey(type)) return;

        foreach (IVFXEffect effect in vfxRegistry[type])
        {
            effect.Play(position, intensity);
        }
    }

    public void Stop(Type type)
    {
        if (!vfxRegistry.ContainsKey(type)) return;

        foreach (IVFXEffect effect in vfxRegistry[type])
        {
            effect.Stop();
        }
    }
}