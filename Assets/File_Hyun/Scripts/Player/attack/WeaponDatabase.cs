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
    public WeaponData[] weapons;

    public WeaponData GetData(WeaponType type)
    {
        return weapons.FirstOrDefault(w => w.type == type);
    }
}