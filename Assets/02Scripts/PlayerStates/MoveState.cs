using UnityEngine;

public class MoveState : BaseState
{
    public MoveState(PlayerController controller, StateMachine stateMachine) : base(controller, stateMachine) { }

    public override void LogicUpdate()
    {
        //1. 점프 입력 체크
        if (controller.IsJumpTriggered)
        {
            stateMachine.ChangeState(controller.JumpState);
            return;
        }
        else
        {
            controller.IsJumpTriggered = false;
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
        Transform cameraTransform = Camera.main ? Camera.main.transform : null;
        Vector2 moveInput = Vector2.zero;

        if (cameraTransform != null)
        {
            //카메라의 전방/우측 벡터 계산
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;
            cameraForward.y = 0f;
            cameraForward.Normalize();
            cameraRight.y = 0f;
            cameraRight.Normalize();

            //이동 입력을 카메라 기준 방향으로 변환
            Vector3 targetDir = cameraForward * controller.currentInput.y + cameraRight * controller.currentInput.x;
            moveInput = new Vector2(targetDir.x, targetDir.z);
        }
        else
        {
            moveInput = controller.currentInput;
        }

        //달리기 여부에 따른 속도 결정
        float currentSpeed = controller.isRunning ? stats.RunSpeed : stats.WalkSpeed;

        controller.Movement.Move(moveInput, currentSpeed, stats.RotationSpeed);
    }
}
