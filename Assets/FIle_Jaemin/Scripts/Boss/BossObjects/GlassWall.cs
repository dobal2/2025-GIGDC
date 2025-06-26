using UnityEngine;

public class GlassWall : Monster
{
    [SerializeField] private GameObject glassEffectPrefab;
    void Start()
    {
        Destroy(gameObject,20);
    }

    protected override void Attack()
    {
        
    }

    void Update()
    {
        if (hp <= 0)
        {
            Die();
        }
        transform.Translate(new Vector3(speed,0,0) * Time.deltaTime);
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!other.GetComponent<PlayerController>().CanTakeDamage)
            {
                return;
            }
            other.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }

    protected override void Die()
    {
        ParticleSystem newGlassEffect = Instantiate(glassEffectPrefab, transform.position, Quaternion.Euler(-180f,90f,0f)).GetComponent<ParticleSystem>();
        newGlassEffect.Play();
        Destroy(newGlassEffect,3);
        Destroy(gameObject);
    }
    
    
}
