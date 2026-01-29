using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private PlayerController controller;

    // 성능 최적화를 위해 문자열을 미리 해시값으로 변환
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");

    private void Start()
    {
        // 컨트롤러의 점프 이벤트 구독
        if (controller != null)
        {
            controller.OnJumpPerformed += OnJump;
        }
    }
    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (controller != null)
        {
            controller.OnJumpPerformed -= OnJump;
        }
    }

    private void LateUpdate()
    {
        if(animator == null || controller == null) return;
        //1. 속도 파라미터 업데이트
        float currentInputMagnitude = controller.currentInput.magnitude;

        //만약 달리고 있다면 값을 2로
        if (controller.isRunning && currentInputMagnitude > 0.1f)
        {
            currentInputMagnitude = 2f;
        }

        animator.SetFloat(SpeedHash, currentInputMagnitude, 0.1f, Time.deltaTime);

        //2. 착지 여부 파라미터 업데이트
        bool isGrounded = controller.Movement.IsGrounded();
        animator.SetBool(IsGroundedHash, isGrounded);
    }

    private void OnJump()
    {
        animator.SetTrigger(JumpHash);
    }
}

