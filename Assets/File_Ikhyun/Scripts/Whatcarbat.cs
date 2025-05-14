using UnityEngine;

public class GlitchTester : MonoBehaviour
{
    [SerializeField] private GlitchController glitchController;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            glitchController.SetGlitchIntensity(1.0f); // 강한 지직거림
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            glitchController.SetGlitchIntensity(0.3f); // 약한 지직거림
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            glitchController.SetGlitchIntensity(0.0f); // 해제
        }
    }
}