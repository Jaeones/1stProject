using UnityEngine;
using UnityEngine.AI;

public class PetAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent; // 속도를 가져올 NavMeshAgent

    // 성능 최적화를 위한 해시값 저장
    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    private void Awake()
    {
        // 만약 인스펙터에서 연결을 깜빡했다면 자동으로 찾아서 연결 시도
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    private void LateUpdate()
    {
        if (animator == null || agent == null) return;

        // NavMeshAgent의 현재 이동 속도를 가져옴
        float currentSpeed = agent.velocity.magnitude;

        // 애니메이터의 Speed 파라미터에 전달 (DampTime을 0.1f 주어 부드럽게 변하도록 함)
        animator.SetFloat(SpeedHash, currentSpeed, 0.1f, Time.deltaTime);
    }
}