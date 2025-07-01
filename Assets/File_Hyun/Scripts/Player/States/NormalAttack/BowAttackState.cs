using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BowAttackState : PlayerState
{
    private float timer;
    private float pushTimer;
    private int currentArrowIndex;
    private List<(Vector2 localOffset, float delay)> scheduledArrows;

    private readonly BowData bowData;

    public BowAttackState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { bowData = player.AttackController.bowData; }

    public override PlayerStateType StateType => PlayerStateType.BowAttack;
    public override bool IsCombatState => true;

    public override void Enter()
    {
        player.Rigidbody.linearVelocity = Vector2.zero;
        if (!player.isGrounded)
            player.Rigidbody.constraints |= RigidbodyConstraints2D.FreezePositionY;

        // 화살 발사 스케줄 준비
        var arrowInfos = bowData.GetArrowInfos(player.AttackController.ComboStep);
        scheduledArrows = arrowInfos?.Select(a => (a.localOffset, a.ShootDelay)).ToList() ?? new();
        currentArrowIndex = 0;
        timer = 0f;
    }

    public override void Exit()
    {
        player.Rigidbody.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        player.AttackController.UpdateComboTimer();

        if (pushTimer > 0f)
            pushTimer -= Time.deltaTime;

        // 화살 발사 조건 체크
        while (currentArrowIndex < scheduledArrows.Count &&
               timer >= scheduledArrows[currentArrowIndex].delay)
        {
            FireArrow(scheduledArrows[currentArrowIndex].localOffset);
            currentArrowIndex++;
        }

        if (TryHandleDash()) return;
        if (TryHandleSkillInput()) return;

        if (player.AttackBuffered && player.AttackController.CanComboInput)
        {
            player.ConsumeAttackBuffer();
            player.AttackController.ContinueCombo();
            stateMachine.ChangeState(new BowAttackState(player, stateMachine));
            return;
        }

        if (player.AttackController.CanMove)
        {
            stateMachine.ChangeState(new LocomotionState(player, stateMachine));
        }
    }

    public override void FixedUpdate()
    {
        if (player.AttackController.IsPushing)
        {
            float push = player.AttackController.GetPushDelta(Time.fixedDeltaTime);
            player.Rigidbody.MovePosition(player.Rigidbody.position + new Vector2(push * player.facingDirection, 0f));
        }
    }

    private void FireArrow(Vector2 localOffset)
    {
        var arrowPrefab = bowData.normalArrowPrefab;
        if (arrowPrefab == null)
        {
            Debug.LogError("[Bow] 화살 프리팹이 할당되지 않았습니다.");
            return;
        }

        if(player.AttackController.ComboStep == 4)
        {
            CameraUtility.ShakeCamera(
                duration: 0.3f,
                strength: 0.2f,
                vibrato: 10,
                randomness: 90,
                fadeOut: true
            );
        }

        Vector2 firePos = (Vector2)player.transform.position + new Vector2(localOffset.x * player.facingDirection, localOffset.y);
        GameObject arrow = Object.Instantiate(arrowPrefab, firePos, Quaternion.identity);
        Vector2 direction = new(PlayerController.Instance.facingDirection, 0);
        arrow.GetComponent<NormalArrow>().Initialize(player.transform, direction, bowData.GetDamage(player.AttackController.ComboStep), bowData.speed, bowData.maxDistance);
    }
}