using System.Collections;
using UnityEngine;

public class Frustration : Monster
{
    private float windTimer = 0f;
    private float fingerTimer = 0f;
    private int takeDamageCount = 0;
    private int currentPhase = 1;

    public GameObject windPrefab;
    public GameObject fingerPrefab;
    public Transform[] bossPositions;

    private int currentPosIndex = 0;

    [Header("Finger Attack Area")]
    [SerializeField] private float fingerMinX = -5f;
    [SerializeField] private float fingerMaxX = 5f;
    [SerializeField] private float fingerMinY = -2f;
    [SerializeField] private float fingerMaxY = 3f;
    void Update()
    {
        windTimer += Time.deltaTime;
        fingerTimer += Time.deltaTime;

        if (windTimer >= 7f)
        {
            StartCoroutine(CastWindAttack());
            windTimer = 0f;
        }

        if (fingerTimer >= 5f)
        {
            StartCoroutine(CastFingerAttack());
            fingerTimer = 0f;
        }

        if (takeDamageCount >= 5)
        {
            MoveBossRandomly();
            takeDamageCount = 0;
        }
    }

    IEnumerator CastWindAttack()
    {
        for (int i = 0; i < 2; i++)
        {
            if (player == null) yield break;

            Vector3 dir = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            GameObject wind = Instantiate(windPrefab, transform.position, Quaternion.Euler(0, 0, angle));
            Rigidbody2D rb = wind.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = dir * 5f;
            }

            yield return new WaitForSeconds(0.6f);
        }
    }

    IEnumerator CastFingerAttack()
    {
        int count = Random.Range(5, 9);
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(fingerMinX, fingerMaxX),
                Random.Range(fingerMinY, fingerMaxY),
                0f
            );

            Instantiate(fingerPrefab, randomPos, Quaternion.identity);
            yield return new WaitForSeconds(0.2f);
        }
    }

    void MoveBossRandomly()
    {
        if (bossPositions.Length < 2) return;

        currentPosIndex = (currentPosIndex + 1) % bossPositions.Length;
        transform.position = bossPositions[currentPosIndex].position;
    }

    public override void TakeDamage(float amount)
    {
        takeDamageCount++;
        base.TakeDamage(amount);
    }

    protected override void Attack()
    {
        // 스킬 패턴이 Update에서 자동 실행됨
    }

    protected override void Die()
    {
        // 사망 애니메이션 또는 효과 추가 가능
    }
}
