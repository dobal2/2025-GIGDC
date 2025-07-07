using UnityEngine;
using System.Collections;
using static PlayerController;

public class DieState : PlayerState
{
    private bool hasPlayedDeath;

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
        }
        else
        {
            player.Animator.Play("Fall");
        }

        hasPlayedDeath = player.isGrounded;
    }

    public override void Update()
    {
        if (!hasPlayedDeath && player.isGrounded)
        {
            player.SetEffectState(PlayerEffectState.Dying);
            player.Animator.Play("Death");
            hasPlayedDeath = true;
        }

        if (hasPlayedDeath && player.Animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
        {
            if (player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                player.StartCoroutine(Die());
            }
        }
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(3f);

        Debug.Log("Player Died");
        StageManager.Instance.Fail();
    }
}