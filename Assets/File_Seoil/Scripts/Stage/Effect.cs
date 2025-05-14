using UnityEngine;

public class Effect : MonoBehaviour
{
    [SerializeField]
    private void Destroy()
    {
        Destroy(gameObject);
    }
}
