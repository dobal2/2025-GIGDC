using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class ShieldFollowing : MonoBehaviour
{
    [SerializeField] private GameObject shieldBrokeEffect;
    private GameObject player;
    
    void Start()
    {
        StartCoroutine(Break(3));
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("No Player");
            return;
        }
        
    }

    IEnumerator Break(float delay)
    {
        yield return new WaitForSeconds(delay);
        VisualEffect newShieldBroke = Instantiate(shieldBrokeEffect, transform.position, Quaternion.identity).GetComponent<VisualEffect>();
        newShieldBroke.Play();
        Destroy(gameObject);
        Destroy(newShieldBroke,2);
    }
    
    void Update()
    {
        if (player != null)
        {
            transform.position = player.transform.position + new Vector3(0,player.transform.localScale.y/2,0);
        }
    }
}
