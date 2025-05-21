using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    protected int direction;

    public virtual void Initialize(int facingDir)
    {
        direction = Mathf.Clamp(facingDir, -1, 1);
    }

    protected virtual void Update()
    {
        transform.position += new Vector3(speed * direction * Time.deltaTime, 0f, 0f);
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        Destroy(gameObject);
    }
}