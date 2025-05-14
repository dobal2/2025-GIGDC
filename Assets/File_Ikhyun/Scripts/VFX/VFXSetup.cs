using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VFXEntry
{
    public string vfxName;
    public GameObject prefab;
}

public class VFXSetup : MonoBehaviour
{
    [Header("VFX List")]
    [Tooltip("VFX Type & Prefab List")]
    public List<VFXEntry> vfxEntries = new List<VFXEntry>();

    void Awake()
    {
        foreach (VFXEntry entry in vfxEntries)
        {
            if (string.IsNullOrEmpty(entry.vfxName) || entry.prefab == null) continue;

            Type type = Type.GetType(entry.vfxName);
            if (type == null)
            {
                Debug.LogWarning($"VFX Type '{entry.vfxName}' does not exist.");
                continue;
            }

            if (!typeof(IVFXEffect).IsAssignableFrom(type))
            {
                Debug.LogWarning($"'{entry.vfxName}' dose not implement IVFXEffect.");
                continue;
            }

            VFXManager.Instance.Register(type, entry.prefab);
        }
    }
}