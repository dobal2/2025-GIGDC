using UnityEngine;

[CreateAssetMenu(fileName = "BombData", menuName = "Scriptable Objects/BombData")]
public class BombData : ScriptableObject
{
    [Header("기본 정보")]
    public GameObject normalBombPrefab;
    public GameObject skillBombPrefab;
    public Vector2 localOffset;
    public float bombExplosionRadius;

    [Header("일반 공격 정보")]
    public float damage;
    public float throwAngle;
    public float throwSpeed;
    public float throwDelay;
    public float normalBombAutoTargetRange;
    public float normalBombAutoTargetArcHeight;

    [Header("특수 공격 정보")]
    public float bombSkillDamage;
    public int bombSkillMaxNumber;
    public int bombSkillMinNumber;
    public float bombSkillMaxExplosionTime;
    public float bombSkillMinExplosionTime;
    public float bombSkillMaxThrowAngle;
    public float bombSkillMinThrowAngle;
    public float bombSkillMaxThrowSpeed;
    public float bombSkillMinThrowSpeed;
    public float Bombskillcooldown;

    [Header("Animator")]
    public RuntimeAnimatorController animatorController;
}