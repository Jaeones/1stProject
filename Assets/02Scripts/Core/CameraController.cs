using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform followTarget;
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, -3f);

    [Header("Settings")]
    [SerializeField] private float sensitivity = 2f;    //마우스 감도
    [SerializeField] private float lookUpMin = -60f; //위로 올려다보는 제한
    [SerializeField] private float lookUpMax = 60f;  //아래로 내려다보는 제한

    private float pitch;    // 상하 회전 값

    private void Start()
    {
        //마우스 커서 숨기기
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (followTarget == null) return;
    }

    //플레이어 컨트롤러에서 입력을 받아 카메라를 회전
    public void RotateCamera(Vector2 mouseInput)
    {
        pitch -= mouseInput.y * sensitivity;
        pitch = Mathf.Clamp(pitch, lookUpMin, lookUpMax);

        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}

