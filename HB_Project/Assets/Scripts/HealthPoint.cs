using UnityEngine;

public class HealthPoint : MonoBehaviour, IDamagable
{
    [SerializeField] public float MaxHp = 100;
    private float _currentHp;
    private Animator _animator;

    void Awake()
    {
        _currentHp = MaxHp;
        _animator = GetComponent<Animator>();    
    }

    public void TakeDamage(float damage)
    {
        if (_currentHp <= 0) return;

        _currentHp -= damage;
        Debug.Log ($"{gameObject.name}의 체력 : {_currentHp}");

        if (_currentHp <= 0) Die();
        else if (_animator != null) _animator.SetTrigger("Hit");
    }

    void Die()
    {
        if(_animator != null) _animator.SetTrigger("Die");
        Debug.Log($"{gameObject.name} : 쥬금");
        
        GetComponent<Collider>().enabled = false;
    }
}
