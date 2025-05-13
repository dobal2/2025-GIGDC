using UnityEngine;
using UnityEngine.UI;

public class GlitchtTester : MonoBehaviour
{
    
    [SerializeField] private Graphic targetGraphic;
    
    void Awake()
    {
        targetGraphic = GetComponent<Graphic>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) {
            GlitchEffecter.Active(targetGraphic);
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            GlitchEffecter.Deactivate(targetGraphic);
        }
    }
}
