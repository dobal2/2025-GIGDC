using System.Collections;
using UnityEngine;

public class InkView : MonoBehaviour
{
    private void Awake()
    {
        CameraUtility.TopCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, CameraUtility.TopCamera.nearClipPlane));
        DontDestroyOnLoad(gameObject);
        StartCoroutine(Destroy());
    }

    private IEnumerator Destroy()
    {
        yield return new WaitForSeconds(12f);
        Destroy(gameObject);
    }
}
