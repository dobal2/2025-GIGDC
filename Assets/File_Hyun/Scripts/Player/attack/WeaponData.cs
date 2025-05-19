using System;
using UnityEngine;

[Serializable]
public struct ComboInfo
{
    public float damage; // 콤보당 데미지
    public float pushDistance; // 콤보당 밀리는 거리 (음수일 수 있음)
    public float pushTime; // 콤보당 밀리는 시간
    public float ComboKeep; // 각 콤보가 끝난 뒤 다음 콤보를 누르기까지 유지될 수 있는 시간 (콤보가 끝난 뒤 측정 시작, 측정 시작 시 움직이기 가능)
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/Weapon")]
public class WeaponData : ScriptableObject
{
    public WeaponType type;

    [Header("콤보 단계별 정보")]
    public ComboInfo[] comboInfos;

    public int maxComboCount => comboInfos?.Length ?? 0;

    public float GetPush(int step)
    {
        if (step < 1 || step > maxComboCount)
            return 0f;
        return comboInfos[step - 1].pushDistance;
    }

    public float GetDelay(int step)
    {
        if (step < 1 || step > maxComboCount)
            return 0.1f;
        return comboInfos[step - 1].pushTime;
    }
}