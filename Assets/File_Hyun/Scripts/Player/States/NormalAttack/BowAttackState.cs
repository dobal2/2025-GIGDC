using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BowAttackState : PlayerState
{
    private float timer;
    private float pushTimer;
    private int currentArrowIndex;
    private float pushSpeedPerSecond;
    private List<(Vector2 localOffset, float delay)> scheduledArrows;

    private BowData bowData;

    public BowAttackState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
        bowData = player.AttackController.bowData;
    }

    public override PlayerStateType StateType => PlayerStateType.BowAttack;
    public override bool IsCombatState => true;

    public override void Enter()
    {
        player.Rigidbody.linearVelocity = Vector2.zero;
        if (!player.isGrounded)
            player.Rigidbody.constraints |= RigidbodyConstraints2D.FreezePositionY;

        // 콤보 단계
        int step = player.AttackController.ComboStep;

        // 화살 발사 스케줄 준비
        var arrowInfos = bowData.GetArrowInfos(step);
        scheduledArrows = arrowInfos?.Select(a => (a.localOffset, a.ShootDelay)).ToList() ?? new();
        currentArrowIndex = 0;
        timer = 0f;

        // 밀림 설정
        float distance = bowData.GetPush(step);
        pushTimer = bowData.GetDelay(step);
        pushSpeedPerSecond = pushTimer > 0f ? distance / pushTimer : 0f;
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
            stateMachine.ChangeState(new PlayerLocomotionState(player, stateMachine));
        }
    }

    public override void FixedUpdate()
    {
        if (pushTimer > 0f)
        {
            float push = pushSpeedPerSecond * Time.fixedDeltaTime;
            player.Rigidbody.MovePosition(player.Rigidbody.position + new Vector2(push * player.facingDirection, 0f));
        }
    }

    public override void Exit()
    {
        player.Rigidbody.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
    }

    private void FireArrow(Vector2 localOffset)
    {
        Debug.Log($"[Bow] 화살 발사 준비: {localOffset}");

        var arrowPrefab = bowData.normalArrowPrefab;
        if (arrowPrefab == null)
        {
            Debug.LogError("[Bow] 화살 프리팹이 할당되지 않았습니다.");
            return;
        }

        Vector2 firePos = (Vector2)player.transform.position + new Vector2(localOffset.x * player.facingDirection, localOffset.y);
        GameObject arrow = Object.Instantiate(arrowPrefab, firePos, Quaternion.identity);

        Debug.Log($"[Bow] 화살 발사됨 at {firePos}");
    }
}