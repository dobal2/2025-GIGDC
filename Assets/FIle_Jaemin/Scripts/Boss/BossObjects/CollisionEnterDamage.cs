using System;
using UnityEngine;

public class CollisionEnterDamage : MonoBehaviour
{
    [SerializeField] private float damage;
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!other.gameObject.GetComponent<PlayerController>().CanTakeDamage)
            {
                return;
            }
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}
