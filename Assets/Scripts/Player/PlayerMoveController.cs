using UnityEngine;

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(SpriteRenderer))]
public class PlayerMoveController : MonoBehaviour
{
    [Header("Player MonoBehaviors")]
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Move Settings")]
    [SerializeField] private float moveSpeed;

    private int horizontalMoveState = 0;

    public void OnMoveRightKeyDown()
    {
        horizontalMoveState++;
        SyncMoveState();
    }

    public void OnMoveRightKeyUp()
    {
        horizontalMoveState--;
        SyncMoveState();
    }

    public void OnMoveLeftKeyDown()
    {
        horizontalMoveState--;
        SyncMoveState();
    }

    public void OnMoveLeftKeyUp()
    {
        horizontalMoveState++;
        SyncMoveState();
    }

    private void SyncMoveState()
    {
        if(horizontalMoveState > 0)
        {
            spriteRenderer.flipX = false;
        }

        if(horizontalMoveState < 0)
        {
            spriteRenderer.flipX = true;
        }

        rigidBody.linearVelocityX = moveSpeed * horizontalMoveState;
    }
}
