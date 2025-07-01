using UnityEngine;
using System.Collections;

public class SkillBomb : MonoBehaviour
{
    [SerializeField] private LayerMask enemyMask;

    private float bombDamage;
    private float bombThrowAngle;
    private float bombThrowSpeed;
    private float bombExplosionRadius;
    private float fuseTime;

    private Vector2 Direction;
    private Rigidbody2D rb;
    private Animator animator;
    private float timer;
    private bool isExploded = false;

    public void Initialize(Vector2 currentDirection, float damage, float throwAngle, float throwSpeed, float explosionRadius, float timeToExplode)
    {
        bombDamage = damage;
        bombThrowAngle = throwAngle;
        bombThrowSpeed = throwSpeed;
        bombExplosionRadius = explosionRadius;
        fuseTime = timeToExplode;
        isExploded = false;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();


        animator.Play("Bomb");
        float angleInRadians = bombThrowAngle * Mathf.Deg2Rad;
        float directionX = Mathf.Sign(currentDirection.x);
        float adjustedAngle = directionX >= 0 ? angleInRadians : Mathf.PI - angleInRadians;
        rb.AddTorque(Random.Range(-3f, 3f), ForceMode2D.Impulse);

        Direction = new Vector2(Mathf.Cos(adjustedAngle), Mathf.Sin(adjustedAngle)).normalized;
        rb.linearVelocity = Direction * bombThrowSpeed;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= fuseTime)
            Explode();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        int otherLayer = collision.gameObject.layer;
        if (((1 << otherLayer) & enemyMask) != 0)
            Explode();
    }

    void Explode()
    {
        if (isExploded) return;
        isExploded = true;
        CameraUtility.ShakeCamera(
            duration: 0.3f,
            strength: 0.1f,
            vibrato: 10,
            randomness: 90,
            fadeOut: true
        );

        DebugDrawDiameter(transform.position, PlayerController.Instance.AttackController.bombData.bombExplosionRadius ,0.3f);

        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation |
                         RigidbodyConstraints2D.FreezePositionX |
                         RigidbodyConstraints2D.FreezePositionY;

        rb.rotation = 0;
        
        StartCoroutine(Destroy());
        animator.Play("Boom");

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, bombExplosionRadius, enemyMask);
        foreach (var hit in hitColliders)
        {
            if (hit.TryGetComponent<Monster>(out var monster))
            {
                monster.TakeDamage(bombDamage);
                monster.KnockBack(
                attacker: this.transform,
                knockBackForce: 0.6f * bombDamage,
                knockBackAngle: 45,
                duration: 0.15f
            );
            }
        }
    }

    IEnumerator Destroy()
    {
        yield return null;
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        float waitTime = state.length;
        yield return new WaitForSeconds(waitTime);
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