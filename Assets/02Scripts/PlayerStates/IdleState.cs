using UnityEngine;

public class IdleState : BaseState
{
    public IdleState(PlayerController controller, StateMachine stateMachine) : base(controller, stateMachine) { }

    public override void Enter()
    {
        controller.Movement.Move(Vector2.zero, 0f);
    }

    public override void LogicUpdate()
    {
        //1. 점프 입력 체크
        if (controller.IsJumpTriggered)
        {
            if (controller.Movement.IsGrounded())
            {
                stateMachine.ChangeState(controller.JumpState);
            }
            else
            {
                controller.IsJumpTriggered = false;
            }
            return;
        }

        //2. 이동 입력 체크
        if (controller.currentInput.sqrMagnitude > 0.01f)
        {
            stateMachine.ChangeState(controller.MoveState);
            return;
        }
    }
}
