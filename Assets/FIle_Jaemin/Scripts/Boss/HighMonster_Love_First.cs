using System.Collections;
using UnityEngine;

public class HighMonster_Love_First : Monster
{
    [Header("Prefabs")]
    [SerializeField] private GameObject noticeDangerPrefab;
    [SerializeField] private GameObject bossInkPrefab;
    [SerializeField] private GameObject bubblePrefab;
    
    [Header("Values")]
    [SerializeField] private float fieldXRangeMin;
    [SerializeField] private float fieldXRangeMax;
    [SerializeField] private float fieldYRangeMin;
    [SerializeField] private float fieldYRangeMax;
    
    [Header("Ink Setting")]
    [SerializeField] private float inkDamage;
    [SerializeField] private float inkForce;

    [Header("Bubble Setting")] 
    [SerializeField] private float bubbleDamage;
    
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
        throw new System.NotImplementedException();
    }

    protected override void Die()
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator MoveUpPattern()
    {
        collider.isTrigger = true;
        rigid.gravityScale = 0;
        // spriteRenderer.enabled = false;
        
        transform.position = new Vector2(Random.Range(fieldXRangeMin, fieldXRangeMax), 0);
        GameObject newDangerObject = Instantiate(noticeDangerPrefab, new Vector2(transform.position.x,4), Quaternion.identity);
        
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
        
        
        while (transform.position.y < 4)
        {
            transform.position += new Vector3(0, 0.1f, 0);
            yield return new WaitForSeconds(0.001f);
        }
        
        collider.isTrigger = false;
        rigid.gravityScale = 1;
        spriteRenderer.enabled = true;

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

    IEnumerator PatternRoutine()
    {
        isAttacking = true;

        yield return new WaitForSeconds(1f);
        
        yield return StartCoroutine(MoveUpPattern());

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }
    
    void Update()
    {
        if (!isAttacking)
        {
            StartCoroutine(PatternRoutine());
        }
    }
}
