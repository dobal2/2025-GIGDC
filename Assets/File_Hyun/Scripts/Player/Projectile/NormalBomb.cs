using System.Collections;
using UnityEngine;

public class NormalBomb : MonoBehaviour
{
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private float autoTargetArrivalDistance = 0.2f;

    private float bombDamage;
    private float bombThrowAngle;
    private float bombThrowSpeed;
    private float bombExplosionRadius;

    private Vector2 direction;
    private Vector2 autoTargetPoint;
    private Rigidbody2D rb;
    private Animator animator;
    private bool isExploded;
    private bool useAutoTargetTrajectory;

    public void Initialize(Vector2 currentDirection, float damage, float throwAngle, float throwSpeed, float explosionRadius)
    {
        Setup(damage, throwAngle, throwSpeed, explosionRadius);
        LaunchDefault(currentDirection);
    }

    public void Initialize(Vector2 currentDirection, float damage, float throwAngle, float throwSpeed, float explosionRadius, Vector2 targetPoint, float arcExtraHeight)
    {
        Setup(damage, throwAngle, throwSpeed, explosionRadius);

        autoTargetPoint = targetPoint;
        useAutoTargetTrajectory = TryLaunchAutoTarget(targetPoint, arcExtraHeight);

        if (!useAutoTargetTrajectory)
            LaunchDefault(currentDirection);
    }

    public void Update()
    {
        if (!useAutoTargetTrajectory || isExploded || rb.linearVelocity.y > 0f)
            return;

        if (((Vector2)transform.position - autoTargetPoint).sqrMagnitude > autoTargetArrivalDistance * autoTargetArrivalDistance)
            return;

        transform.position = autoTargetPoint;
        Explode();
    }

    private void Setup(float damage, float throwAngle, float throwSpeed, float explosionRadius)
    {
        bombDamage = damage;
        bombThrowAngle = throwAngle;
        bombThrowSpeed = throwSpeed;
        bombExplosionRadius = explosionRadius;
        isExploded = false;
        useAutoTargetTrajectory = false;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        animator.Play("Bomb");
    }

    private void LaunchDefault(Vector2 currentDirection)
    {
        float angleInRadians = bombThrowAngle * Mathf.Deg2Rad;
        float directionX = Mathf.Sign(currentDirection.x);
        float adjustedAngle = directionX >= 0 ? angleInRadians : Mathf.PI - angleInRadians;
        rb.AddTorque(directionX * 10f, ForceMode2D.Impulse);

        direction = new Vector2(Mathf.Cos(adjustedAngle), Mathf.Sin(adjustedAngle)).normalized;
        rb.linearVelocity = direction * bombThrowSpeed;
    }

    private bool TryLaunchAutoTarget(Vector2 targetPoint, float arcExtraHeight)
    {
        float gravityMagnitude = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        if (!CombatTargetingUtility.TryGetBallisticVelocity(transform.position, targetPoint, gravityMagnitude, arcExtraHeight, out Vector2 velocity))
            return false;

        float directionX = Mathf.Abs(velocity.x) <= 0.01f ? 1f : Mathf.Sign(velocity.x);
        rb.AddTorque(directionX * 10f, ForceMode2D.Impulse);
        rb.linearVelocity = velocity;
        direction = velocity.normalized;
        return true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        int otherLayer = other.gameObject.layer;

        if (((1 << otherLayer) & enemyMask) != 0 || ((1 << otherLayer) & collisionMask) != 0)
            Explode();
    }

    private void Explode()
    {
        if (isExploded)
            return;

        isExploded = true;

        CameraUtility.ShakeCamera(
            duration: 0.3f,
            strength: 0.3f,
            vibrato: 10,
            randomness: 90,
            fadeOut: true
        );

        DebugDrawDiameter(transform.position, bombExplosionRadius, 0.3f);

        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation |
                         RigidbodyConstraints2D.FreezePositionX |
                         RigidbodyConstraints2D.FreezePositionY;
        rb.rotation = 0f;

        StartCoroutine(DestroyRoutine());
        animator.Play("Boom");
        PlayerController.Instance.PlayClip(PlayerController.Instance.BombBoom);

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, bombExplosionRadius, enemyMask);
        foreach (Collider2D hit in hitColliders)
        {
            if (!hit.TryGetComponent<Monster>(out Monster monster))
                continue;

            monster.TakeDamage(bombDamage);
            monster.KnockBack(
                attacker: transform,
                knockBackForce: 0.5f * bombDamage,
                knockBackAngle: 45f,
                duration: 0.2f
            );
        }
    }

    private IEnumerator DestroyRoutine()
    {
        yield return null;
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(state.length);
        Destroy(gameObject);
    }

    private void DebugDrawDiameter(Vector2 center, float radius, float duration)
    {
#if UNITY_EDITOR
        Color color = Color.red;
        Vector2 left = center + Vector2.left * radius;
        Vector2 right = center + Vector2.right * radius;
        Vector2 down = center + Vector2.down * radius;
        Vector2 up = center + Vector2.up * radius;

        Debug.DrawLine(left, right, color, duration);
        Debug.DrawLine(down, up, color, duration);
#endif
    }
}