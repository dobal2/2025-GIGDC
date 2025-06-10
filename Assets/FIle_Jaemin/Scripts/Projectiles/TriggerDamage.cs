using System;
using UnityEngine;

public class TriggerDamage : MonoBehaviour
{
    public float damage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}
