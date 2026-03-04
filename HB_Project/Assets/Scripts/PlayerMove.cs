using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    private PlayerController _playerController;

    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    public void MoveUpdate()
    {
        // 중력 및 이동 로직 실행
        ApplyGravity();

        if (_playerController.IsAttack) return;

        if (_playerController.IsLockOn && _playerController.LockOnTarget != null) LockOnMovement();
        else DefaultMovement();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        _playerController.SetMoveInput(ctx.ReadValue<Vector2>());
    }

    public void MoveCancel(InputAction.CallbackContext ctx)
    {
        _playerController.SetMoveInput(Vector2.zero);
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.started && _playerController.IsGrounded())
        {
            _playerController.JumpInput = true;
            if (_playerController.GetAnimator() != null) _playerController.GetAnimator().SetTrigger("Jump");
        }
    }

    void ApplyGravity()
    {
        bool grounded = _playerController.IsGrounded();
        Vector3 velocity = _playerController.Velocity;

        // 땅에선 가속도 초기화
        if (grounded && velocity.y < 0) velocity.y = -2f;

        // 점프 로직
        if (_playerController.JumpInput && grounded)
        {
            velocity.y = Mathf.Sqrt(_playerController.JumpHeight * -2f * Physics.gravity.y);
            _playerController.JumpInput = false;
        }

        // 하강할 때는 중력이 더 적용될 수 있도록
        float gravityMultiplier = (velocity.y < 0) ? 2.0f : 1.0f;
        velocity.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;

        _playerController.Velocity = velocity;
        _playerController.GetCharacterController().Move(velocity * Time.deltaTime);
    }

    void DefaultMovement()
    {
        Vector3 moveDirection = _playerController.BaseCameraDirection();
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _playerController.RotateSpeed * Time.deltaTime);
        }
        _playerController.GetCharacterController().Move(moveDirection * _playerController.Speed * Time.deltaTime);
    }

    void LockOnMovement()
    {
        Vector3 moveDirection = _playerController.BaseCameraDirection();
        Vector3 lookDir = _playerController.LockOnTarget.position - transform.position;
        lookDir.y = 0f;

        if (lookDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _playerController.RotateSpeed * Time.deltaTime);
        }

        _playerController.GetCharacterController().Move(moveDirection * _playerController.Speed * Time.deltaTime);
    }
}