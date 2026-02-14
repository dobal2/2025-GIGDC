using System.Collections;
using UnityEngine;

public class VainProjectile : MonoBehaviour
{
    [SerializeField] private float damage;
    public Transform vain;
    private Animator anim;
    private Rigidbody2D rb;

    private Transform target;
    private bool isFollowing = false;
    private bool hasTriggered = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isFollowing && target != null)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            rb.linearVelocity = dir * 10f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            var pc = other.GetComponent<PlayerController>();
            if (pc != null && !pc.CanTakeDamage) return;

            hasTriggered = true;
            target = other.transform;
            isFollowing = true;
            StartCoroutine(FollowThenHit(other.GetComponent<PlayerHealth>()));
        }
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator FollowThenHit(PlayerHealth playerHealth)
    {
        yield return new WaitForSeconds(0.2f);
        isFollowing = false;
        rb.linearVelocity = Vector2.zero;

        anim.SetTrigger("Hit");

        yield return new WaitForSeconds(0.1f);
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}