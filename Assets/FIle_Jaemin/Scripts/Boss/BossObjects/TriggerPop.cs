using System;
using System.Collections;
using UnityEngine;

public class TriggerPop : MonoBehaviour
{
    private Bubble bubble;

    private void Start()
    {
        bubble = GetComponentInParent<Bubble>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(bubble.Explosion(0f));
        }
    }
    
    
}
