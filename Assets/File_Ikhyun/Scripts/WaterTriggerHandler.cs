using System;
using UnityEngine;

public class WaterTriggerHandler : MonoBehaviour
{
    [Tooltip("플레이어 레이어만 상호작용")]
    [SerializeField] private string _playerLayerName = "Player";
    
    private EdgeCollider2D _edgeColl;
    private InteractableWater _water;
    private int _playerLayer;

    private void Awake()
    {
        _edgeColl = GetComponent<EdgeCollider2D>();
        _water = GetComponent<InteractableWater>();
        _playerLayer = LayerMask.NameToLayer(_playerLayerName);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어 레이어만 허용
        if (collision.gameObject.layer != _playerLayer)
            return;

        Rigidbody2D rb = collision.GetComponentInParent<Rigidbody2D>();
        if (rb == null)
            return;

        Vector2 velocity = rb.linearVelocity;
        float combinedForce = 0f;

        // 방향에 따라 multiplier 결정
        int verticalDir = velocity.y < 0 ? 1 : -1;
        int horizontalDir = velocity.x < 0 ? -1 : 1;

        // Y축 기반 스플래시
        float yForce = 2f * velocity.y * _water.ForceMultiplier;
        yForce = Mathf.Clamp(Mathf.Abs(yForce), 0f, _water.MaxForce) * verticalDir;

        // X축 기반 스플래시 추가 (보조적인 영향)
        float xForce = velocity.x * 0.5f * _water.ForceMultiplier;
        xForce = Mathf.Clamp(xForce, -_water.MaxForce * 0.5f, _water.MaxForce * 0.5f);

        // 전체 힘은 두 방향의 조합 (혹은 두 힘을 따로 써도 됨)
        combinedForce = yForce + xForce;

        _water.Splash(collision, combinedForce);
    }
}