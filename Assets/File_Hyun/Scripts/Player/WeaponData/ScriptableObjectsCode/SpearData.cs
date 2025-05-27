using System;
using UnityEngine;

[Serializable]
public struct SpearComboInfo
{
    public float damage;
    public float pushDistance;
    public float pushTime;
    public float Combodelay;
    public float ComboKeep;
}

[CreateAssetMenu(fileName = "SpearData", menuName = "Scriptable Objects/SpearData")]
public class SpearData : ScriptableObject
{
    [Header("특수공격 정보")]
    public float spearSkillDamage; // 특수공격 데미지
    public float spearSkillcooldown; // 특수공격 쿨타임

    [Header("Animator")]
    public RuntimeAnimatorController animatorController;

    [Header("콤보 단계별 정보")]
    public SpearComboInfo[] comboInfos;

    public int MaxCombo => comboInfos?.Length ?? 0;

    public float GetPush(int step)
    {
        if (step < 1 || step > MaxCombo)
            return 0f;
        return comboInfos[step - 1].pushDistance;
    }

    public float GetDelay(int step)
    {
        if (step < 1 || step > MaxCombo)
            return 0.1f;
        return comboInfos[step - 1].pushTime;
    }

    public float GetComboDelay(int step)
    {
        if (step < 1 || step > MaxCombo)
            return 0f;
        return comboInfos[step - 1].Combodelay;
    }

    public float GetComboKeep(int step)
    {
        if (step < 1 || step > MaxCombo)
            return 0f;
        return comboInfos[step - 1].ComboKeep;
    }
}