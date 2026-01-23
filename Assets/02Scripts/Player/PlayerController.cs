using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private InputReader inputReader;
    [SerializeField]
    private PlayerStatsSO stats;
    [SerializeField]
    private StateMachine stateMachine;
    // 외부에서 stats에 접근할 수 있도록 프로퍼티 제공
    public PlayerStatsSO Stats => stats;
    public PlayerMovement Movement { get; private set; }
    public Vector2 currentInput { get; private set; }
    public bool isRunning { get; private set; }
    public bool IsJumpTriggered { get; set; }

    // 상태 인스턴스들
    public IdleState IdleState { get; private set; }
    public MoveState MoveState { get; private set; }
    public JumpState JumpState { get; private set; }

    private void Awake()
    {
        Movement = GetComponent<PlayerMovement>();
        // 상태 인스턴스 생성
        IdleState = new IdleState(this, stateMachine);
        MoveState = new MoveState(this, stateMachine);
        JumpState = new JumpState(this, stateMachine);

    }

    private void Start()
    {
        // 데이터 파일 설정값 적용 (레이어 'Ground' 설정)
        Movement.SetGroundCheckSettings(stats.GroundCheckDistance, LayerMask.GetMask("Ground"));

        // 초기 상태 설정
        stateMachine.Initialize(IdleState);
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

    // 입력 이벤트 핸들러
    void OnMove(Vector2 input)
    {
        currentInput = input;
    }

    private void OnJump()
    {
        Movement.Jump(stats.JumpForce);
    }

    private void OnRun(bool runState)
    {
        isRunning = runState;
    }
}
