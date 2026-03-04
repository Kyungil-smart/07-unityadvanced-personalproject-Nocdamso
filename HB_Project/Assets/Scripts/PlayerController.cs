using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("움직임 설정")]
    public float Speed = 5f;
    public float RotateSpeed = 10f;
    public float JumpHeight = 0.5f;

    [Header("바닥 체크")]
    public float GroundCheckDistance = 0.2f;
    public LayerMask GroundLayer;
    [Header("록온 상태")]
    public bool IsLockOn = false;
    public Transform LockOnTarget;

    private CharacterController _controller;
    private Vector2 _moveInput;
    private Vector3 _velocity;
    
    private Animator _animator;
    private Transform _cameraTransform;

    private bool _jumpInput;
    private bool _lockOnInput;


    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _attackAction;
    private InputAction _lockOnAction;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _cameraTransform = Camera.main.transform;

        _moveAction = InputSystem.actions["Move"];
        _jumpAction = InputSystem.actions["Jump"];
        _lockOnAction = InputSystem.actions["LockOn"];
        _attackAction = InputSystem.actions["Attack"];
        
    }

    void OnEnable()
    {
        _moveAction.performed += OnMove;
        _moveAction.canceled += MoveCancel;
        _jumpAction.started += Onjump;
        _lockOnAction.started += OnLockOn;
        _attackAction.started += OnAttack;        
    }

    void OnDisable()
    {
        _moveAction.performed -= OnMove;
        _moveAction.canceled -= MoveCancel;
        _jumpAction.started -= Onjump; 
        _lockOnAction.started -= OnLockOn;
        _attackAction.started -= OnAttack;
    }

    void Update()
    {
        // Debug.Log(_moveInput);
        // Debug.Log($"move enabled: {_moveAction.enabled}");
        // Debug.Log($"moveInput:{_moveInput} pos:{transform.position}");

        ApplyGravity();

        if (IsLockOn && LockOnTarget != null)
        {
            LockOnMovement();
        }
        else
        {
            DefaultMovement();
        }

        UpdateAnimator();        
    }
    
    void ApplyGravity()
    {
        bool grounded = IsGrounded();

        // 플레이어가 땅에 있을 때 고정
        if (grounded)
        {
            _velocity.y = -2f;
        }

        // 땅에 있는 채로 점프키를 누르면 점프
        if(_jumpInput && grounded)
        {
            _velocity.y = Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y);
            _jumpInput = false;
        }
        
        // 점프 후 내려올 때 체공시간을 짧게 하기 위해 중력을 배로 늘림
        float gravityMultiplier = (_velocity.y < 0) ? 10.0f : 1.0f;
        _velocity.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime); 
    }

    void DefaultMovement()
    {
        Vector3 moveDirection = BaseCameraDirection();

        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotateSpeed * Time.deltaTime);
        }

        _controller.Move(moveDirection * Speed * Time.deltaTime);
    }

    void LockOnMovement()
    {
        Vector3 moveDirection = BaseCameraDirection();

        Vector3 lookDir = LockOnTarget.position - transform.position;
        lookDir.y = 0f;
        if(lookDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotateSpeed * Time.deltaTime);
        }

        _controller.Move(moveDirection * Speed * Time.deltaTime);
    }

    Vector3 BaseCameraDirection()
    {
        Vector3 forward = _cameraTransform.forward;
        Vector3 right = _cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        return (forward * _moveInput.y + right * _moveInput.x).normalized;
    }

    void UpdateAnimator()
    {
        if (_animator == null) return;

        if (IsLockOn)
        {
            float inputX = Mathf.Abs(_moveInput.x) < 0.1f ? 0f : _moveInput.x;
            float inputZ = Mathf.Abs(_moveInput.y) < 0.1f ? 0f : _moveInput.y;
            
            _animator.SetFloat("InputX", inputX, 0.1f, Time.deltaTime);
            _animator.SetFloat("InputZ", inputZ, 0.1f, Time.deltaTime);
        }
        else
        {
            _animator.SetFloat("InputX", 0f, 0.1f, Time.deltaTime);
            _animator.SetFloat("InputZ", _moveInput.magnitude,0.1f, Time.deltaTime);
        }

        _animator.SetFloat("MoveSpeed", _moveInput.magnitude);
    }

    bool IsGrounded()
    {
        if(_controller.isGrounded) return true;

        Vector3 ray = transform.position + Vector3.up * 0.1f;

        if (Physics.Raycast(ray, Vector3.down, out RaycastHit hit, GroundCheckDistance, GroundLayer)) return true;

        return false;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();
    }

    public void MoveCancel(InputAction.CallbackContext ctx)
    {
        _moveInput = Vector2.zero;
    }

    public void Onjump(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            _jumpInput = true;

            if(_animator != null) _animator.SetTrigger("Jump");
        }
    }

    public void OnLockOn(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            IsLockOn = !IsLockOn;
            Debug.Log("락온 누름");
        }
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        
    }
    
}
