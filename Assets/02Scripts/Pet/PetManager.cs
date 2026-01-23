using UnityEngine;

public class PetManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("따라다닐 플레이어의 Transform 연결")]
    [SerializeField]private Transform playerTransform;

    [Tooltip("왼쪽에 배치할 펫")]
    [SerializeField]private PetController leftPet;

    [Tooltip("오른쪽에 배치할 펫")]
    [SerializeField]private PetController rightPet;

    [Header("Formation Settings")]
    [Tooltip("플레이어 뒤쪽으로 얼마나 떨어질지")]
    [SerializeField]private float backDistance = 1.5f;

    [Tooltip("플레이어 좌우로 얼마나 벌릴지")]
    [SerializeField]private float sideSpacing = 1.5f;

    private void Update()
    {
        if (playerTransform == null || leftPet == null || rightPet == null) return;

        //위치 계산 로직
        //1. 기준 위치 계산(플레이어 뒤쪽, 오른쪽 방향)
        Vector3 playerBack = -playerTransform.forward;
        Vector3 playerRight = playerTransform.right;

        //2.목표 위치 계산
        // 왼쪽 펫 위치
        Vector3 leftSlotPos = playerTransform.position + playerBack * backDistance - playerRight * sideSpacing;

        // 오른쪽 펫 위치
        Vector3 rightSlotPos = playerTransform.position + playerBack * backDistance + playerRight * sideSpacing;

        //3.펫 위치 업데이트
        leftPet.SetTargetPosition(leftSlotPos);
        rightPet.SetTargetPosition(rightSlotPos);
    }
}
