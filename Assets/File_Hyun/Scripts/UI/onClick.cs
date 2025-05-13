using UnityEngine;

public class onClick : MonoBehaviour
{
    [SerializeField] private bool ShowLog = true;

    public void onclick()
    {
        if (ShowLog) Debug.Log("onClick");
    }
}
