using UnityEngine;

public class NormalBomb : MonoBehaviour
{
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private LayerMask enemyMask;

    private float bombDamage;
    private float bombThrowAngle;
    private float bombThrowSpeed;
    private float bombExplosionRadius;

    private Vector2 Direction;
    private Rigidbody2D rb;
    private Animator animator;

    public void Initialize(Vector2 currentDirection, float damage, float throwAngle, float throwSpeed, float explosionRadius)
    {
        bombDamage = damage;
        bombThrowAngle = throwAngle;
        bombThrowSpeed = throwSpeed;
        bombExplosionRadius = explosionRadius;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        float angleInRadians = bombThrowAngle * Mathf.Deg2Rad;

        float directionX = Mathf.Sign(currentDirection.x);
        float adjustedAngle = directionX >= 0 ? angleInRadians : Mathf.PI - angleInRadians;

        Direction = new Vector2(Mathf.Cos(adjustedAngle), Mathf.Sin(adjustedAngle)).normalized;
        rb.linearVelocity = Direction * bombThrowSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        int otherLayer = other.gameObject.layer;

        if (((1 << otherLayer) & enemyMask) != 0 || ((1 << otherLayer) & collisionMask) != 0)
        {
            Explode();
        }
    }

    void Explode()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, bombExplosionRadius, enemyMask);
        foreach (var hit in hitColliders)
        {
            if (hit.TryGetComponent<Monster>(out var monster))
                monster.TakeDamage(bombDamage);
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, bombExplosionRadius);
    }
}