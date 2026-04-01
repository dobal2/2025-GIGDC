using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterState : PlayerState
{
    private bool hitProcessed;
    private bool enteredGrounded;

    public CounterState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override PlayerStateType StateType => PlayerStateType.Counter;
    public override bool IsCombatState => true;

    public override void Enter()
    {
        enteredGrounded = player.isGrounded;
        hitProcessed = false;

        player.CounterPressed = false;
        player.DashPressed = false;
        player.SkillPressed = false;
        player.ChangePressed = false;
        player.jumpBufferTimer = 0f;
        player.ConsumeAttackBuffer();
        player.Rigidbody.linearVelocity = Vector2.zero;
        player.AttackController.ResetCombo();
        player.MarkCounterUsed();

        if (!enteredGrounded)
            player.Rigidbody.constraints |= RigidbodyConstraints2D.FreezePositionY;

        player.Animator.Play(enteredGrounded ? "Player_Attack_Counter" : "Player_Attack_Counter_Flying");
        player.StartCoroutine(DelayedCounterHit());
    }

    public override void Exit()
    {
        player.Rigidbody.linearVelocity = Vector2.zero;
        player.Rigidbody.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
    }

    public override void Update()
    {
        AnimatorStateInfo animInfo = player.Animator.GetCurrentAnimatorStateInfo(0);
        string stateName = enteredGrounded ? "Player_Attack_Counter" : "Player_Attack_Counter_Flying";

        if (animInfo.IsName(stateName) && animInfo.normalizedTime >= 1f)
            stateMachine.ChangeState(new LocomotionState(player, stateMachine));
    }

    private IEnumerator DelayedCounterHit()
    {
        yield return new WaitForSeconds(player.CounterHitDelay);
        TryCounterHit();
    }

    private void TryCounterHit()
    {
        if (hitProcessed)
            return;

        hitProcessed = true;

        Vector2 offset = player.GetCounterOffset(enteredGrounded);
        offset.x *= player.facingDirection;

        Vector2 center = (Vector2)player.transform.position + offset;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            center,
            player.CounterBoxSize,
            0f,
            Vector2.right * player.facingDirection,
            0f,
            player.CounterTargetLayer
        );

        bool hasHitMonster = false;
        HashSet<Monster> hitMonsters = new();
        List<Vector3> orbSpawnPositions = new();

        foreach (RaycastHit2D hit in hits)
        {
            Monster monster = hit.collider.GetComponent<Monster>();
            if (monster == null || !hitMonsters.Add(monster))
                continue;

            if (!monster.OnCounterHit())
                continue;

            hasHitMonster = true;
            orbSpawnPositions.Add(hit.collider.bounds.center);
        }

        if (hasHitMonster)
        {
            foreach (Vector3 spawnPosition in orbSpawnPositions)
                player.SpawnCounterEnergyOrbs(spawnPosition);
        }

        player.NotifyCounterTry(hasHitMonster);
        DebugDrawBox(center, player.CounterBoxSize, 0.2f);
    }

    private void DebugDrawBox(Vector2 center, Vector2 size, float duration)
    {
#if UNITY_EDITOR
        Vector2 half = size * 0.5f;
        Vector2 topLeft = center + new Vector2(-half.x, half.y);
        Vector2 topRight = center + new Vector2(half.x, half.y);
        Vector2 bottomLeft = center + new Vector2(-half.x, -half.y);
        Vector2 bottomRight = center + new Vector2(half.x, -half.y);

        Debug.DrawLine(topLeft, topRight, Color.cyan, duration);
        Debug.DrawLine(topRight, bottomRight, Color.cyan, duration);
        Debug.DrawLine(bottomRight, bottomLeft, Color.cyan, duration);
        Debug.DrawLine(bottomLeft, topLeft, Color.cyan, duration);
#endif
    }
}