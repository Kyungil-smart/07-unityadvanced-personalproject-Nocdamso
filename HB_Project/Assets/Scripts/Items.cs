using UnityEngine;
using UnityEngine.InputSystem;

public class Items : MonoBehaviour
{
    [Header("물약 설정")]
    [SerializeField] private float _healAmount = 30f;
    [SerializeField] private float _moveSpeedMultiplier = 0.3f;
    public int MaxPotionCount = 5;
    public int PotionCount;

    private PlayerController _playerController;
    private HealthPoint _healthpoint;
    private Animator _animator;
    private PlayerMove _playerMove;

    public bool IsDrinking { get; private set;}

    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _healthpoint = GetComponent<HealthPoint>();
        _animator = GetComponent<Animator>();

        _playerMove = GetComponent<PlayerMove>();
        PotionCount = MaxPotionCount;
    } 

    public void OnUseItem(InputAction.CallbackContext ctx)
    {
        if(IsDrinking || _playerController.IsAttack 
                      || !_playerController.IsGrounded() 
                      || PotionCount <= 0
                      || (_playerMove != null && _playerMove.IsRolling))
            return;

        StartDrink();
    }

    private void StartDrink()
    {
        IsDrinking = true;
        PotionCount--;
        Debug.Log($"물약 남은 개수 : {PotionCount}");

        _animator.SetTrigger(AnimatorHash.UseItem);
    }

    public void ResetItems()
    {
        PotionCount = MaxPotionCount;
    }

    public void OnHealEvent()
    {
        _healthpoint.Heal(_healAmount);
    }

    public void OnDrinkEnd()
    {
        IsDrinking = false;
    }

    public float MoveSpeedModifier() => IsDrinking ? _moveSpeedMultiplier : 1f;
}
