using System.Collections;
using UnityEngine;

public class CameraShakeTest : MonoBehaviour
{
    [SerializeField] private float frequency;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        while (true)
        {
            yield return new WaitForSeconds(frequency);
            CameraUtility.ShakeCamera();
        }
    }
}
