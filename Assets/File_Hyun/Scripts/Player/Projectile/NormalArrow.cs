using UnityEngine;

public class NormalArrow : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float maxDistance = 30f;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private LayerMask enemyMask;

    private Vector2 direction;
    private Vector2 startPosition;
    private float projectileDamage;

    private Animator Animator;

    public void Initialize(Vector2 currentDirection, float damage)
    {
        direction = currentDirection.normalized;
        projectileDamage = damage;
        startPosition = transform.position;
        Animator = GetComponent<Animator>();
    }

    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        transform.Translate(direction * moveDistance, Space.World);

        // 최대 거리 초과 시 파괴
        if (Vector2.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        int otherLayer = other.gameObject.layer;

        if (((1 << otherLayer) & enemyMask) != 0)
        {
            if (other.TryGetComponent<Monster>(out var monster))
            {
                monster.TakeDamage(projectileDamage);
                Destroy(gameObject);
            }
        }
        else if (((1 << otherLayer) & collisionMask) != 0)
        {
            Destroy(gameObject);
        }
    }
}