using UnityEngine;

public class HealthPoint : MonoBehaviour, IDamagable
{
    public float MaxHp = 1000;
    public float _currentHp;

    private Animator _animator;
    private Collider _collider;

    private PlayerController _playerController;
    private PlayerMove _playerMove;
    private Items _items;



    void Awake()
    {
        _currentHp = MaxHp;
        _animator = GetComponent<Animator>();   
        _collider = GetComponent<Collider>();

        _playerController = GetComponent<PlayerController>();
        _playerMove = GetComponent<PlayerMove>(); 
        _items = GetComponent<Items>();
    }

    public bool IsInvincible { get; set; } = false;

    public void TakeDamage(float damage)
    {
        if (_currentHp <= 0 || IsInvincible) return;


        if(_items != null) _items.OnDrinkEnd();

        if(_playerController != null) _playerController.IsAttack = false;

        if(_playerMove != null && _playerMove.IsRolling)
        {
            _playerMove.StopAllCoroutines();
            _playerMove.ResetRollingState();
        }

        _currentHp -= damage;
        Debug.Log ($"{gameObject.name}의 체력 : {_currentHp}");

        if (_currentHp <= 0) Die();
        else if (_animator != null) _animator.SetTrigger(AnimatorHash.Hit);
    }

    public void Heal(float amount)
    {
        _currentHp = Mathf.Min(_currentHp + amount, MaxHp);
        Debug.Log($"체력 회복, 현재 체력 : {_currentHp}");
    }

    void Die()
    {
        if(_animator != null) _animator.SetTrigger(AnimatorHash.Die);
        Debug.Log($"{gameObject.name} : 쥬금");
        
        if (_collider != null) _collider.enabled = false;
    }
}
