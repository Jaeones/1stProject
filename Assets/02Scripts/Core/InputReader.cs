using System;
using UnityEngine;

public class InputReader : MonoBehaviour
{
    public event Action<Vector2>MoveEvent;
    public event Action<Vector2>LookEvent;
    public event Action JumpEvent;
    public event Action<bool>RunEvent;

    private void Update()
    {
        HandleMoveInput();
        HandleLookInput();
        HandleJumpInput();
        HandleRunInput();
    }

    void HandleMoveInput()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector2 inputVector = new Vector2(x, y).normalized;
        MoveEvent?.Invoke(inputVector);

    }

    void HandleLookInput()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");
        LookEvent?.Invoke(new Vector2(x, y));
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
