using UnityEngine;

public class GlassWall : MonoBehaviour
{
    [SerializeField] private float hp = 15;
    [SerializeField] private float speed = 1;
    
    void Start()
    {
        Destroy(gameObject,20);
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

    private void Die()
    {
        Destroy(gameObject);
    }
}
