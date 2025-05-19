using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public WeaponType type;

    [Header("일반 공격")]
    public int maxComboCount;
    public float[] comboDamages;
    public float[] comboPushDistances;
    public float[] comboDelays;
}