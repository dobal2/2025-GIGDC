using System.Collections;
using UnityEngine;

public class BossVain : Boss
{
    [Header("Skills")]
    [SerializeField] private GameObject tentaclePrefab;
    [SerializeField] private GameObject goldToothPrefab;
    [SerializeField] private GameObject wormAttackHitbox;

    [SerializeField] private Transform[] tentacleSpawnPoints;
    [SerializeField] private Transform goldToothSpawnPoint;
    [SerializeField] private Transform wormAttackPoint;
    [SerializeField] private GameObject wormBodyPrefab;
    [SerializeField] private GameObject wormHeadPrefab;
    [SerializeField] private float wormGrowDuration = 1.5f;
    [SerializeField] private float wormFinalLength = 8f;

    private float skillCooldown = 3f;
    private float lastSkillTime;
    private bool isVulnerable = false;
    private bool isPerformingSkill = false;

    private int lastUsedSkill = -1;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(VulnerableCycle());
    }

    protected override void Update()
    {
        base.Update();

        if (canAttack && !isPerformingSkill && Time.time - lastSkillTime > skillCooldown)
        {
            lastSkillTime = Time.time;

            int newSkill = PickNewSkillAvoidingRepeat(1, 4, lastUsedSkill);
            lastUsedSkill = newSkill;

            StartCoroutine(ExecuteSkill(newSkill));
        }
    }

    private int PickNewSkillAvoidingRepeat(int minInclusive, int maxExclusive, int lastSkill)
    {
        int skill;
        do
        {
            skill = Random.Range(minInclusive, maxExclusive);
        } while (skill == lastSkill && maxExclusive - minInclusive > 1);
        return skill;
    }

    private IEnumerator VulnerableCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            isVulnerable = true;
            Debug.Log("Boss is now vulnerable!");
            yield return new WaitForSeconds(4f);
            isVulnerable = false;
            Debug.Log("Boss is no longer vulnerable.");
        }
    }

    private IEnumerator ExecuteSkill(int skillNum)
    {
        isPerformingSkill = true;
        canAttack = false;

        switch (skillNum)
        {
            case 1:
                yield return StartCoroutine(DoTentacleAttack());
                break;
            case 2:
                yield return StartCoroutine(DoGoldToothAttack());
                break;
            case 3:
                yield return StartCoroutine(DoWormAttack());
                break;
        }

        yield return new WaitForSeconds(1f);
        canAttack = true;
        isPerformingSkill = false;
    }

    private IEnumerator DoTentacleAttack()
    {
        yield return new WaitForSeconds(1.5f);

        foreach (var point in tentacleSpawnPoints)
        {
            GameObject tentacle = Instantiate(tentaclePrefab, point.position, Quaternion.identity);
            Destroy(tentacle, 2f);
        }

        yield return new WaitForSeconds(1f);
    }

    private IEnumerator DoGoldToothAttack()
    {
        yield return new WaitForSeconds(0.5f);

        anim.SetTrigger("Throw");

        GameObject tooth = Instantiate(goldToothPrefab, goldToothSpawnPoint.position, Quaternion.identity);
        Rigidbody2D rb = tooth.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2[] arcForces = new Vector2[]
            {
                new Vector2(-12f, 6f),
                new Vector2(-6f, 7f),
                new Vector2(-20f, 5f)
            };

            Vector2 selectedForce = arcForces[Random.Range(0, arcForces.Length)];
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(selectedForce, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator DoWormAttack()
    {
        anim.SetBool("IsWormAttacking", true);
        yield return new WaitForSeconds(0.5f);

        Vector3 startPos = wormAttackPoint.position;
        Vector3 direction = facingRight ? Vector3.right : Vector3.left;

        float bodyScaleY = 1.5f;
        float bodyScaleZ = 1.5f;

        // 몸통 생성 (X축 길이 0으로 시작)
        GameObject body = Instantiate(wormBodyPrefab, startPos, Quaternion.identity);
        body.transform.localScale = new Vector3(0f, bodyScaleY, bodyScaleZ);
        body.transform.position = startPos;

        // 머리 생성 (고정 크기)
        GameObject head = Instantiate(wormHeadPrefab, startPos, Quaternion.identity);
        head.transform.localScale = new Vector3(3f, 3f, 3f); // ✅ 크기 고정

        float elapsed = 0f;

        while (elapsed < wormGrowDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / wormGrowDuration);
            float currentLength = Mathf.Lerp(0f, wormFinalLength, t);

            // 몸통 X축 스케일 늘리기
            body.transform.localScale = new Vector3(currentLength, bodyScaleY, bodyScaleZ);

            // 위치: 기준점에서 절반만큼 이동 (Pivot이 왼쪽 기준이어야 정확)
            body.transform.position = startPos + direction * (currentLength / 2f);

            // 머리는 몸통 끝에 위치, 고정 크기
            head.transform.position = startPos + direction * currentLength;

            yield return null;
        }

        yield return new WaitForSeconds(1f);
        Destroy(body);
        Destroy(head);
        anim.SetBool("IsWormAttacking", false);
        anim.SetTrigger("EndWorm");
    }


    public override void TakeDamage(float amount)
    {
        if (isVulnerable)
        {
            base.TakeDamage(amount);
        }
        else
        {
            Debug.Log("Boss is invulnerable right now.");
        }
    }

    protected override void Die()
    {
        Debug.Log("BossFrustration Die()");
    }
}
