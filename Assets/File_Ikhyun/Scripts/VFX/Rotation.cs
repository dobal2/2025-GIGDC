using UnityEngine;
using UnityEngine.VFX;

public class Rotation : MonoBehaviour {
    public VisualEffect vfx;

    
    void Update()
    {
        if (Input.GetKey(KeyCode.E)) {
            transform.Rotate(0, 0, 2);
        }
        else if (Input.GetKey(KeyCode.Q)) {
            transform.Rotate(0, 0, -2);
        }
        
        else if (Input.GetKeyDown(KeyCode.F1)) {
            vfx.Play();
        }
    }
}
