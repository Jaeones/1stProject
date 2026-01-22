using UnityEngine;

// 에디터 우클릭 메뉴에 생성 옵션을 추가합니다.
[CreateAssetMenu(fileName = "DefaultPlayerStats", menuName = "Player/PlayerStats")]
public class PlayerStatsSO : ScriptableObject
{
    [Header("Movement Settings")]
    [Tooltip("기본 걷기 속도")]
    public float WalkSpeed = 5f;

    [Tooltip("달리기(Shift) 속도")]
    public float RunSpeed = 8f;

    [Tooltip("회전 속도 (높을수록 빠릿하게 돔)")]
    public float RotationSpeed = 10f;

    [Header("Jump Settings")]
    [Tooltip("점프 힘")]
    public float JumpForce = 5f;

    [Tooltip("땅 감지 거리 (약간 넉넉하게)")]
    public float GroundCheckDistance = 0.2f;

    [Tooltip("중력 계수 (기본 중력에 곱해짐)")]
    public float GravityMultiplier = 2f;
}