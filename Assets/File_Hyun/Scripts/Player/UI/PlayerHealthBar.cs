using UnityEngine;
using TMPro;

public class PlayerHealthBar : MonoBehaviour
{
    public SpriteRenderer heartRenderer;
    public Sprite[] heartSprites;
    public TextMeshPro healthText;
    public Transform player;

    public float baseAcceleration = 300f;
    public float damping = 0.91f;
    public float verticalOffset = 1.5f;
    public float horizontalOffset = 1.2f;
    public float fadeSpeed = 10f;

    private int displayedHealth = 5;
    private float targetAlpha = 0f;
    private bool isVisible = false;

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
            Vector2 acceleration = direction.normalized * baseAcceleration * distance;
            velocity += acceleration * Time.deltaTime;
        }

        currentPos += velocity * Time.deltaTime;
        velocity *= damping;

        transform.position = new Vector3(currentPos.x, currentPos.y, transform.position.z);

        float currentAlpha = heartRenderer.color.a;
        float newAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);
        SetAlpha(newAlpha);

        int currentHealth = Mathf.Clamp(Mathf.CeilToInt(PlayerHealth.Instance.Currenthealth), 0, 5);
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
        isVisible = true;

        Vector2 pos = GetTargetWorldPosition();
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        velocity = Vector2.zero;
    }

    private void Hide()
    {
        targetAlpha = 0f;
        isVisible = false;
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