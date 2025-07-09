using UnityEngine;

public class PositionLocker : MonoBehaviour
{
    private Vector3 fixedWorldPos;

    void Start()
    {
        fixedWorldPos = transform.position;
    }

    void LateUpdate()
    {
        transform.position = fixedWorldPos;
    }
}
