using UnityEngine;

public class RestorationFilm : MonoBehaviour
{
    [SerializeField] float healAmount = 1f;
    [SerializeField] float floatAmplitude = 0.3f;
    [SerializeField] float floatSpeed = 3f;

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerHealth.Instance.TakeHeal(healAmount);
        Destroy(gameObject);
    }
}
