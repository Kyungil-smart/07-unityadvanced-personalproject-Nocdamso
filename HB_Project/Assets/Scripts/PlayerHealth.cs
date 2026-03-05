using UnityEngine;

public class PlayerHealth : HealthPoint
{
    private PlayerController _playerController;
    private PlayerMove _playerMove;
    private Items _items;

    public bool IsDead { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        _playerController = GetComponent<PlayerController>();
        _playerMove = GetComponent<PlayerMove>();
        _items = GetComponent<Items>();
    }

    public override void TakeDamage(float damage)
    {
        if(_currentHp <= 0 || IsInvincible) return;

        if (_playerController != null) _playerController.IsAttack = false;

        if(_items != null) _items.OnDrinkEnd();

        if(_playerController != null && _playerMove.IsRolling)
        {
            _playerMove.StopAllCoroutines();
            _playerMove.ResetRollingState();
        }

        base.TakeDamage(damage);
    }

    protected override void Die()
    {
        if(IsDead) return;
        IsDead = true;

        base.Die();

        _playerController.enabled = false;
        _playerMove.enabled = false;

        GameSceneManager.Instance.PlayerDied();
    }

    public void ResetHealth()
    {
        IsDead = false;
        _currentHp = MaxHp;
        IsInvincible = false;

        if(_items != null) _items.OnDrinkEnd();

        if(_animator != null)
        {
            _animator.Rebind();
            _animator.Update(0f);
        }
    }
}
