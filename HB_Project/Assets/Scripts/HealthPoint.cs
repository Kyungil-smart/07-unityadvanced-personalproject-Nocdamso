using UnityEngine;

public class HealthPoint : MonoBehaviour, IDamagable
{
    public float MaxHp = 1000;
    public float _currentHp;

    protected Animator _animator;
    public bool IsInvincible { get; set; } = false;
   

    protected virtual void Awake()
    {
        _currentHp = MaxHp;
        _animator = GetComponent<Animator>();   
    }


    public virtual void TakeDamage(float damage)
    {
        if (_currentHp <= 0 || IsInvincible) return;

        _currentHp -= damage;
        Debug.Log ($"{gameObject.name}의 체력 : {_currentHp}");

        if (_currentHp <= 0) Die();
        else PlayerHitAnimation();
    }

    public virtual void Heal(float amount)
    {
        _currentHp = Mathf.Min(_currentHp + amount, MaxHp);
    }

    protected virtual void PlayerHitAnimation()
    {
        if (_animator != null) _animator.SetTrigger(AnimatorHash.Hit);
    }

    protected virtual void Die()
    {
        if(_animator != null) _animator.SetTrigger(AnimatorHash.Die);
        Debug.Log($"{gameObject.name} : 쥬금");
    }
}
