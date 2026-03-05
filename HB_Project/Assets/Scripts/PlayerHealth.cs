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

        if(GetComponent<PlayerController>() != null) GetComponent<PlayerController>().enabled = false;

        if(GetComponent<PlayerMove>() != null) GetComponent<PlayerMove>().enabled = false;

        GetComponent<CharacterController>().enabled = false;
    }
}
