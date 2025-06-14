using UnityEngine;

public enum WeaponType
{
    Spear,
    Bow,
    Bomb
}

[System.Serializable]
public struct UnlockedWeapons
{
    public bool Spear;
    public bool Bow;
    public bool Bomb;

    public readonly bool IsUnlocked(WeaponType type) => type switch
    {
        WeaponType.Spear => Spear,
        WeaponType.Bow => Bow,
        WeaponType.Bomb => Bomb,
        _ => false
    };
}

[CreateAssetMenu(fileName = "WeaponDatabase", menuName = "Scriptable Objects/Weapon Database")]
public class WeaponDatabase : ScriptableObject
{
    public SpearData spearData;
    public BowData bowData;
    public BombData bombData;
    public UnlockedWeapons unlockedWeapons;

    public bool IsWeaponUnlocked(WeaponType type) => unlockedWeapons.IsUnlocked(type);

    public ScriptableObject GetRawData(WeaponType type)
    {
        return type switch
        {
            WeaponType.Spear => spearData,
            WeaponType.Bow => bowData,
            WeaponType.Bomb => bombData,
            _ => null
        };
    }

    public T GetData<T>(WeaponType type) where T : ScriptableObject
    {
        return GetRawData(type) as T;
    }
}