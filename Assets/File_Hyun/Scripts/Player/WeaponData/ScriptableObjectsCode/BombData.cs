using UnityEngine;

[CreateAssetMenu(fileName = "BombData", menuName = "Scriptable Objects/BombData")]
public class BombData : ScriptableObject
{
    [Header("기본정보")]
    public GameObject normalBombPrefab;
    public GameObject skillBombPrefab;
    public Vector2 localOffset;
    public float bombExplosionRadius;

    [Header("특수공격 정보")]
    public float bombSkillDamage;
    public float bombSkillMaxNumber;
    public float bombSkillMinNumber;
    public float bombSkillMaxExplosionTime;
    public float bombSkillMinExplosionTime;
    public float bombSkillMaxThrowAngle;
    public float bombSkillMinThrowAngle;
    public float Bombskillcooldown;

    [Header("Animator")]
    public RuntimeAnimatorController animatorController;

    [Header("일반공격 정보")]
    public float damage;
    public float throwAngle;
    public float throwSpeed;
    public float throwDelay;
}