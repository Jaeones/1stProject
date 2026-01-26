using UnityEngine;

public class JumpState : BaseState
{
    private float jumpStartTime;

    public JumpState(PlayerController controller, StateMachine stateMachine) : base(controller, stateMachine) { }

    public override void Enter()
    {
        controller.Movement.Jump(stats.JumpForce);
        controller.IsJumpTriggered = false;
        controller.CallOnJumpEvent();
        jumpStartTime = Time.time;
    }

    public override void LogicUpdate()
    {
        // 공중 제어 약간의 관성과 약간의 방향 전환 허용
        MoveState moveState = (MoveState)controller.MoveState;

        // 착지 체크 점프직후 바로 착지 판정 방지
        if (Time.time > jumpStartTime + 0.1f)
        {
            if (controller.Movement.IsGrounded())
            {
                if(controller.currentInput.sqrMagnitude > 0.01f)
                    stateMachine.ChangeState(controller.MoveState);
                else
                    stateMachine.ChangeState(controller.IdleState);
            }
        }
    }
}
