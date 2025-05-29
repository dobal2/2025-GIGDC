using System;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float lifeTime = 0.3f;
    private float timer;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        timer = lifeTime;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
            Destroy(gameObject);
    }

    public void SetGhost(Sprite sprite, Vector3 position, Quaternion rotation, Vector3 scale, Color color, bool flipX)
    {
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = scale;

        sr.sprite = sprite;
        sr.color = color;
        sr.flipX = flipX;
    }
}
