using System.Linq;
using UnityEngine;

public enum WeaponType
{
    Spear,
    Bow,
    Bomb
}

[CreateAssetMenu(fileName = "WeaponDatabase", menuName = "Scriptable Objects/Weapon Database")]
public class WeaponDatabase : ScriptableObject
{
    public SpearData spearData;
    public BowData bowData;
    public BombData bombData;

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