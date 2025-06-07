using System;
using UnityEngine;

public class ExcitementClone : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float startXPos;
    [SerializeField] private float endXPos;
    
    [SerializeField] private float moveSpeed = 3f;

    private bool movingRight = true;

    void Update()
    {
        Vector3 pos = transform.position;
        
        if (movingRight)
        {
            pos.x += moveSpeed * Time.deltaTime;
            if (pos.x >= endXPos)
            {
                pos.x = endXPos;
                movingRight = false;
            }
        }
        else
        {
            pos.x -= moveSpeed * Time.deltaTime;
            if (pos.x <= startXPos)
            {
                pos.x = startXPos;
                movingRight = true;
            }
        }

        transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}