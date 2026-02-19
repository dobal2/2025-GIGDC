using System.Collections;
using System.Transactions;
using UnityEngine;
using UnityEngine.VFX;

public class Boss_Love : Boss
{
    [Header("Prefabs")]
    [SerializeField] private GameObject noticeDangerPrefab;
    [SerializeField] private GameObject bossInkPrefab;
    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private GameObject heartBubblePrefab;
    [SerializeField] private GameObject bress;

    [Header("Values")] 
    [SerializeField] private int phase = 1;
    [SerializeField] private float fieldXRangeMin;
    [SerializeField] private float fieldXRangeMax;
    [SerializeField] private float fieldYRangeMin;
    [SerializeField] private float fieldYRangeMax;
    [SerializeField] private float normalY;
    [SerializeField] private float digY;
    [SerializeField] private float heartBubbleY;

    private bool isTransforming;

    [SerializeField] private float phase2YPos;
    
    [Header("Ink Setting")]
    [SerializeField] private float inkDamage;
    [SerializeField] private float inkForce;
    
    [Header("Bress Setting")] 
    [SerializeField] private float bressDamage;

    private GameObject newDangerObject;
    private Collider2D col;
    private bool isAttacking;

    [Header("Sounds")] 
    [SerializeField] private AudioSource bressSound;
    [SerializeField] private AudioSource showUpSound;
    [SerializeField] private AudioSource transformSound;
    

    protected override void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        base.Start();
    }

    protected override void Attack()
    {
        
    }

    protected override void Die()
    {
        StopAllCoroutines();
        if (phase == 1)
        {
            Phase2();
        }
        else
        {
            StageManager.Objects = 0;
            gameObject.SetActive(false);
        }
    }

    private void Phase2()
    {
        StopAllCoroutines();
        anim.SetTrigger("Transform");
        transformSound.Play();

        if(newDangerObject != null)
            Destroy(newDangerObject);

        isTransforming = true;

        phase = 2;
        maxHp = phase2Hp;
        hp = maxHp;
        anim.runtimeAnimatorController = phase2Anim;

        rigid.gravityScale = 0;

        transform.position = new Vector2(0, phase2YPos);

        isAttacking = false;
        col.isTrigger = false;

        UpdateHPBar();

        StartCoroutine(DelayEnableTransform());
    }

    IEnumerator DelayEnableTransform()
    {
        yield return new WaitForSeconds(2);
        isTransforming = false;
    }
    private void MoveUpInk()
    {
        int bossInkCount = Random.Range(6, 10);
        for (int i = 0; i < bossInkCount; i++)
        {
            GameObject newBossInk = Instantiate(bossInkPrefab, transform.position, Quaternion.identity);
            newBossInk.GetComponent<Bullet>().damage = inkDamage;
            
            float angle = Random.Range(-20f, 20f);
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.up;
            
            
            Rigidbody2D rb = newBossInk.GetComponent<Rigidbody2D>();
            rb.AddForce(direction * inkForce, ForceMode2D.Impulse);
        }
    }

    private IEnumerator MoveUpPattern()
    {
        col.isTrigger = true;
        rigid.gravityScale = 0;

        transform.position = new Vector2(Random.Range(fieldXRangeMin, fieldXRangeMax), digY);
        newDangerObject = Instantiate(noticeDangerPrefab, new Vector2(transform.position.x, normalY), Quaternion.identity);

        yield return new WaitForSeconds(1.5f);
        Destroy(newDangerObject);

        MoveUpInk();

        float moveSpeed = 13f; // 초당 y축 이동 속도
        while (transform.position.y < normalY)
        {
            if (phase != 1) yield break;
            
            float step = moveSpeed * Time.deltaTime;
            transform.position += new Vector3(0, step, 0);
            yield return null;
        }

        col.isTrigger = false;
        rigid.gravityScale = 1;

        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(BubblePattern());
    }

    
    IEnumerator BubblePattern()
    {
        float bubbleCount = Random.Range(8, 14);

        for (int i = 0; i < bubbleCount; i++)
        {
            Vector2 randomPos = new(Random.Range(fieldXRangeMin, fieldXRangeMax),
                Random.Range(fieldYRangeMin, fieldYRangeMax));
            GameObject newBubble = Instantiate(bubblePrefab, randomPos, Quaternion.identity);
            newBubble.GetComponent<Bubble>().SetBoss(this, false);

        }
        
        yield return new WaitForSeconds(1f);
    }
    
    IEnumerator SpawnAroundBubblePattern()
    {
        int bubbleCount = Random.Range(3, 6);
        float radius = 3.5f; // 버블이 퍼질 반지름
        Vector2 center = transform.position + new Vector3(0,1.5f,0); // 또는 원하는 중심 좌표

        for (int i = 0; i < bubbleCount; i++)
        {
            float angle = i * Mathf.PI * 2f / bubbleCount; // 각도를 나눠서 원형 배치
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Vector2 spawnPos = center + offset;

            GameObject newBubble = Instantiate(bubblePrefab, spawnPos, Quaternion.identity);
            newBubble.GetComponent<Bubble>().SetBoss(this,true);
        }

        yield return new WaitForSeconds(1f);
    }
    
    
    IEnumerator SpawnHeartBubblePattern()
    {
        Vector2 randomPos = new(
            Random.Range(fieldXRangeMin, fieldXRangeMax),
            heartBubbleY
        );

        GameObject newHeartBubble = Instantiate(heartBubblePrefab, randomPos, Quaternion.identity);

        yield return new WaitForSeconds(1f);
    }


    
    IEnumerator BressPattern()
    {
        anim.SetTrigger("Bress");
        bress.GetComponent<VisualEffect>().Play();
        bressSound.Play();
        
        yield return new WaitForSeconds(3f);
        
        bressSound.Stop();
        
        yield return new WaitForSeconds(1f);
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
            if (!isTransforming)
            {
                yield return StartCoroutine(SpawnHeartBubblePattern());   
                yield return StartCoroutine(BressPattern());
                yield return StartCoroutine(SpawnAroundBubblePattern());
                
            }
        }
        
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    protected override void Update()
    {
        base.Update();
        
        if (battleStarted && !isAttacking)
        {
            StartCoroutine(PatternRoutine());
        }
    }
}
