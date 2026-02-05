using UnityEngine;
using UnityEngine.Rendering.Universal;

public interface ITutorialStep
{
    public void Enter();
    public void Update();
    public void Exit();
}

public abstract class TutorialStepBase : ITutorialStep
{
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
