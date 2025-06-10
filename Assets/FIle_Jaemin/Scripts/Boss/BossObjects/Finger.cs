using System;
using UnityEngine;
using System.Collections;

public class Finger : MonoBehaviour
{
    public float fallSpeed;
    void Start()
    {
        
    }

    private void Update()
    {
        transform.Translate(new Vector3(0,-fallSpeed,0) * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Platform") || other.CompareTag("Ground"))
        {
            fallSpeed = 0;
            Destroy(gameObject,2);
        }
    }
}