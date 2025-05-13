using System.Collections;
using UnityEngine;

public class HighMonster_Love_First : Monster
{
    [Header("Prefabs")]
    [SerializeField] private GameObject noticeDangerPrefab;
    [SerializeField] private GameObject bossInkPrefab;
    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private GameObject bressPrefab;

    [Header("Values")] 
    [SerializeField] private int phase = 1;
    [SerializeField] private float phase2Hp;
    [SerializeField] private float fieldXRangeMin;
    [SerializeField] private float fieldXRangeMax;
    [SerializeField] private float fieldYRangeMin;
    [SerializeField] private float fieldYRangeMax;
    [SerializeField] private float normalY;
    [SerializeField] private float digY;
    
    [Header("Ink Setting")]
    [SerializeField] private float inkDamage;
    [SerializeField] private float inkForce;

    [Header("Bubble Setting")] 
    [SerializeField] private float bubbleDamage;

    [Header("Bress Setting")] 
    [SerializeField] private float bressDamage;
    
    private SpriteRenderer spriteRenderer;
    private Collider2D collider;
    private bool isAttacking;

    protected override void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        base.Start();
        StartCoroutine(PatternRoutine());
    }

    protected override void Attack()
    {
        if (!isAttacking)
        {
            StartCoroutine(PatternRoutine());
        }
    }

    protected override void Die()
    {
        StopAllCoroutines();
        if (phase == 1)
        {
            phase = 2;
            maxHp = phase2Hp;
            hp = maxHp;

            rigid.gravityScale = 0;

            transform.position = new Vector2(0, 7);

            isAttacking = false;
            collider.isTrigger = false;
            
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator MoveUpPattern()
    {
        collider.isTrigger = true;
        rigid.gravityScale = 0;
        
        transform.position = new Vector2(Random.Range(fieldXRangeMin, fieldXRangeMax), digY);
        GameObject newDangerObject = Instantiate(noticeDangerPrefab, new Vector2(transform.position.x,normalY), Quaternion.identity);
        
        yield return new WaitForSeconds(1.5f);
        Destroy(newDangerObject);
        
        int bossInkCount = Random.Range(10, 15);
        for (int i = 0; i < bossInkCount; i++)
        {
            GameObject newBossInk = Instantiate(bossInkPrefab, transform.position, Quaternion.identity);
            newBossInk.GetComponent<Bullet>().damage = inkDamage;
            
            float angle = Random.Range(-40f, 40f);
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.up;
            
            
            Rigidbody2D rb = newBossInk.GetComponent<Rigidbody2D>();
            rb.AddForce(direction * inkForce, ForceMode2D.Impulse);
        }
        
        
        while (transform.position.y < normalY)
        {
            transform.position += new Vector3(0, 0.1f, 0);
            yield return new WaitForSeconds(0.001f);
        }
        
        collider.isTrigger = false;
        rigid.gravityScale = 1;

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(BubblePattern());
    }
    
    IEnumerator BubblePattern()
    {
        float bubbleCount = Random.Range(8, 14);
        
        for (int i = 0; i < bubbleCount; i++)
        {
            Vector2 randomPos = new Vector2(Random.Range(fieldXRangeMin, fieldXRangeMax),
                Random.Range(fieldYRangeMin, fieldYRangeMax));
            GameObject newBubble = Instantiate(bubblePrefab, randomPos, Quaternion.identity);
            newBubble.GetComponent<Bubble>().damage = bubbleDamage;

        }
        
        yield return new WaitForSeconds(1f);
    }
    
    IEnumerator SpawnAroundBubblePattern()
    {
        int bubbleCount = Random.Range(3, 6);
        float radius = 4f; // 버블이 퍼질 반지름
        Vector2 center = transform.position; // 또는 원하는 중심 좌표

        for (int i = 0; i < bubbleCount; i++)
        {
            float angle = i * Mathf.PI * 2f / bubbleCount; // 각도를 나눠서 원형 배치
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Vector2 spawnPos = center + offset;

            GameObject newBubble = Instantiate(bubblePrefab, spawnPos, Quaternion.identity);
            newBubble.GetComponent<Bubble>().damage = bubbleDamage;
        }

        yield return new WaitForSeconds(1f);
    }

    
    IEnumerator BressPattern()
    {
        GameObject newBress = Instantiate(bressPrefab, transform.position, Quaternion.identity);
        newBress.GetComponent<Bress>().damage = bressDamage;
        newBress.transform.rotation *= Quaternion.Euler(new Vector3(0, 0, -60f));

        while (Mathf.DeltaAngle(0, newBress.transform.eulerAngles.z) < 60)
        {
            newBress.transform.rotation *= Quaternion.Euler(new Vector3(0, 0, 0.2f));
            yield return new WaitForSeconds(0.001f);
        }
        
        yield return new WaitForSeconds(0.5f);
        
        while (Mathf.DeltaAngle(0, newBress.transform.eulerAngles.z) > -60f)
        {
            newBress.transform.rotation *= Quaternion.Euler(new Vector3(0, 0, -0.2f));
            yield return new WaitForSeconds(0.001f);
        }
        
        yield return new WaitForSeconds(0.5f);
        
        Destroy(newBress);
        
        yield return new WaitForSeconds(2f);
    }

    IEnumerator PatternRoutine()
    {
        isAttacking = true;
        
        yield return new WaitForSeconds(1f);

        if (phase == 1)
        {
            yield return StartCoroutine(MoveUpPattern());    
        }
        else if (phase == 2)
        {
            yield return StartCoroutine(BressPattern());
            yield return StartCoroutine(SpawnAroundBubblePattern());
        }
        else
        {
            Debug.LogError("fuck");
        }
        
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }
    
    void Update()
    {
        if (hp <= 0) Die();
        
        Attack();
    }
}
