using UnityEngine;
using System.Collections;
using static PlayerController;

public class DieState : PlayerState
{
    private bool isWaitingForAnimation = false;

    public DieState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override PlayerStateType StateType => PlayerStateType.Death;

    public override void Enter()
    {
        player.SpriteRenderer.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        player.isNoClip = true;
        player.Rigidbody.linearVelocityX = 0f;

        if (player.isGrounded)
        {
            player.SetEffectState(PlayerEffectState.Dying);
            player.Animator.Play("Death");
            player.StartCoroutine(WaitForAnimationAndDie("Death"));
        }
        else
        {
            player.Animator.Play("Fall");
            isWaitingForAnimation = true;
        }
    }

    public override void Update()
    {
        if (isWaitingForAnimation && player.isGrounded)
        {
            isWaitingForAnimation = false;
            player.SetEffectState(PlayerEffectState.Dying);
            player.Animator.Play("Death");
            player.StartCoroutine(WaitForAnimationAndDie("Death"));
        }
    }

    private IEnumerator WaitForAnimationAndDie(string animationName)
    {
        AnimatorStateInfo stateInfo;
        do
        {
            yield return null;
            stateInfo = player.Animator.GetCurrentAnimatorStateInfo(0);
        }
        while (!stateInfo.IsName(animationName) || stateInfo.normalizedTime < 1f);

        yield return new WaitForSeconds(3f);

        Debug.Log("Player Died");
        StageManager.Instance.Fail();
    }
}