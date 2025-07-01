using UnityEngine;
using static PlayerController;

public class SkillArrow : MonoBehaviour
{
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private LayerMask enemyMask;

    private Vector2 direction;
    private Vector2 startPosition;
    private float projectileDamage;
    private float speed;
    private float maxDistance;

    public void Initialize(Vector2 currentDirection, float damage, float speed, float maxDistance)
    {
        direction = currentDirection.normalized;
        projectileDamage = damage;
        this.speed = speed;
        this.maxDistance = maxDistance;
        startPosition = transform.position;
        transform.rotation = Quaternion.Euler(0f, direction.x == -1 ? 180f : 0f, 0f);
    }

    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        transform.Translate(direction * moveDistance, Space.World);

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
                monster.KnockBack(
                    attacker: this.transform,
                    knockBackForce: 0.5f * projectileDamage,
                    knockBackAngle: 0,
                    duration: 0.1f
                );
                Destroy(gameObject);
            }
        }
        else if (((1 << otherLayer) & collisionMask) != 0)
        {
            Destroy(gameObject);
        }
    }
}