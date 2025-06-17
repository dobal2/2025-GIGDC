using UnityEngine;

public class SkillCooldownGauge : MonoBehaviour
{
    public SpriteRenderer cooldownRenderer;
    public Sprite[] cooldownSprites;

    public Transform player;

    public float baseAcceleration = 500f;
    public float damping = 0.9f;
    public float verticalOffset = 2.2f;
    public float horizontalOffset = 0f;
    public float fadeSpeed = 10f;

    private int displayedIndex = -1;
    private float targetAlpha = 0f;
    private Vector2 velocity;

    void Start()
    {
        cooldownRenderer.sortingOrder = 5;
        SetAlpha(0f);
    }

    void LateUpdate()
    {
        if (!player || PlayerController.Instance?.AttackController == null)
            return;

        float progress = Mathf.Clamp01(PlayerController.Instance.AttackController.SkillCooldownProgress);
        int spriteIndex = Mathf.Clamp(Mathf.FloorToInt(progress * 9f), 0, 8);

        if (spriteIndex != displayedIndex)
            UpdateCooldown(spriteIndex);

        if (progress < 1f)
            Show();
        else
            Hide();

        Vector2 targetPos = GetTargetWorldPosition();
        Vector2 currentPos = transform.position;
        Vector2 direction = targetPos - currentPos;
        float distance = direction.magnitude;

        if (distance > 0.01f)
        {
            Vector2 acceleration = baseAcceleration * distance * direction.normalized;
            velocity += acceleration * Time.deltaTime;
        }

        currentPos += velocity * Time.deltaTime;
        velocity *= damping;

        transform.position = new Vector3(currentPos.x, currentPos.y, transform.position.z);

        float currentAlpha = cooldownRenderer.color.a;
        float newAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);
        SetAlpha(newAlpha);
    }

    Vector2 GetTargetWorldPosition()
    {
        Vector2 offsetDir = Vector2.left * PlayerController.Instance.facingDirection;
        return (Vector2)player.position + offsetDir * horizontalOffset + Vector2.up * verticalOffset;
    }

    void UpdateCooldown(int index)
    {
        displayedIndex = index;
        cooldownRenderer.sprite = cooldownSprites[displayedIndex];
    }

    void Show()
    {
        if (targetAlpha < 1f)
        {
            targetAlpha = 1f;
            Vector2 pos = GetTargetWorldPosition();
            transform.position = new Vector3(pos.x, pos.y, transform.position.z);
            velocity = Vector2.zero;
        }
    }

    void Hide()
    {
        targetAlpha = 0f;
    }

    void SetAlpha(float a)
    {
        Color c = cooldownRenderer.color;
        c.a = a;
        cooldownRenderer.color = c;
    }
}