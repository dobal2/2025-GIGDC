using UnityEngine;

public class Tentacle : MonoBehaviour
{
    [SerializeField] private float damage;
    
    public void DestoryObject()
    {
        Destroy(gameObject);
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
            Destroy(gameObject);
        }
    }
}
