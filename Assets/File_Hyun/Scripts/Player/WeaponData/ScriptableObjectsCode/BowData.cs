using System;
using UnityEngine;

[Serializable]
public struct BowComboInfo
{
    public float damage;
    public float pushDistance;
    public float pushTime;
    public float Combodelay;
    public float ComboKeep;
}

[CreateAssetMenu(fileName = "BowData", menuName = "Scriptable Objects/BowData")]
public class BowData : ScriptableObject
{
    [Header("특수공격 정보")]
    public float BowskillDamage; // 특수공격 데미지
    public float Bowskillcooldown; // 특수공격 쿨타임

    [Header("Animator")]
    public RuntimeAnimatorController animatorController;

    [Header("콤보 단계별 정보")]
    public BowComboInfo[] bowComboInfos; // 임시
}