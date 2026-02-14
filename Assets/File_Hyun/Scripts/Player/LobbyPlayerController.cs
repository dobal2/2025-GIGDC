using System.Collections;
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

    private bool autoAnimLocked;
    private int overrideToken;
    private int lastAutoStateHash;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        Animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        SetLobby();
        CurrentLobbyPlayer = LobbyplayerState.one;

        switch (Stage.Data)
        {
            case StageDataType.Stage1:
                CurrentLobbyPlayer = LobbyplayerState.two;
                break;
            case StageDataType.Stage2:
                CurrentLobbyPlayer = LobbyplayerState.three;
                break;
            case StageDataType.Stage3:
                CurrentLobbyPlayer = LobbyplayerState.four;
                break;
        }
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
        if (!autoAnimLocked)
            PlayAnim();
    }

    public void SetLobby()
    {
        InputManager.Instance.RegisterLobby(this);
        InputManager.Instance.CurrentContext = InputManager.InputContext.Lobby;
    }

    private void PlayAnim()
    {
        int stateNumber = (int)CurrentLobbyPlayer + 1;
        string animPrefix = "Byeongtae_";
        string animType = MoveInput == 0 ? "Idle" : "Walk";
        string animName = $"{animPrefix}{animType}_{stateNumber}";

        int hash = Animator.StringToHash(animName);

        if (hash == lastAutoStateHash)
            return;

        lastAutoStateHash = hash;
        Animator.Play(hash);
    }

    private void PlayOverrideAnim(string stateName)
    {
        autoAnimLocked = true;
        overrideToken++;

        int token = overrideToken;
        Animator.Play(stateName);

        StartCoroutine(WaitOverrideEnd(stateName, token));
    }

    private IEnumerator WaitOverrideEnd(string stateName, int token)
    {
        while (token == overrideToken && !Animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            yield return null;

        while (token == overrideToken)
        {
            if (!Animator.IsInTransition(0))
            {
                AnimatorStateInfo info = Animator.GetCurrentAnimatorStateInfo(0);

                if (info.IsName(stateName) && info.normalizedTime >= 1f)
                    break;
            }

            yield return null;
        }

        if (token != overrideToken)
            yield break;

        autoAnimLocked = false;
        lastAutoStateHash = 0;
    }

    #region PlayerDialogEvent
    public void Byeongtae1_AwakeUp() => PlayOverrideAnim("Byeongtae1_AwakeUp");
    public void Byeongtae1_Lay() => PlayOverrideAnim("Byeongtae1_Lay");
    public void Byeongtae1_TurnHead() => PlayOverrideAnim("Byeongtae1_TurnHead");
    public void Byeongtae2_Shrink() => PlayOverrideAnim("Byeongtae2_Shrink");
    public void Byeongtae4_Walk() => PlayOverrideAnim("Byeongtae4_Walk");
    #endregion
}