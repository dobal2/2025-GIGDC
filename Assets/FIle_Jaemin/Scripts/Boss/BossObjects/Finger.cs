using UnityEngine;
using System.Collections;

public class Finger : MonoBehaviour
{
    [SerializeField] private float fallDuration = 1.5f;
    [SerializeField] private float fallDistance = 5f;

    void Start()
    {
        StartCoroutine(Fall());
    }

    IEnumerator Fall()
    {
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.down * fallDistance;
        float elapsed = 0f;

        while (elapsed < fallDuration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / fallDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
    }
}