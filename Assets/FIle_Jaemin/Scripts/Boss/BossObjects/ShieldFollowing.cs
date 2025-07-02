using UnityEngine;

public class ShieldFollowing : MonoBehaviour
{
    private GameObject player;
    void Start()
    {
        Destroy(gameObject,3);
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("No Player");
            return;
        }
        
    }
    
    void Update()
    {
        if (player != null)
        {
            transform.position = player.transform.position + new Vector3(0,player.transform.localScale.y/2,0);
        }
    }
}
