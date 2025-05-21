using System;
using System.Collections;
using UnityEngine;

public class InterestEffect : MonoBehaviour
{
    private bool facingRight = true;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(FadeEffect());
    }

    public void Flip()
    {
        facingRight = !facingRight;
        
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    
    IEnumerator FadeEffect()
    {
        Color color = _spriteRenderer.color;

        while (color.a > 0f)
        {
            color.a -= 0.03f;
            _spriteRenderer.color = color;
            yield return new WaitForSeconds(0.01f);
        }
        
        Destroy(gameObject);
    }

}
