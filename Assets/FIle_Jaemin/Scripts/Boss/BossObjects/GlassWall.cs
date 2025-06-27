using Unity.Mathematics;
using UnityEngine;

public class GlassWall : Monster
{
    [SerializeField] private GameObject glassEffectPrefab;

    protected override void Start()
    {
        //base.Start();
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
    

    protected override void Die()
    {
        ParticleSystem newGlassEffect = Instantiate(glassEffectPrefab, transform.position, Quaternion.Euler(-180f,90f,0f)).GetComponent<ParticleSystem>();
        
        // float yRotation = player.transform.eulerAngles.y;
        //
        // if (Mathf.Approximately(yRotation, 0f))
        // {
        //     newGlassEffect.gameObject.transform.rotation = Quaternion.Euler(-180f,90f,0f);
        // }
        // else if (Mathf.Approximately(yRotation, 180f))
        // {
        //     newGlassEffect.gameObject.transform.rotation = Quaternion.Euler(0f,90f,0f);
        // }
        
        newGlassEffect.Play();
        Destroy(newGlassEffect,3);
        Destroy(gameObject);
    }
    
    
}
