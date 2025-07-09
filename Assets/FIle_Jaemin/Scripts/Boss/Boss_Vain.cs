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
    [SerializeField] private GameObject phase1Map;
    [SerializeField] private GameObject phase2Map;

    [SerializeField] private GameObject aoeWarningPrefab;
    [SerializeField] private GameObject aoeProjectilePrefab;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Basic Attack")]
    [SerializeField] private float basicAttackRange = 2f;
    [SerializeField] private float basicAttackCooldown = 2f;
    private float lastBasicAttackTime = -Mathf.Infinity;

    private float skillCooldown = 3f;
    private float lastSkillTime;
    private bool isVulnerable = false;
    private bool isPerformingSkill = false;
    private int lastUsedSkill = -1;

    [Header("Clone Attack")]
    [SerializeField] private float cloneCooldown = 8f;
    private float lastCloneTime = -Mathf.Infinity;

    [SerializeField] private GameObject clonePrefab;

    private GameObject currentWormBody;
    private GameObject currentWormHead;
    
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private Transform attackCheck;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(VulnerableCycle());
    }

    protected override void Attack()
    {
        
    }

    protected override void Update()
    {
        base.Update();

        if (currentPhase == 2 && !isPerformingSkill)
        {
            FollowPlayer();

            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            if (distanceToPlayer <= basicAttackRange && Time.time - lastBasicAttackTime >= basicAttackCooldown)
            {
                lastBasicAttackTime = Time.time;
                StartCoroutine(DoBasicAttack());
            }
        }

        if (canAttack && !isPerformingSkill && Time.time - lastSkillTime > skillCooldown)
        {
            lastSkillTime = Time.time;
            int newSkill = PickNewSkillAvoidingRepeat(currentPhase == 1 ? 1 : 2, currentPhase == 1 ? 4 : 5, lastUsedSkill);
            lastUsedSkill = newSkill;
            StartCoroutine(ExecuteSkill(newSkill));
        }
    }

    private void FollowPlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.transform.position - transform.position).normalized;
        direction.y = 0;
        transform.position += direction * moveSpeed * Time.deltaTime;

        if (direction.x > 0 && !facingRight) Flip();
        else if (direction.x < 0 && facingRight) Flip();
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

    private void Phase2()
    {
        StopAllCoroutines();
        isPerformingSkill = false;
        canAttack = false;

        if (currentWormBody != null) Destroy(currentWormBody);
        if (currentWormHead != null) Destroy(currentWormHead);
        currentWormBody = null;
        currentWormHead = null;

        anim.SetBool("IsWormAttacking", false);
        anim.ResetTrigger("Throw");
        currentPhase = 2;
        maxHp = phase2Hp;
        hp = maxHp;
        anim.runtimeAnimatorController = phase2Anim;
        phase1Map.SetActive(false);
        phase2Map.SetActive(true);
        transform.localScale = Vector3.one;
        rigid.gravityScale = 1;
        GetComponent<BoxCollider2D>().size = new Vector2(3.64f, 4.73f);
        GetComponent<BoxCollider2D>().offset = new Vector2(-0.03f, -0.05f);
        GetComponent<SpriteRenderer>().flipX = false;

        StartCoroutine(DelayAttackEnable());
    }

    private IEnumerator DelayAttackEnable()
    {
        yield return new WaitForSeconds(0.5f);
        canAttack = true;
    }

    private IEnumerator ExecuteSkill(int skillNum)
    {
        isPerformingSkill = true;
        canAttack = false;

        if (currentPhase == 1)
        {
            switch (skillNum)
            {
                case 1: yield return StartCoroutine(DoTentacleAttack()); break;
                case 2: yield return StartCoroutine(DoGoldToothAttack()); break;
                case 3: yield return StartCoroutine(DoWormAttack()); break;
            }
        }
        else if (currentPhase == 2)
        {
            switch (skillNum)
            {
                case 2:
                    if (Time.time - lastCloneTime >= cloneCooldown)
                    {
                        lastCloneTime = Time.time;
                        yield return StartCoroutine(DoCloneAttack());
                    }
                    else
                    {
                        yield return StartCoroutine(ExecuteSkill(Random.Range(3, 5)));
                        yield break;
                    }
                    break;
                case 3: yield return StartCoroutine(DoAoeAttack()); break;
                case 4: yield return StartCoroutine(DoAmbushAttack()); break;
            }
        }

        yield return new WaitForSeconds(1f);
        canAttack = true;
        isPerformingSkill = false;
    }

    private IEnumerator DoBasicAttack()
    {
        isPerformingSkill = true;
        anim.SetTrigger("Attack1");
        GameObject hitbox = Instantiate(wormAttackHitbox, transform.position, Quaternion.identity);
        Destroy(hitbox, 0.3f);
        yield return new WaitForSeconds(0.5f);
        isPerformingSkill = false;
    }

    private IEnumerator DoAmbushAttack()
    {
        anim.SetTrigger("Teleport");
        yield return new WaitForSeconds(1.0f);
        GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(0.9f);
        GetComponent<SpriteRenderer>().enabled = true;
    }

    public void TeleportToPlayerBack()
    {
        Vector3 playerPos = player.transform.position;
        
        float playerRotY = player.transform.rotation.eulerAngles.y;
        
        Vector3 backDirection = (playerRotY == 0f) ? Vector3.left : Vector3.right;
        
        Vector3 teleportPosition = playerPos + backDirection * 2f;
        teleportPosition.y = transform.position.y;
        
        transform.position = teleportPosition;

        if (backDirection == Vector3.left && !facingRight) Flip();  // 왼쪽인데 왼쪽 보고 있으면 반전 필요
        else if (backDirection == Vector3.right && facingRight) Flip();  // 오른쪽인데 오른쪽 보고 있으면 반전 필요



        anim.SetTrigger("Attack2");
    }



    
    public void AttackOverlapCircle()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackRange);
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                target.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            }
        }
    }

    private IEnumerator DoCloneAttack()
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 spawnOffset = new Vector3(Random.Range(4f, 7f) * (Random.value < 0.5f ? -1 : 1), 0, 0);
        Instantiate(clonePrefab, transform.position + spawnOffset, Quaternion.identity);
        yield return new WaitForSeconds(1.5f);
    }

    private IEnumerator DoAoeAttack()
    {
        anim.SetTrigger("Attack4");
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("IsAttack4Waiting", true);

        GameObject warning = Instantiate(aoeWarningPrefab);
        warning.transform.position = player.transform.position;

        float duration = 3f;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            if (warning != null)
                warning.transform.position = new Vector2(player.transform.position.x, player.transform.position.y - warning.transform.localScale.y / 2);
            yield return null;
        }

        if (warning != null) Destroy(warning);

        Vector3 dropStart = player.transform.position + Vector3.up * 10f;
        GameObject projectile = Instantiate(aoeProjectilePrefab, dropStart, Quaternion.identity);
        projectile.GetComponent<Rigidbody2D>().linearVelocity = Vector2.down * 10f;

        yield return new WaitForSeconds(2f);
        anim.SetBool("IsAttack4Waiting", false);
        anim.SetTrigger("GetWeapon");
    }

    public void Waiting()
    {
        anim.SetBool("IsAttack4Waiting", true);
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
        isPerformingSkill = true;
        yield return new WaitForSeconds(0.5f);
        anim.SetTrigger("Throw");

        GameObject tooth = Instantiate(goldToothPrefab, goldToothSpawnPoint.position, Quaternion.identity);
        Vector2[] arcForces = { new Vector2(-12f, 6f), new Vector2(-6f, 7f), new Vector2(-20f, 5f) };
        tooth.GetComponent<Rigidbody2D>().AddForce(arcForces[Random.Range(0, arcForces.Length)], ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.5f);
        isPerformingSkill = false;
    }

    private IEnumerator DoWormAttack()
    {
        anim.SetBool("IsWormAttacking", true);
        yield return new WaitForSeconds(0.5f);

        Vector3 startPos = wormAttackPoint.position + (facingRight ? Vector3.left : Vector3.right) * 1.5f;
        Vector3 direction = facingRight ? Vector3.left : Vector3.right;

        currentWormBody = Instantiate(wormBodyPrefab, startPos, Quaternion.identity);
        currentWormHead = Instantiate(wormHeadPrefab, startPos, Quaternion.identity);

        float elapsed = 0f;
        while (elapsed < wormGrowDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / wormGrowDuration);
            float currentLength = Mathf.Lerp(0f, wormFinalLength, t);

            currentWormBody.transform.localScale = new Vector3(currentLength, 1.5f, 1.5f);
            currentWormBody.transform.position = startPos + direction * (currentLength / 2f);
            currentWormHead.transform.position = startPos + direction * currentLength;

            yield return null;
        }

        yield return new WaitForSeconds(1f);
        Destroy(currentWormBody);
        Destroy(currentWormHead);
        anim.SetBool("IsWormAttacking", false);
        anim.SetTrigger("EndWorm");
    }

    public override void TakeDamage(float amount)
    {
        if (isVulnerable)
            base.TakeDamage(amount);
        else
            Debug.Log("Boss is invulnerable right now.");
    }

    protected override void Die()
    {
        StopAllCoroutines();
        if (currentPhase == 1)
            Phase2();
        else
        {
            StageManager.Objects--;
            gameObject.SetActive(false);
        }
        
    }
    
    private void OnDrawGizmosSelected()
    {
        if (attackCheck != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawWireSphere(attackCheck.position, attackRange);
        }
    }
}
