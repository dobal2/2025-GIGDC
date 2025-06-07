using UnityEngine;

[CreateAssetMenu(fileName = "BombData", menuName = "Scriptable Objects/BombData")]
public class BombData : ScriptableObject
{
    [Header("특수공격 정보")]
    public float bombSkillDamage;
    public float bombSkillMaxNumber;
    public float bombSkillMinNumber;
    public float bombSkillMaxExplosionTime;
    public float bombSkillMinExplosionTime;
    public float Bombskillcooldown;

    [Header("Animator")]
    public RuntimeAnimatorController animatorController;

    [Header("일반공격 정보")]
    public float bombDamage;
    public float bombExplosionRadius;
}