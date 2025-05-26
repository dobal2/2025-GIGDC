using System;
using System.Collections;
using UnityEngine;

public class PlayerGhostTrail : MonoBehaviour
{
    public GameObject ghostPrefab;
    public float ghostSpawnInterval = 0.01f;
    public KeyCode ghostKey = KeyCode.LeftShift;

    private SpriteRenderer spriteRenderer;
    private Coroutine ghostRoutine;
    

    
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void Start()
    {
        PlayerController.Instance.OnEffectStateChanged += HandleEffectChange;
        
    }

    void HandleEffectChange(PlayerController.PlayerEffectState state)
    {
        switch (state)
        {
            case PlayerController.PlayerEffectState.Afterimage:
                if (ghostRoutine == null)
                    ghostRoutine = StartCoroutine(SpawnGhostsRoutine());
                break;

            case PlayerController.PlayerEffectState.GroundWalkDust:
                // 필요시 다른 이펙트 실행
                StopGhostRoutine();
                break;

            case PlayerController.PlayerEffectState.None:
                StopGhostRoutine();
                break;

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

            // 속도가 너무 느릴 경우 잔상 생략 (ex: 0.1 이하)
            if (speed < 0.1f)
            {
                yield return null;
                continue;
            }

            // 속도가 빠를수록 spawn 확률 증가
            float spawnProbability = Mathf.Clamp01(speed / 10f); // 속도 0~10 기준 정규화

            // 최소 간격 지나고, 확률 통과 시 생성
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
    
}
