using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private AttackSensor _weaponSensor;

    private PlayerController _playerController;
    private PlayerStamina _playerStamina;
    private Items _items;

    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _playerStamina = GetComponent<PlayerStamina>();
        _items = GetComponent<Items>();
    }

    public void CombatUpdate()
    {
        // 공격 상태일 때만 타겟이나 카메라 방향으로 회전
        if (_playerController.IsAttack) 
        {
            if (_playerController.IsAttack)
            {
                AttackRotation();
            }
        }
    }

    public void OnLightAttack(InputAction.CallbackContext ctx)
    {
        if(!ctx.started) return;

        if(!_playerController.IsGrounded() || (_items != null && _items.IsDrinking)) return;

        if (_playerStamina.CanAction(_playerStamina.AttackCost))
        {
            _playerController.GetAnimator().SetTrigger(AnimatorHash.LightAttack);
        }
    }

    public void OnHeavyAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;

        if (_items != null && _items.IsDrinking) return;

        if (_playerStamina.CanAction(_playerStamina.AttackCost * 1.5f))
        {
            _playerController.GetAnimator().SetTrigger(AnimatorHash.HeavyAttack);
        }
    }

    void AttackRotation()
    {
        Vector3 targetDir = Vector3.zero;

        // 록온 대상이 있으면 대상을 향해 회전
        if (_playerController.IsLockOn && _playerController.LockOnTarget != null)
        {
            targetDir = (_playerController.LockOnTarget.position - transform.position).normalized;
        }
        // 록온 대상이 없으면 이동 입력 방향(카메라 기준)으로 회전
        else if (_playerController.GetMoveInput().magnitude > 0.1f)
        {
            targetDir = _playerController.BaseCameraDirection();
        }

        if (targetDir != Vector3.zero)
        {
            // 바닥 평면 회전만 허용
            targetDir.y = 0; 
            Quaternion targetRotation = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _playerController.RotateSpeed * Time.deltaTime);
        }
    }

    public void EnableWeaponCollision()
    {
        if(_weaponSensor != null)
        {
            _weaponSensor.EnableAttack();
        }
    }

    public void DisableWeaponCollision()
    {
        if (_weaponSensor != null)
        {
            _weaponSensor.DisableAttack();
        }
    }

    public void SetAttackDamage(float damage)
    {
        if (_weaponSensor != null)
        {
            _weaponSensor.SetDamage(damage);
        }
    }

    public void OnLockOn(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            _playerController.IsLockOn = !_playerController.IsLockOn;
        }
    }


    public void AttackStaminaEvent()
    {
        _playerStamina.SpendStamina(_playerStamina.AttackCost);
    }


    // --- 애니메이션 이벤트 리시버 ---

    // 공격 애니메이션의 특정 프레임에서 IsAttack 상태를 제어 (1: 시작, 0: 종료)
    public void SetAttacking(int value)
    {
        _playerController.IsAttack = (value == 1);

        if (value == 0)
        {
            _playerController.GetAnimator().ResetTrigger(AnimatorHash.LightAttack);
            _playerController.GetAnimator().ResetTrigger(AnimatorHash.HeavyAttack);
        }
    }

    // 점프 공격 애니메이션 등에서 물리적인 힘을 가할 때 사용
    public void JumpAttack(float force)
    {
        Vector3 currentVelocity = _playerController.Velocity;
        currentVelocity.y = force; 

        _playerController.Velocity = currentVelocity;
    }
}