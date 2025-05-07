using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class LowMonster_Rare_inferior : Monster
{
    [SerializeField] private GameObject attackGameObject;
    [SerializeField] private float attackScaleTime = 0.2f;
    [SerializeField] private float maxScale = 2f;
    private bool didVerticalAttack;

    protected override void Start()
    {
        if(attackGameObject == null)
            Debug.LogError("No attackGameObject prefab assigned");
            
    }

    protected override void Attack()
    {
        if (didVerticalAttack)
        {
            didVerticalAttack = false;
            StartCoroutine(DoHorizontalAttack());
        }
        else
        {
            didVerticalAttack = true;
            StartCoroutine(DoVerticalAttack());
        }
    }

    protected override void Die()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (canAttack)
        {
            Attack();
            StartCoroutine(WaitToAttack(attackCoolDown));
        }
    }

    private IEnumerator DoHorizontalAttack()
    {
        Vector3 originalScale = attackGameObject.transform.localScale;

        yield return StartCoroutine(ScaleTo(attackGameObject, new Vector3(maxScale, 0.6f, 1f), attackScaleTime));
        yield return StartCoroutine(ScaleTo(attackGameObject, originalScale, attackScaleTime));
    }

    private IEnumerator DoVerticalAttack()
    {
        Vector3 originalScale = attackGameObject.transform.localScale;

        yield return StartCoroutine(ScaleTo(attackGameObject, new Vector3(0.6f, maxScale, 1f), attackScaleTime));
        yield return StartCoroutine(ScaleTo(attackGameObject, originalScale, attackScaleTime));
    }

    private IEnumerator ScaleTo(GameObject obj, Vector3 targetScale, float duration)
    {
        Vector3 startScale = obj.transform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            obj.transform.localScale = Vector3.Lerp(startScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.transform.localScale = targetScale;
        
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}
