using System;
using UnityEngine;

public class InputReader : MonoBehaviour
{
    public event Action<Vector2>MoveEvent;
    public event Action JumpEvent;
    public event Action<bool>RunEvent;

    private void Update()
    {
        HandleMoveInput();
        HandleJumpInput();
        HandleRunInput();
    }

    void HandleMoveInput()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector2 inputVector = new Vector2(x, y);

        // 입력 벡터 정규화 (대각선 이동 시 속도 증가 방지)
        if (inputVector.sqrMagnitude > 1f)
        {
            inputVector.Normalize();
        }

        // 이벤트를 구독한 녀석들에게 알림 발송
        MoveEvent?.Invoke(inputVector);

    }
    void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            JumpEvent?.Invoke();
        }
    }

    void HandleRunInput()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        RunEvent?.Invoke(isRunning);
    }
}
