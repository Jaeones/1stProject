using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class PetController : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Follow Settings")]
    [Tooltip("이 거리보다 멀어지면 펫이 근처로 텔레포트")]
    [SerializeField] private float teleportDistance = 10f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = 5f;
        agent.acceleration = 10f;
    }

    public void SetTargetPosition(Vector3 targetPos)
    {
        if (agent == null) return;

        // 현재 위치와 목표 위치 간의 거리 계산
        float dist = Vector3.Distance(transform.position, targetPos);

        // 거리가 너무 멀어지면 텔레포트
        if (dist > teleportDistance)
        {
            //텔레포트
            agent.Warp(targetPos);
        }
        else
        {
            //네비게이션 이동
            agent.SetDestination(targetPos);
        }
    }

}
