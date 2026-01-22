using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Ground Check Settings")]
    [SerializeField]
    private LayerMask groundLayer;
    // 플레이어 발밑에서 땅까지의 거리 허용 오차
    private float groundCheckDist = 0.2f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // 미끄러움 방지등을 위한 물리 설정
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public void Move(Vector2 inputDirection, float speed, float rotationSpeed)
    {
        if (inputDirection.sqrMagnitude < 0.1f) return;

        //1. 이동 방향 계산 (3D 벡터)
        Vector3 direction = new Vector3(inputDirection.x, 0f, inputDirection.y).normalized;

        //2. 캐릭터 회전
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        //3. 물리 이동 (기존 Y축 속도 유지)
        Vector3 moveVelocity = direction * speed;
        rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);
    }

    public void Jump(float jumpForce)
    {
        if (IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    public bool IsGrounded()
    {
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        float rayDistance = groundCheckDist + 0.1f;
        // 캐릭터 중심에서 아래로 레이캐스트 발사
        Debug.DrawRay(rayStart, Vector3.down * rayDistance, Color.red);
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDist + 0.1f, groundLayer);
    }

    // 데이터 세팅용(플레이어 컨트롤러에서 호출)
    public void SetGroundCheckSettings(float dist, LayerMask layer)
    {
        groundCheckDist = dist;
        groundLayer = layer;
    }
}
