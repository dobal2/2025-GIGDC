using UnityEngine;

public class LobbyPlayerController : MonoBehaviour
{
    public static LobbyPlayerController Instance { get; private set; }

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
}