using System;
using UnityEngine;

public class LethargyTriggerExplosion : MonoBehaviour
{
    private LowMonster_Rare_lethargy monster;

    private void Start()
    {
        monster = GetComponentInParent<LowMonster_Rare_lethargy>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && monster.isExplosioned == false)
        {
            StartCoroutine(monster.Explosion(2));   
        }
    }
}
