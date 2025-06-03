using System;
using UnityEngine;

[Serializable]
public struct BowArrowInfo
{
    public Vector2 localOffset;
    public float ShootDelay;
}

[Serializable]
public struct BowComboInfo
{
    public float damage;
    public float pushDistance;
    public float pushTime;
    public float Combodelay;
    public float ComboKeep;

    public BowArrowInfo[] arrows;
}

[CreateAssetMenu(fileName = "BowData", menuName = "Scriptable Objects/BowData")]
public class BowData : ScriptableObject
{
    [Header("특수공격 정보")]
    public float minBowSkillDamage;
    public float maxBowSkillDamage;
    public float laserSkillDamage;
    public float bowSkillcooldown;
    public Vector2 fireOffset;

    [Header("Animator")]
    public RuntimeAnimatorController animatorController;

    [Header("화살 프리팹")]
    public GameObject normalArrowPrefab;
    public GameObject skillArrowPrefab;

    [Header("콤보 단계별 정보")]
    public BowComboInfo[] bowComboInfos;

    public int MaxCombo => bowComboInfos?.Length ?? 0;

    private bool InBounds(int step) => step >= 1 && step <= MaxCombo;

    public float GetDamage(int step) => InBounds(step) ? bowComboInfos[step - 1].damage : 0f;
    public float GetPush(int step) => InBounds(step) ? bowComboInfos[step - 1].pushDistance : 0f;
    public float GetDelay(int step) => InBounds(step) ? bowComboInfos[step - 1].pushTime : 0.1f;
    public float GetComboDelay(int step) => InBounds(step) ? bowComboInfos[step - 1].Combodelay : 0f;
    public float GetComboKeep(int step) => InBounds(step) ? bowComboInfos[step - 1].ComboKeep : 0f;
    public float GetShootDelay(int step) => InBounds(step) && bowComboInfos[step - 1].arrows?.Length > 0 ? bowComboInfos[step - 1].arrows[0].ShootDelay : 0f;
    public BowArrowInfo[] GetArrowInfos(int step) => InBounds(step) ? bowComboInfos[step - 1].arrows : null;
}