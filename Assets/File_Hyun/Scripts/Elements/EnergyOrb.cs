using UnityEngine;

public class EnergyOrb : MonoBehaviour
{
    [SerializeField] private float moveDurationMin = 0.35f;
    [SerializeField] private float moveDurationMax = 0.7f;
    [SerializeField] private float arcHeightMin = 0.4f;
    [SerializeField] private float arcHeightMax = 1.4f;
    [SerializeField] private float lateralScatterMin = 0.25f;
    [SerializeField] private float lateralScatterMax = 1.2f;
    [SerializeField] private float collectDistance = 0.2f;
    [SerializeField] private Vector3 targetOffset = new(0f, 0.6f, 0f);
    [SerializeField] private int restoreAmount = 1;

    private PlayerController target;
    private Vector3 startPosition;
    private Vector3 lateralOffset;
    private float arcHeight;
    private float moveDuration;
    private float elapsed;
    private bool initialized;

    public void Initialize(PlayerController targetPlayer)
    {
        target = targetPlayer;
        startPosition = transform.position;
        elapsed = 0f;
        moveDuration = Random.Range(moveDurationMin, moveDurationMax);
        arcHeight = Random.Range(arcHeightMin, arcHeightMax);
        float scatterDistance = Random.Range(lateralScatterMin, lateralScatterMax);
        Vector2 scatterDirection = Random.insideUnitCircle.normalized;
        if (scatterDirection == Vector2.zero)
            scatterDirection = Vector2.up;
        lateralOffset = new Vector3(scatterDirection.x, scatterDirection.y, 0f) * scatterDistance;
        initialized = true;
    }

    public void Update()
    {
        if (!initialized)
            return;

        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        elapsed += Time.deltaTime;
        float t = moveDuration <= 0f ? 1f : Mathf.Clamp01(elapsed / moveDuration);
        float easedT = 1f - Mathf.Pow(1f - t, 3f);

        Vector3 targetPosition = target.transform.position + targetOffset;
        Vector3 basePosition = Vector3.Lerp(startPosition, targetPosition, easedT);
        Vector3 scatter = lateralOffset * (1f - easedT);
        float arc = Mathf.Sin(easedT * Mathf.PI) * arcHeight;
        transform.position = basePosition + scatter + Vector3.up * arc;

        if ((transform.position - targetPosition).sqrMagnitude <= collectDistance * collectDistance)
        {
            target.RestoreEnergy(restoreAmount);
            Destroy(gameObject);
        }
    }
}