using UnityEngine;
using TMPro;

public class PlayerHealthBar : MonoBehaviour
{
    public SpriteRenderer heartRenderer;
    public Sprite[] heartSprites;
    public TextMeshPro healthText;
    public Transform player;

    public float baseAcceleration = 500f;
    public float damping = 0.9f;
    public float verticalOffset = 2.6f;
    public float horizontalOffset = 0f;
    public float fadeSpeed = 10f;

    private int displayedHealth = 5;
    private float targetAlpha = 0f;

    private Vector2 velocity;

    void Start()
    {
        healthText.transform.localPosition = new Vector3(0f, 0.7f, 0f);

        heartRenderer.sortingOrder = 6;
        healthText.GetComponent<MeshRenderer>().sortingOrder = 7;

        SetAlpha(0f);
    }

    void LateUpdate()
    {
        if (!player) return;

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

        float currentAlpha = heartRenderer.color.a;
        float newAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);
        SetAlpha(newAlpha);

        int currentHealth = Mathf.Clamp(Mathf.CeilToInt(PlayerHealth.CurrentHealth), 0, 5);
        if (currentHealth != displayedHealth)
            UpdateHealth(currentHealth);
    }

    Vector2 GetTargetWorldPosition()
    {
        Vector2 offsetDir = Vector2.left * PlayerController.Instance.facingDirection;
        return (Vector2)player.position + offsetDir * horizontalOffset + Vector2.up * verticalOffset;
    }

    public void UpdateHealth(int newHealth)
    {
        displayedHealth = newHealth;

        heartRenderer.sprite = heartSprites[displayedHealth];
        healthText.text = displayedHealth.ToString();

        if (displayedHealth < 5)
            Show();
        else
            Hide();
    }

    private void Show()
    {
        targetAlpha = 1f;

        Vector2 pos = GetTargetWorldPosition();
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        velocity = Vector2.zero;
    }

    private void Hide()
    {
        targetAlpha = 0f;
    }

    private void SetAlpha(float a)
    {
        Color hc = heartRenderer.color;
        hc.a = a;
        heartRenderer.color = hc;

        Color tc = healthText.color;
        tc.a = a;
        healthText.color = tc;
    }
}