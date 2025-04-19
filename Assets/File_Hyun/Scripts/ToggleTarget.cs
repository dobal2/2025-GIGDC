using UnityEngine;

public class ToggleTarget : MonoBehaviour
{
    public GameObject targetObject;

    public void TurnOn()
    {
        Debug.Log("on");
        targetObject.SetActive(true);
    }

    public void TurnOff()
    {
        Debug.Log("off");
        targetObject.SetActive(false);
    }

    public void Toggle()
    {
        targetObject.SetActive(!targetObject.activeSelf);
    }
}