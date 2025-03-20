using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;

public class TestForTest : MonoBehaviour
{
    public PostProcessVolume volume;
    private ColorGrading colorGrading;
    private float defaultExposure;
    private float defaultSaturation;

    void Start()
    {
        // Color Grading 설정 가져오기
        volume.profile.TryGetSettings(out colorGrading);
        
        // 기본 값 저장
        defaultExposure = colorGrading.postExposure.value;
        defaultSaturation = colorGrading.saturation.value;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        StartCoroutine(DamageEffectRoutine());
    }

    IEnumerator DamageEffectRoutine()
    {
        // 피격 시 어둡고 색이 바래지게 설정
        colorGrading.postExposure.value = -2f;  // 화면 어둡게
        colorGrading.saturation.value = -50f;   // 색 바래짐
        
        yield return new WaitForSeconds(0.5f);

        // 원래 값으로 복구
        colorGrading.postExposure.value = defaultExposure;
        colorGrading.saturation.value = defaultSaturation;
    }
}
