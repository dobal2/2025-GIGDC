using System;
using UnityEngine;

public class Test_Move : MonoBehaviour
{
    
    private void Update() {
        float d = Input.GetAxisRaw("Horizontal");
        float speed = 5;
        
        transform.Translate(d/ 4, 0, 0);
    }

    // private void FixedUpdate() {
    //     VFXManager.Spawn("InkTrail", this.transform.position);
    // }
}
