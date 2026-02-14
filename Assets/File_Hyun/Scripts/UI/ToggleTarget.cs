using UnityEngine;

public class ToggleTarget : MonoBehaviour
{
    public void TurnOn() => gameObject.SetActive(true);

    public void TurnOff() => gameObject.SetActive(false);
}