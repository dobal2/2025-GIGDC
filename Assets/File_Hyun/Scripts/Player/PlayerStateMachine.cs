using System.Diagnostics;

public class PlayerStateMachine
{
    private PlayerState currentState;
    private PlayerState previousState;

    public string CurrentStateName => currentState?.Name ?? "None";

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
        //UnityEngine.Debug.Log($"Current State: {currentState?.Name}");
        currentState?.Update();
    }

    public void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }
}