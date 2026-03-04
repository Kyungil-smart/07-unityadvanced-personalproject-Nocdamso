using UnityEngine;
using UnityEngine.InputSystem;

public class LegacyPlayerController : MonoBehaviour
{
    [Header("설정")]
    public float Speed = 5f;
    public float RotateSpeed = 10f;
    public float JumpHeight = 1f;
    public float GroundCheckDistance = 0.2f;
    public LayerMask GroundLayer;

    [Header("상태")]
    public bool IsLockOn = false;
    public Transform LockOnTarget;
    public bool IsAttack = false;
    private bool _jumpInput = false;

    // 컴포넌트 참조
    private CharacterController _controller;
    private Animator _animator;
    private Transform _cameraTransform;
    private Vector2 _moveInput;
    private Vector3 _velocity;

    // InputAction 참조
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _attackAction;
    private InputAction _lockOnAction;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _cameraTransform = Camera.main.transform;

        // InputSystem에서 액션 가져오기
        _moveAction = InputSystem.actions["Move"];
        _jumpAction = InputSystem.actions["Jump"];
        _attackAction = InputSystem.actions["Attack"];
        _lockOnAction = InputSystem.actions["LockOn"];
    }

    void OnEnable()
    {
        // 입력 이벤트 연결
        _moveAction.performed += OnMove;
        _moveAction.canceled += OnMoveCancel;
        _jumpAction.started += OnJump;
        _attackAction.started += OnAttack;
        _lockOnAction.started += OnLockOn;
    }

    void OnDisable()
    {
        // 입력 이벤트 해제
        _moveAction.performed -= OnMove;
        _moveAction.canceled -= OnMoveCancel;
        _jumpAction.started -= OnJump;
        _attackAction.started -= OnAttack;
        _lockOnAction.started -= OnLockOn;
    }

    void Update()
    {
        ApplyGravity();

        // 공격 중에는 이동 로직을 타지 않음
        if (IsAttack)
        {
            HandleAttackRotation();
        }
        else
        {
            HandleMovement();
        }

        UpdateAnimation();
    }

    #region Input Methods
    private void OnMove(InputAction.CallbackContext ctx) => _moveInput = ctx.ReadValue<Vector2>();
    private void OnMoveCancel(InputAction.CallbackContext ctx) => _moveInput = Vector2.zero;
    
    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (IsGrounded())
        {
            _jumpInput = true;
            _animator.SetTrigger("Jump");
        }
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        if (IsGrounded() && !IsAttack)
        {
            _animator.SetTrigger("Attack");
        }
    }

    private void OnLockOn(InputAction.CallbackContext ctx)
    {
        IsLockOn = !IsLockOn;
    }
    #endregion

    #region Core Logic
    private void HandleMovement()
    {
        Vector3 moveDir = GetBaseCameraDirection();

        if (IsLockOn && LockOnTarget != null)
        {
            // 록온 이동: 대상을 바라보며 이동
            Vector3 lookDir = LockOnTarget.position - transform.position;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, RotateSpeed * Time.deltaTime);
            }
        }
        else if (moveDir.magnitude > 0.1f)
        {
            // 일반 이동: 이동 방향을 바라봄
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, RotateSpeed * Time.deltaTime);
        }

        _controller.Move(moveDir * Speed * Time.deltaTime);
    }

    private void HandleAttackRotation()
    {
        Vector3 targetDir = Vector3.zero;
        if (IsLockOn && LockOnTarget != null)
            targetDir = (LockOnTarget.position - transform.position).normalized;
        else if (_moveInput.magnitude > 0.1f)
            targetDir = GetBaseCameraDirection();

        if (targetDir != Vector3.zero)
        {
            targetDir.y = 0;
            Quaternion targetRot = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, RotateSpeed * Time.deltaTime);
        }
    }

    private void ApplyGravity()
    {
        bool grounded = IsGrounded();
        if (grounded && _velocity.y < 0) _velocity.y = -2f;

        if (_jumpInput && grounded)
        {
            _velocity.y = Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y);
            _jumpInput = false;
        }

        float gravityMultiplier = (_velocity.y < 0) ? 2.0f : 1.0f;
        _velocity.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void UpdateAnimation()
    {
        if (IsLockOn)
        {
            _animator.SetFloat("InputX", _moveInput.x, 0.1f, Time.deltaTime);
            _animator.SetFloat("InputZ", _moveInput.y, 0.1f, Time.deltaTime);
        }
        else
        {
            _animator.SetFloat("InputX", 0f, 0.1f, Time.deltaTime);
            _animator.SetFloat("InputZ", _moveInput.magnitude, 0.1f, Time.deltaTime);
        }
        
        _animator.SetFloat("MoveSpeed", IsAttack ? 0f : _moveInput.magnitude);
    }
    #endregion

    #region Helpers
    private Vector3 GetBaseCameraDirection()
    {
        Vector3 forward = _cameraTransform.forward;
        Vector3 right = _cameraTransform.right;
        forward.y = 0; right.y = 0;
        return (forward * _moveInput.y + right * _moveInput.x).normalized;
    }

    private bool IsGrounded()
    {
        if (_controller.isGrounded) return true;
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, GroundCheckDistance, GroundLayer);
    }

    // 애니메이션 이벤트 수신용
    public void SetAttacking(int value) => IsAttack = (value == 1);
    public void JumpAttack(float force) => _velocity.y = force;
    #endregion
}
