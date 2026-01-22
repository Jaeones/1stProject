using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private InputReader inputReader;
    [SerializeField]
    private PlayerStatsSO stats;

    private PlayerMovement movement;
    private Transform cameraTransform;

    private Vector2 currentInput;
    private bool isRunning;


    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogError("Main Camera not found in the scene.");
        }
        // 데이터 파일 설정값 적용 (레이어 'Ground' 설정)
        movement.SetGroundCheckSettings(stats.GroundCheckDistance, LayerMask.GetMask("Ground"));
    }

    private void OnEnable()
    {
        inputReader.MoveEvent += OnMove;
        inputReader.JumpEvent += OnJump;
        inputReader.RunEvent += OnRun;
    }

    private void OnDisable()
    {
        inputReader.MoveEvent -= OnMove;
        inputReader.JumpEvent -= OnJump;
        inputReader.RunEvent -= OnRun;
    }

    private void Update()
    {
        Vector2 moveInput = Vector2.zero;

        if (cameraTransform != null && currentInput.sqrMagnitude > 0.01f)
        {
            // 카메라의 전방과 우측 벡터 계산
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            // 입력 벡터를 카메라 기준으로 변환
            Vector3 moveDirection = camForward * currentInput.y + camRight * currentInput.x;
            moveInput = new Vector2(moveDirection.x, moveDirection.z);
        }

        float targetSpeed = isRunning ? stats.RunSpeed : stats.WalkSpeed;
        movement.Move(moveInput, targetSpeed, stats.RotationSpeed);
    }

    // 입력 이벤트 핸들러
    void OnMove(Vector2 input)
    {
        currentInput = input;
    }

    private void OnJump()
    {
        movement.Jump(stats.JumpForce);
    }

    private void OnRun(bool runState)
    {
        isRunning = runState;
    }
}
