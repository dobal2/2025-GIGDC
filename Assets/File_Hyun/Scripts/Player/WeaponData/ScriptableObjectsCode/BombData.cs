using System;
using UnityEngine;

[Serializable]
public struct BombComboInfo
{
    public float damage;
    public float pushDistance;
    public float pushTime;
    public float Combodelay;
    public float ComboKeep;
}

[CreateAssetMenu(fileName = "BombData", menuName = "Scriptable Objects/BombData")]
public class BombData : ScriptableObject
{
    [Header("특수공격 정보")]
    public float BombskillDamage; // 특수공격 데미지
    public float Bombskillcooldown; // 특수공격 쿨타임

    [Header("Animator")]
    public RuntimeAnimatorController animatorController;

    [Header("콤보 단계별 정보")]
    public BombComboInfo[] BombComboInfos; //임시
}