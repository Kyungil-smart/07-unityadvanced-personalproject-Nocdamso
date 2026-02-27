using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("움직임 속도")]
    public float speed = 5f;

    public float jumpHeight = 0.5f;

    [Header("바닥 체크")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;  

    private CharacterController _controller;
    private Vector2 _moveInput;
    private Vector3 _velocity;
    
    private Animator _animator;

    private bool _jumpInput;


    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _attackAction;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _moveAction = InputSystem.actions["Move"];
        _jumpAction = InputSystem.actions["Jump"];
        _attackAction = InputSystem.actions["Attack"];
        

    }

    void OnEnable()
    {
        _moveAction.performed += OnMove;
        _moveAction.canceled += MoveCancel;
        _jumpAction.started += Onjump;
        _jumpAction.canceled += OnJumpCanceled;
        _attackAction.started += OnAttack;        
    }

    void OnDisable()
    {
        _moveAction.performed -= OnMove;
        _moveAction.canceled -= MoveCancel;
        _jumpAction.started -= Onjump; 
        _jumpAction.canceled -= OnJumpCanceled;
        _attackAction.started -= OnAttack;
    }

    void Update()
    {
        Debug.Log(_moveInput);
        Debug.Log($"move enabled: {_moveAction.enabled}");
        Debug.Log($"moveInput:{_moveInput} pos:{transform.position}");
        bool grounded = IsGrounded();

        if (grounded)
        {
            _velocity.y = -2f;
        }

        if(_jumpInput && grounded)
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
            _jumpInput = false;
        }
        _velocity.y += Physics.gravity.y * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
      
        
        Vector3 move = transform.right * _moveInput.x + transform.forward * _moveInput.y;
        _controller.Move(move * speed * Time.deltaTime);   


        if (_animator == null) return;

        float inputX = Mathf.Abs(_moveInput.x) < 0.1f ? 0f : _moveInput.x;
        float inputZ = Mathf.Abs(_moveInput.y) < 0.1f ? 0f : _moveInput.y;
        
        _animator.SetFloat("InputX", inputX, 0.1f, Time.deltaTime);
        _animator.SetFloat("InputZ", inputZ, 0.1f, Time.deltaTime);

        _animator.SetFloat("MoveSpeed", _moveInput.magnitude);
        
    }

    bool IsGrounded()
    {
        if(_controller.isGrounded) return true;

        Vector3 ray = transform.position + Vector3.up * 0.1f;

        if (Physics.Raycast(ray, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer)) return true;

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

    public void OnJumpCanceled(InputAction.CallbackContext ctx)
    {
        if(_velocity.y > 0)
        {
            _velocity.y *= .5f;
        }
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        
    }
    
}
