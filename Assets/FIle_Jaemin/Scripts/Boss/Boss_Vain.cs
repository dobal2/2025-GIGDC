using System.Collections;
using UnityEngine;

public class BossFrustration : Boss
{
    [Header("Skills")]
    [SerializeField] private GameObject tentaclePrefab;
    [SerializeField] private GameObject goldToothPrefab;
    [SerializeField] private GameObject wormAttackHitbox;

    [SerializeField] private Transform[] tentacleSpawnPoints;
    [SerializeField] private Transform goldToothSpawnPoint;
    [SerializeField] private Transform wormAttackPoint;

    private float skillCooldown = 3f;
    private float lastSkillTime;
    private bool isVulnerable = false;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(VulnerableCycle());
    }

    protected override void Update()
    {
        base.Update();

        if (canAttack && Time.time - lastSkillTime > skillCooldown)
        {
            lastSkillTime = Time.time;
            int skill = Random.Range(1, 4); // 1~3번 스킬
            StartCoroutine(ExecuteSkill(skill));
        }
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
    }

    private IEnumerator DoTentacleAttack()
    {
        yield return new WaitForSeconds(1.5f);
        foreach (var point in tentacleSpawnPoints)
        {
            GameObject tentacle = Instantiate(tentaclePrefab, point.position, Quaternion.identity);
            Destroy(tentacle, 2f); // 2초 후 자동 제거
        }
        yield return new WaitForSeconds(1.0f);
    }

    private IEnumerator DoGoldToothAttack()
    {
        yield return new WaitForSeconds(0.5f);

        GameObject tooth = Instantiate(goldToothPrefab, goldToothSpawnPoint.position, Quaternion.identity);

        Rigidbody2D rb = tooth.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // 왼쪽 포물선 방향 3가지
            Vector2[] arcForces = new Vector2[]
            {
                new Vector2(-4f, 6f),   // 짧은 왼쪽 위
                new Vector2(-6f, 7f),   // 중간
                new Vector2(-8f, 5f)    // 긴 거리
            };

            Vector2 selectedForce = arcForces[Random.Range(0, arcForces.Length)];
            rb.linearVelocity = Vector2.zero; // 초기화
            rb.AddForce(selectedForce, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(0.5f);
    }



    private IEnumerator DoWormAttack()
    {
        anim.SetTrigger("WormAttack");
        wormAttackHitbox.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        wormAttackHitbox.SetActive(false);
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
