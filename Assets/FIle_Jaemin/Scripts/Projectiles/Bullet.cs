using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;

    private void Start()
    {
        Destroy(gameObject, 20f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.GetComponent<PlayerController>().isNoClip)
            {
                return;
            }
            other.GetComponent<PlayerHealth>().TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
