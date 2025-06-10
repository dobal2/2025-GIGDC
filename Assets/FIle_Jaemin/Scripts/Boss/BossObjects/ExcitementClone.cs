using System;
using UnityEngine;

public class ExcitementClone : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float startXPos;
    [SerializeField] private float endXPos;
    
    [SerializeField] private float moveSpeed = 3f;

    [SerializeField] private bool facingRight;
    private bool movingRight = true;

    void Update()
    {
        Vector3 pos = transform.position;
        
        if (movingRight)
        {
            if(!facingRight)
                Flip();
            pos.x += moveSpeed * Time.deltaTime;
            if (pos.x >= endXPos)
            {
                pos.x = endXPos;
                movingRight = false;
            }
        }
        else
        {
            if(facingRight)
                Flip();
            pos.x -= moveSpeed * Time.deltaTime;
            if (pos.x <= startXPos)
            {
                pos.x = startXPos;
                movingRight = true;
            }
        }

        transform.position = pos;
    }
    
    private void Flip()
    {
        facingRight = !facingRight;
        
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y + 180f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}