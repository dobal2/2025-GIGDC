using System;
using UnityEngine;

public class WaterTriggerHandler : MonoBehaviour
{
    [SerializeField] private LayerMask _waterMask;
    //[SerializeField] private GameObject _Particles;

    private EdgeCollider2D _edgeColl;

    private InteractableWater _water;
    
    private void Awake()
    {
        _edgeColl = GetComponent<EdgeCollider2D>();
        _water = GetComponent<InteractableWater>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((_waterMask.value & 1 << collision.gameObject.layer) > 0)
        {
            Rigidbody2D rb = collision.GetComponentInParent<Rigidbody2D>();

            if (rb != null)
            {
                // Vector2 localpos = gameObject.transform.localPosition;
                // Vector2 hitObjectPos = collision.transform.position;
                // Bounds hitObjectBounds = collision.bounds;
                //
                // Vector3 spawnPos = Vector3.zero;
                // if (collision.transform.position.y >= _edgeColl.points[1].y + _edgeColl.offset.y + localpos.y)
                // {
                //     //hit from above
                //     spawnPos = hitObjectPos - new Vector2(0f, hitObjectBounds.extents.y);
                // }
                // else
                // {
                //     //hit from below
                //     spawnPos = hitObjectPos + new Vector2(0f, hitObjectBounds.extents.y);
                // }
                
                //clamp splash point to a MAX velocity

                int multiplier = -1;
                if (rb.linearVelocity.y < 0)
                {
                    multiplier = 1;
                }
                
                float vel =2f *rb.linearVelocity.y * _water.ForceMultiplier;
                
                vel = Mathf.Clamp(Mathf.Abs(vel), 0f, _water.MaxForce);
                vel *= multiplier;
                
                _water.Splash(collision, vel);
            }

            
        }
    }
}
