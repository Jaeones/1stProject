public abstract class BaseState
{
    protected PlayerController controller;
    protected StateMachine stateMachine;
    protected PlayerStatsSO stats;
    public BaseState(PlayerController controller, StateMachine stateMachine)
    {
        this.controller = controller;
        this.stateMachine = stateMachine;
        this.stats = controller.Stats;
    }

    // 상태 진입 시 1회 호출
    public virtual void Enter() { }
    public virtual void LogicUpdate() { }
    public virtual void PhysicsUpdate() { }
    public virtual void Exit() { }
}