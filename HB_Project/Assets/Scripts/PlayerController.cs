using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("이동 속도")]
    private float rotateSpeed = 10f;
    private float _walkSpeed = 1.0f;

    [Header("구르기 달리기")]
    private float _pressTime = 0.2f;
    private float _pressButtonTimer = 0f;
    private bool _isDodgeButtonPressed = false;


    private Vector2 _moveInput;
    private bool _isRunning;
    private bool _isSlowWalking;
    private Vector3 _velocity;


    private CharacterController _controller;
    private Animator _animator;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    private void OnSlowWalk(InputValue value)
    {
        _isSlowWalking = value.isPressed;
    }

    private void OnDodgeSprint(InputValue value)
    {
        if (value.isPressed)
        {
            // 초기화
            _isDodgeButtonPressed = true;
            _pressButtonTimer = 0f;
        }
        else
        {
            _isDodgeButtonPressed = false;
            // 눌렀다 때는 시간이 0.2초 보다 빠르면 구르기
            if (_pressButtonTimer < _pressTime)
            {
                Dodge();
            }
            _isRunning = false;
        }
    }

    private void Update()
    {
        CheckPressButtonTimer();
        PlayerMove();
        Gravity();
    }

    private void CheckPressButtonTimer()
    {
        if (_isDodgeButtonPressed)
        {
            _pressButtonTimer += Time.deltaTime;

            if (_pressButtonTimer >= _pressTime)
            {
                _isRunning = true;
            }
        }
    }

    private void PlayerMove()
    {
        
    }
    
    private void Dodge()
    {
        
    }

    private void Gravity()
    {
        if(_controller.isGrounded && _velocity.y < 0)
            _velocity.y = -2f;

        _velocity.y += -9.81f * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }
}
