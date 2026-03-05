using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float WalkSpeed = 5f;
    public float RunSpeed = 10f;
    public float RotateSpeed = 10f;

    [Header("점프 설정")]
    public float JumpHeight = 1f;

    [Header("구르기 설정")]
    public float RollSpeed = 1f;
    public float RollDuration = 0.5f;
    public float InvincibleDuration = 0.5f;

    [Header("바닥 설정")]
    public float GroundCheckDistance = 0.2f;
    public LayerMask GroundLayer;

    [Header("상태")]
    public bool IsLockOn = false;
    public Transform LockOnTarget;
    public bool IsAttack { get; set; } = false;
    public bool JumpInput { get; set; } = false;

    private bool _isGrounded;

    private CharacterController _controller;
    private Vector2 _moveInput;
    private Vector3 _velocity; 
    private Animator _animator;
    private Transform _cameraTransform;

    // 각 컴포넌트 참조
    private PlayerMove _playerMove;
    private PlayerCombat _playerCombat;
    private PlayerAnimations _playerAnimation;
    private PlayerHealth _playerHealth;
    private Items _items;
    
    // InputAction 참조
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _lightAttackAction;
    private InputAction _heavyAttackAction;
    private InputAction _lockOnAction;
    private InputAction _dodgeSprintAction;
    private InputAction _useItemAction;


    public Vector3 Velocity { get => _velocity; set => _velocity = value; }
    public Animator GetAnimator() => _animator;
    public CharacterController GetCharacterController() => _controller;
    public Vector2 GetMoveInput() => _moveInput;
    public void SetMoveInput(Vector2 input) => _moveInput = input;

    public bool IsGrounded() => _isGrounded;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _cameraTransform = Camera.main.transform;

        _moveAction = InputSystem.actions["Move"];
        _jumpAction = InputSystem.actions["Jump"];
        _lightAttackAction = InputSystem.actions["LightAttack"];
        _heavyAttackAction = InputSystem.actions["HeavyAttack"];
        _lockOnAction = InputSystem.actions["LockOn"];
        _dodgeSprintAction = InputSystem.actions["DodgeSprint"];
        _useItemAction = InputSystem.actions["UseItem"];

        _playerMove = GetComponent<PlayerMove>();
        _playerCombat = GetComponent<PlayerCombat>();
        _playerAnimation = GetComponent<PlayerAnimations>();
        _playerHealth = GetComponent<PlayerHealth>();
        _items = GetComponent<Items>();
    }

    void OnEnable()
    {
        _moveAction.performed += _playerMove.OnMove;
        _moveAction.canceled += _playerMove.MoveCancel;
        _jumpAction.started += _playerMove.OnJump;

        _dodgeSprintAction.started += _playerMove.OnDodgeSprint;
        _dodgeSprintAction.performed += _playerMove.OnDodgeSprint;
        _dodgeSprintAction.canceled += _playerMove.OnDodgeSprint;

        _lightAttackAction.started += _playerCombat.OnLightAttack;
        _heavyAttackAction.started += _playerCombat.OnHeavyAttack;
        _lockOnAction.started += _playerCombat.OnLockOn;
        
        _useItemAction.started += _items.OnUseItem;
    }

    void OnDisable()
    {
        _moveAction.performed -= _playerMove.OnMove;
        _moveAction.canceled -= _playerMove.MoveCancel;
        _jumpAction.started -= _playerMove.OnJump;

        _dodgeSprintAction.started -= _playerMove.OnDodgeSprint;
        _dodgeSprintAction.performed -= _playerMove.OnDodgeSprint;
        _dodgeSprintAction.canceled -= _playerMove.OnDodgeSprint;

        _lightAttackAction.started -= _playerCombat.OnLightAttack;
        _heavyAttackAction.started -= _playerCombat.OnHeavyAttack;
        _lockOnAction.started -= _playerCombat.OnLockOn;

        _useItemAction.started -= _items.OnUseItem;
    }

    void Update()
    {
        if(_playerHealth != null && _playerHealth.IsDead) return;

        _isGrounded = CheckGround();
        _playerMove.MoveUpdate();
        _playerCombat.CombatUpdate();
        _playerAnimation.AnimationUpdate();
    }

    private bool CheckGround()
    {
        if (_controller.isGrounded)
        {
            return true;
        }

        Vector3 ray = transform.position + Vector3.up * 0.1f;
        return Physics.Raycast(ray, Vector3.down, GroundCheckDistance, GroundLayer);
    }

    public Vector3 BaseCameraDirection()
    {
        if(_moveInput.magnitude < 0.1f) return Vector3.zero;

        Vector3 forward = _cameraTransform.forward;
        Vector3 right = _cameraTransform.right;
        forward.y = 0f; right.y = 0f;
        forward.Normalize(); right.Normalize();
        return (forward * _moveInput.y + right * _moveInput.x).normalized;
    }
}