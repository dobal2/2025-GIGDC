using UnityEngine;

public class SuperFuckingNuclearBomb : MonoBehaviour
{
    public static SuperFuckingNuclearBomb Instance { get; private set; }

    public bool IsActive = true;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if(!IsActive) return;

        GameObject[] all = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject fuck in all)
        {
            fuck.transform.Rotate(new Vector3(1, 1, 1));
        }
    }
}