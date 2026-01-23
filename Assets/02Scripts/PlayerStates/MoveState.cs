using UnityEngine;

public class MoveState : BaseState
{
    public MoveState(PlayerController controller, StateMachine stateMachine) : base(controller, stateMachine) { }

    public override void LogicUpdate()
    {
        if (controller.IsJumpTriggered)
        {
            //1. 점프 입력 체크
            if (controller.Movement.IsGrounded())
            {
                stateMachine.ChangeState(controller.JumpState);
            }
            else
            {
                controller.IsJumpTriggered = false;
            }

            if (controller.Movement.IsGrounded()) return;
        }

        

        //2. 이동 입력 체크
        if (controller.currentInput.sqrMagnitude < 0.01f)
        {
            stateMachine.ChangeState(controller.IdleState);
            return;
        }

        //3. 이동 처리(카메라 기준 방향 계산)
        CalculateMove();
    }

    void CalculateMove()
    {
        Vector3 moveDir = 
            controller.transform.forward * controller.currentInput.y +
            controller.transform.right * controller.currentInput.x;

        //대각선 이동 보정
        if(moveDir.sqrMagnitude > 1f)
        {
            moveDir.Normalize();
        }
        float currentSpeed = controller.isRunning ? controller.Stats.RunSpeed : controller.Stats.WalkSpeed;
        controller.Movement.Move(moveDir, currentSpeed);
    }
}
