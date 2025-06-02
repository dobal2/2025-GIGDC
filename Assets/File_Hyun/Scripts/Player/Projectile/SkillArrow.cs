using UnityEngine;

public class SkillArrow : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float maxDistance = 30f;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private LayerMask enemyMask;

    private Vector2 direction;
    private Vector2 startPosition;

    private Animator Animator;

    void Start()
    {
        Animator = GetComponent<Animator>();

        direction = Vector2.right * PlayerController.Instance.facingDirection;
        startPosition = transform.position;
    }

    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        Vector2 currentPosition = (Vector2)transform.position;

        // 충돌 체크: 적
        RaycastHit2D hitEnemy = Physics2D.Raycast(currentPosition, direction, moveDistance, enemyMask);
        if (hitEnemy.collider != null)
        {
            OnHitEnemy(hitEnemy.collider);
            Destroy(gameObject);
            return;
        }

        // 충돌 체크: 벽, 땅 등
        RaycastHit2D hit = Physics2D.Raycast(currentPosition, direction, moveDistance, collisionMask);
        if (hit.collider != null)
        {
            Destroy(gameObject);
            return;
        }

        // 이동
        transform.Translate(direction * moveDistance, Space.World);

        // 최대 거리 초과 시 파괴
        if (Vector2.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnHitEnemy(Collider2D enemy)
    {
        Debug.Log($"[Arrow] 적 명중: {enemy.name}");
        // TODO: 애니메이션 또는 데미지 처리 추가
    }
}