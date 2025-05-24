public class PlayerStateMachine
{
    private PlayerState currentState;
    private PlayerState previousState;

    public PlayerStateType CurrentStateType => currentState?.StateType ?? PlayerStateType.Missing;

    public void Initialize(PlayerState startingState)
    {
        currentState = startingState;
        currentState.Enter();
    }

    public void ChangeState(PlayerState newState)
    {
        if (currentState != null && !currentState.CanTransitionTo(newState))
            return;

        currentState?.Exit();
        previousState = currentState;
        currentState = newState;
        currentState.Enter();
    }

    public void Update()
    {
        //UnityEngine.Debug.Log($"Current State: {CurrentStateType}");
        currentState?.Update();
    }

    public void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }
}