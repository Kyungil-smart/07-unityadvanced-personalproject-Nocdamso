using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    private PlayerController _playerController;
    private Animator _animator;
    private PlayerHealth _playerHealth;
    private PlayerStamina _playerStamina;
    private Items _items;

    private bool _isButtonDown = false;
    public bool IsRolling { get; private set; }
    public bool IsRunning { get; private set; }

    [Header("입력 감도")]
    [SerializeField] private float _rollThreshold = 0.2f;
    private float _pressStartTime;


    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _animator = GetComponent<Animator>();
        _playerHealth = GetComponent<PlayerHealth>();
        _playerStamina = GetComponent<PlayerStamina>();
        _items = GetComponent<Items>();
    }

    public void MoveUpdate()
    {
        // 죽었거나 게임오버 ESC메뉴가 열려있으면 이동로직 무시
        if (_playerHealth.IsDead ||
            (GameSceneManager.Instance != null && GameSceneManager.Instance.IsGameOver) ||
            (GameSceneManager.Instance != null && GameSceneManager.Instance.IsMenuOpen))
        {
            if(IsRunning) StopRunning();
            return;
        }

        if (_isButtonDown && !IsRolling && (Time.time - _pressStartTime >= _rollThreshold))
        {
            // 처음 한 번만 실행되도록
            if (!IsRunning && _playerStamina.CurrentStamina > 1f) 
            {
                IsRunning = true;
                _animator.SetBool(AnimatorHash.IsRunning, true);
            }
        }

        if (IsRunning)
        {
            if(_playerController.GetMoveInput().magnitude > 0.1f)
            {
                _playerStamina.SpendStaminaPerSec(_playerStamina.RunCostPerSecond);
            }

            if(_playerStamina.CurrentStamina <= 0)
            {
                StopRunning();
            }
        }

        if (!IsRolling)
        {
            // 중력 및 이동 로직 실행
            ApplyGravity();

            if (_playerController.IsAttack) return;

            if (_playerController.IsLockOn && _playerController.LockOnTarget != null) LockOnMovement();
            else DefaultMovement(); 
        }
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
            if (_items != null && _items.IsDrinking) return;

            if (_playerStamina.SpendStamina(_playerStamina.JumpCost))
            {
                _playerController.JumpInput = true;

                _animator.SetTrigger(AnimatorHash.Jump);                      
            }
        }
    }

    public void OnDodgeSprint(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {   
            // 처음 버튼 누른 시간 저장
            _pressStartTime = Time.time;
            _isButtonDown = true;
        } 

        if (ctx.performed)
        {
            // 누른 상태가 지속되면 달림
            if (Time.time - _pressStartTime >= _rollThreshold && !IsRolling)
            {
                IsRunning = true;
                _animator.SetBool(AnimatorHash.IsRunning, true);
            }
        }

        if (ctx.canceled)
        {
            // 버튼을 짧게 눌렀다 때면 구름
            _isButtonDown = false;

            if(Time.time - _pressStartTime < _rollThreshold 
                                            && !IsRolling 
                                            && _playerController.IsGrounded() 
                                            && _playerStamina.CanAction(_playerStamina.RollCost))
            {
                StartCoroutine(RollRoutine());
            }

            StopRunning();
        }
    }

    private void StopRunning()
    {
        IsRunning = false;
        _animator.SetBool(AnimatorHash.IsRunning, false);
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

        float currentSpeed = IsRunning ? _playerController.RunSpeed : _playerController.WalkSpeed;
        currentSpeed *= _items.MoveSpeedModifier();

        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _playerController.RotateSpeed * Time.deltaTime);
        }
        _playerController.GetCharacterController().Move(moveDirection * currentSpeed * Time.deltaTime);
    }

    void LockOnMovement()
    {
        Vector3 moveDirection = _playerController.BaseCameraDirection();
        Vector3 lookDir = _playerController.LockOnTarget.position - transform.position;
        lookDir.y = 0f;

        float currentSpeed = IsRunning ? _playerController.RunSpeed : _playerController.WalkSpeed;

        if (lookDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _playerController.RotateSpeed * Time.deltaTime);
        }

        _playerController.GetCharacterController().Move(moveDirection * currentSpeed * Time.deltaTime);
    }

    private IEnumerator RollRoutine()
    {
        if (_items != null && _items.IsDrinking)
        {
            _items.OnDrinkEnd();
        }

        _playerStamina.SpendStamina(_playerStamina.RollCost);

        IsRolling = true;
        if (_playerHealth != null) _playerHealth.IsInvincible = true;
        _animator.SetTrigger(AnimatorHash.Roll);
        
        // 카메라 기준 입력 방향으로 구름
        Vector3 rollDir = _playerController.BaseCameraDirection();

        // 입력이 없다면 바라보는 방향으로 구름
        if (rollDir == Vector3.zero) 
        {
            rollDir = transform.forward;            
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(rollDir);
        }

        float timer = 0f;
        while (timer < _playerController.RollDuration)
        {
            // 중력을 유지하며 구르기
            Vector3 rollMove = rollDir * _playerController.RollSpeed;
            rollMove.y = _playerController.Velocity.y;

            _playerController.GetCharacterController().Move(rollMove * Time.deltaTime);

            // 중력 계산
            Vector3 gravity = _playerController.Velocity;
            gravity.y += Physics.gravity.y * Time.deltaTime;
            _playerController.Velocity = gravity;

            timer += Time.deltaTime;
            yield return null;
        }

        IsRolling = false;

        if (_playerHealth != null) _playerHealth.IsInvincible = false;
    }

    public void ResetRollingState()
    {
        IsRolling = false;

        if(_playerHealth != null) _playerHealth.IsInvincible = false;
    }
}