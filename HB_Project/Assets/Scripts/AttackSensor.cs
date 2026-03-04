using UnityEngine;

public class AttackSensor : MonoBehaviour
{
    [SerializeField] private float _currentDamage = 10f;
    [SerializeField] private string _targetTag;
    private bool _canDamage = false;

    public void SetDamage(float amount)
    {
        _currentDamage = amount;
    }
    
    public void EnableAttack() => _canDamage = true;
    public void DisableAttack() => _canDamage = false;

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log ($"{other.name}과 닿음");
        if (!_canDamage)
        {
            return;
        } 

        if (!other.CompareTag(_targetTag))
        {

            return;
        }

        if (other.CompareTag(_targetTag))
        {
            IDamagable damagable = other.GetComponent<IDamagable>();
            if (damagable != null)
            {
                Debug.Log ($"{other.name}에게 {_currentDamage}");
                damagable.TakeDamage(_currentDamage);

                _canDamage = false;
            } 
        }
    }
}
