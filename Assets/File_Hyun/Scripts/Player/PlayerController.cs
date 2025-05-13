using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("�̵� ����")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public LayerMask groundLayer;

    [Header("�ʼ� ����")]
    public Rigidbody2D rb;
    public Transform groundCheck;
    public Animator animator;

    private bool isGrounded = false;

    private void Update()
    {
        HandleMovement();
        HandleJump();
        HandleAttack();
    }

    void HandleMovement()
    {
        float move = 0f;
        if (Input.GetKey(InputManager.Instance.keyData.Player.LeftMoveKey))
            move = -1f;
        else if (Input.GetKey(InputManager.Instance.keyData.Player.RightMoveKey))
            move = 1f;

        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        // �ִϸ��̼� ó�� (����/�ӵ�)
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(move));
            if (move != 0) transform.localScale = new Vector3(move, 1, 1);
        }
    }

    void HandleJump()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        if (isGrounded && Input.GetKeyDown(InputManager.Instance.keyData.Player.JumpKey))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            if (animator != null) animator.SetTrigger("Jump");
        }
    }

    void HandleAttack()
    {
        if (Input.GetKeyDown(InputManager.Instance.keyData.Player.AttackKey))
        {
            var weapon = WeaponManager.Instance.GetCurrentWeapon();

            switch (weapon)
            {
                case WeaponManager.WeaponType.Weapon1:
                    Debug.Log("����1: �⺻ ����!");
                    break;
                case WeaponManager.WeaponType.Weapon2:
                    Debug.Log("����2: ���� ����!");
                    break;
                case WeaponManager.WeaponType.Weapon3:
                    Debug.Log("����3: ���� ����!");
                    break;
            }

            if (animator != null) animator.SetTrigger("Attack");
        }
    }
}