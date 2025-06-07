using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerGhostTrail : MonoBehaviour
{
    public GameObject ghostPrefab;

    [SerializeField] private SpriteRenderer spriteRenderer;
    private Coroutine ghostRoutine;

    [SerializeField] private ParticleSystem spearSkill;
    [SerializeField] private ParticleSystem dash;

    [Header("Post Processing")]
    [SerializeField] private Volume postProcessingVolume;
    private Bloom bloom;
    private Coroutine bloomRoutine;

    private void Awake()
    {
        if (postProcessingVolume.profile.TryGet(out bloom))
        {
            // you can repair volume data here.
        }
    }

    void Start()
    {
        PlayerController.Instance.OnEffectStateChanged += HandleEffectChange;
    }

    void HandleEffectChange(PlayerController.PlayerEffectState state)
    {
        switch (state)
        {
            case PlayerController.PlayerEffectState.Dash:
                dash.Play();
                StopGhostRoutine();
                break;

            case PlayerController.PlayerEffectState.FastFall:
                if (ghostRoutine == null)
                    ghostRoutine = StartCoroutine(SpawnGhostsRoutine());
                break;

            case PlayerController.PlayerEffectState.SpearAirSkill:
                spearSkill.Play();
                TriggerFlashEffect();
                break;

            case PlayerController.PlayerEffectState.GroundWalkDust:
            case PlayerController.PlayerEffectState.None:
            default:
                StopGhostRoutine();
                break;
        }
    }

    private void StopGhostRoutine()
    {
        if (ghostRoutine != null)
        {
            StopCoroutine(ghostRoutine);
            ghostRoutine = null;
        }
    }

    void OnDestroy()
    {
        if (PlayerController.Instance != null)
            PlayerController.Instance.OnEffectStateChanged -= HandleEffectChange;
    }

    IEnumerator SpawnGhostsRoutine()
    {
        float baseInterval = 0.02f;
        float lastSpawnTime = 0f;

        while (true)
        {
            float speed = PlayerController.Instance.CurrentVelocity.magnitude;
            if (speed < 0.1f) { yield return null; continue; }

            float spawnProbability = Mathf.Clamp01(speed / 10f);
            if (Time.time - lastSpawnTime > baseInterval && UnityEngine.Random.value < spawnProbability)
            {
                SpawnGhost();
                lastSpawnTime = Time.time;
            }

            yield return null;
        }
    }

    private void SpawnGhost()
    {
        GameObject ghost = Instantiate(ghostPrefab);
        ghost.GetComponent<Ghost>().SetGhost(
            spriteRenderer.sprite,
            transform.position,
            transform.rotation,
            transform.localScale,
            new Color(1f, 1f, 1f, 0.2f),
            spriteRenderer.flipX
        );
    }
    
    private void TriggerFlashEffect()
    {
        if (bloomRoutine != null)
            StopCoroutine(bloomRoutine);
        bloomRoutine = StartCoroutine(BloomFlash());
    }

    IEnumerator BloomFlash()
    {
        if (!postProcessingVolume.profile.TryGet(out bloom))
            yield break;

        float originalIntensity = bloom.intensity.value;

        
        bloom.intensity.value = 3f;

        
        yield return new WaitForSeconds(0.05f);

        
        float duration = 0.4f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            
            bloom.intensity.value = Mathf.Lerp(3f, originalIntensity, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        bloom.intensity.value = originalIntensity;
    }

}
