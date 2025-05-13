using UnityEngine;

public class ToggleTarget : MonoBehaviour
{
    public GameObject targetObject;

    public void TurnOn()
    {
        targetObject.SetActive(true);
    }

    public void TurnOff()
    {
        targetObject.SetActive(false);
    }

    public void Toggle()
    {
        targetObject.SetActive(!targetObject.activeSelf);
    }
}