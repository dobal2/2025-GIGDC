using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum WeaponType
{
    Spear,
    Bow,
    Unplanned
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public WeaponType type;

    [Header("일반 공격")]
    public int maxComboCount = 3;
    public float[] comboDamages;
    public float[] comboPushDistances;
    public float[] comboDelays;
}

[CreateAssetMenu(fileName = "WeaponDatabase", menuName = "Scriptable Objects/Weapon Database")]
public class WeaponDatabase : ScriptableObject
{
    public List<WeaponData> weapons;

    public WeaponData GetData(WeaponType type)
    {
        return weapons.FirstOrDefault(w => w.type == type);
    }
}