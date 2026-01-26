using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;   //내 몸체 콜라이더 정보

    [Header("Ground Check Settings")]
    [SerializeField]
    private LayerMask groundLayer;
    // 플레이어 발밑에서 땅까지의 거리 허용 오차
    private float groundCheckDist = 0.2f;

    [Header("Camera Root")]
    [SerializeField]private Transform cameraRoot;
    [SerializeField] private float sensitivity = 2f;    //마우스 감도

    private float camPitch;    // 상하 회전 값

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        // 미끄러움 방지등을 위한 물리 설정
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Move(Vector3 direction, float speed)
    {
        Vector3 moveVelocity = direction * speed;
        rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);
    }

    public void Look(Vector2 mouseInput)
    {
        //플레이어 좌우 회전
        transform.Rotate(Vector3.up * mouseInput.x * sensitivity);
        //카메라 상하 회전
        camPitch -= mouseInput.y * sensitivity;
        camPitch = Mathf.Clamp(camPitch, -60f, 60f);
        cameraRoot.localRotation = Quaternion.Euler(camPitch, 0f, 0f);
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
        Vector3 rayStart;
        if (col != null)
        {
            rayStart = new Vector3(transform.position.x, col.bounds.min.y + 0.1f, transform.position.z);
        }
        else
        {
            rayStart = transform.position + Vector3.up * 0.1f;
        }

        float rayDistance = groundCheckDist + 0.1f;

        Debug.DrawRay(rayStart, Vector3.down * rayDistance, Color.red);

        // 2. 발바닥에서 아래로 레이저 발사
        return Physics.Raycast(rayStart, Vector3.down, rayDistance, groundLayer);
    }

    // 데이터 세팅용(플레이어 컨트롤러에서 호출)
    public void SetGroundCheckSettings(float dist, LayerMask layer)
    {
        groundCheckDist = dist;
        groundLayer = layer;
    }
}
