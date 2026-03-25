using UnityEngine;

public class CounterResultTracker : MonoBehaviour
{
    public static CounterResultTracker Instance { get; private set; }
    public bool LastCounterSuccess { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnEnable()
    {
        PlayerController.Instance.OnCounterTry += OnCounterTry;
    }

    void OnDisable()
    {
        if (PlayerController.Instance != null)
            PlayerController.Instance.OnCounterTry -= OnCounterTry;
    }

    private void OnCounterTry(bool success)
    {
        LastCounterSuccess = success;
    }
}
