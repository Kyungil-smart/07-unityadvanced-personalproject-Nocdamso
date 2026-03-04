using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
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
    public bool IsAttack { get; set; } = false;
    public bool JumpInput { get; set; } = false;

    private CharacterController _controller;
    private Vector2 _moveInput;
    private Vector3 _velocity; 
    private Animator _animator;
    private Transform _cameraTransform;

    // 각 컴포넌트 참조
    private PlayerMove _playerMove;
    private PlayerCombat _playerCombat;
    private PlayerAnimations _playerAnimation;
    
    // InputAction 참조
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _attackAction;
    private InputAction _lockOnAction;


    public Vector3 Velocity { get => _velocity; set => _velocity = value; }
    public Animator GetAnimator() => _animator;
    public CharacterController GetCharacterController() => _controller;
    public Vector2 GetMoveInput() => _moveInput;
    public void SetMoveInput(Vector2 input) => _moveInput = input;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _cameraTransform = Camera.main.transform;

        _moveAction = InputSystem.actions["Move"];
        _jumpAction = InputSystem.actions["Jump"];
        _attackAction = InputSystem.actions["Attack"];
        _lockOnAction = InputSystem.actions["LockOn"];

        _playerMove = GetComponent<PlayerMove>();
        _playerCombat = GetComponent<PlayerCombat>();
        _playerAnimation = GetComponent<PlayerAnimations>();
    }

    void OnEnable()
    {
        _moveAction.performed += _playerMove.OnMove;
        _moveAction.canceled += _playerMove.MoveCancel;
        _jumpAction.started += _playerMove.OnJump;
        
        _attackAction.started += _playerCombat.OnAttack;
        _lockOnAction.started += _playerCombat.OnLockOn;
    }

    void OnDisable()
    {
        _moveAction.performed -= _playerMove.OnMove;
        _moveAction.canceled -= _playerMove.MoveCancel;
        _jumpAction.started -= _playerMove.OnJump;
        
        _attackAction.started -= _playerCombat.OnAttack;
        _lockOnAction.started -= _playerCombat.OnLockOn;
    }

    void Update()
    {
        _playerMove.MoveUpdate();
        _playerCombat.CombatUpdate();
        _playerAnimation.AnimationUpdate();
    }

    public Vector3 BaseCameraDirection()
    {
        Vector3 forward = _cameraTransform.forward;
        Vector3 right = _cameraTransform.right;
        forward.y = 0f; right.y = 0f;
        forward.Normalize(); right.Normalize();
        return (forward * _moveInput.y + right * _moveInput.x).normalized;
    }

    public bool IsGrounded()
    {
        if (_controller.isGrounded) return true;
        Vector3 ray = transform.position + Vector3.up * 0.1f;
        return Physics.Raycast(ray, Vector3.down, GroundCheckDistance, GroundLayer);
    }
}