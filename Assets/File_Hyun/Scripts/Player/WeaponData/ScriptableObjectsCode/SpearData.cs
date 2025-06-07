using System;
using UnityEngine;

[Serializable]
public struct SpearComboInfo
{
    public float damage;
    public float pushDistance;
    public float pushTime;
    public float comboDelay;
    public float comboKeep;
    public float range;
    public float damageDelay;
    public Vector2 localOffset;
}

[CreateAssetMenu(fileName = "SpearData", menuName = "Scriptable Objects/SpearData")]
public class SpearData : ScriptableObject
{
    [Header("특수공격 정보")]
    public float spearSkillDamage;
    public float spearSkillRange;
    public float jumpSpeed;
    public float spearSkillCooldown;

    [Header("Animator")]
    public RuntimeAnimatorController animatorController;

    [Header("콤보 단계별 정보")]
    public SpearComboInfo[] spearComboInfos;

    public int MaxCombo => spearComboInfos?.Length ?? 0;
    private bool InBounds(int step) => step >= 1 && step <= MaxCombo;

    public float GetDamage(int step) => InBounds(step) ? spearComboInfos[step - 1].damage : 0f;
    public float GetPush(int step) => InBounds(step) ? spearComboInfos[step - 1].pushDistance : 0f;
    public float GetDelay(int step) => InBounds(step) ? spearComboInfos[step - 1].pushTime : 0.1f;
    public float GetComboDelay(int step) => InBounds(step) ? spearComboInfos[step - 1].comboDelay : 0f;
    public float GetComboKeep(int step) => InBounds(step) ? spearComboInfos[step - 1].comboKeep : 0f;
}