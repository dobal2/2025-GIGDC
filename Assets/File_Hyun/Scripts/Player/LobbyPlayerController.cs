using System.Collections.Generic;
using UnityEngine;

public class LobbyPlayerController : MonoBehaviour
{
    public LobbyPlayerController Instance { get; private set; }

    public enum LobbyplayerState
    {
        one,
        two,
        three,
        four
    }

    public LobbyplayerState CurrentLobbyPlayer = LobbyplayerState.one;

    [HideInInspector] public Animator Animator;
    [HideInInspector] public Rigidbody2D rb;
    public float speed = 5f;

    [HideInInspector] public int facingDirection = 1;
    public float MoveInput { get; set; }

    [Header("Interaction Settings")]
    public Collider2D interactionTrigger;
    public string interactionTag = "Interactable";

    private readonly List<Collider2D> nearbyInteractables = new();
    private Collider2D currentTarget;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        SetLobby();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(MoveInput * speed, rb.linearVelocity.y);
        if (MoveInput != 0)
        {
            facingDirection = MoveInput > 0 ? 1 : -1;
            transform.rotation = Quaternion.Euler(0f, facingDirection == -1 ? 180f : 0f, 0f);
            UpdateClosestInteractable();
        }
    }

    private void Update()
    {
        PlayAnim();
    }

    public void SetLobby()
    {
        InputManager.Instance.RegisterLobby(this);
        InputManager.Instance.currentContext = InputManager.InputContext.Lobby;
    }

    private void PlayAnim()
    {
        int stateNumber = (int)CurrentLobbyPlayer + 1;
        string animPrefix = "Byeongtae_";
        string animType = MoveInput == 0 ? "Idle" : "Walk";
        string animName = $"{animPrefix}{animType}_{stateNumber}";

        Animator.Play(animName);
    }

    private void UpdateClosestInteractable()
    {
        if (nearbyInteractables.Count == 0)
        {
            if (currentTarget != null)
            {
                HandleHoverExit(currentTarget);
                currentTarget = null;
            }
            return;
        }

        Collider2D closest = null;
        float minDist = float.MaxValue;

        foreach (var col in nearbyInteractables)
        {
            float dist = Vector2.Distance(transform.position, col.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = col;
            }
        }

        if (closest != currentTarget)
        {
            if (currentTarget != null)
                HandleHoverExit(currentTarget);

            currentTarget = closest;

            if (currentTarget != null)
                HandleHoverEnter(currentTarget);
        }
    }

    public void TryInteract()
    {
        if (currentTarget != null)
            HandleInteract(currentTarget);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == interactionTrigger)
            return;

        if (!other.CompareTag(interactionTag))
            return;

        if (IsInInteractionTrigger(other))
        {
            nearbyInteractables.Add(other);
            UpdateClosestInteractable();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other == interactionTrigger)
            return;

        if (nearbyInteractables.Remove(other))
        {
            if (other == currentTarget)
            {
                HandleHoverExit(currentTarget);
                currentTarget = null;
            }
            UpdateClosestInteractable();
        }
    }

    private bool IsInInteractionTrigger(Collider2D other)
    {
        return interactionTrigger.bounds.Intersects(other.bounds);
    }

    private void HandleHoverEnter(Collider2D target)
    {
        // 가장 가까운 타겟이 들어옴(바뀜, 호출)
        Debug.Log($"[HoverEnter] Hovered over {target.name}");
    }

    private void HandleHoverExit(Collider2D target)
    {
        // 가장 가까운 타겟이 나감(바뀜, 호출)
        Debug.Log($"[HoverExit] Exited hover from {target.name}");
    }

    private void HandleInteract(Collider2D target)
    {
        // 실제 상호작용(호출)
        Debug.Log($"[Interact] Interacted with {target.name}");
    }
}