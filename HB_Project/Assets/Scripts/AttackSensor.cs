using UnityEngine;

public class AttackSensor : MonoBehaviour
{
    [SerializeField] public float Damage = 10f;
    private bool _canDamage = false;

    public void EnableAttack() => _canDamage = true;
    public void DisableAttack() => _canDamage = false;

    private void OnggerEnter(Collider other)
    {
        if(!_canDamage) return;

        IDamagable damagable = other.GetComponent<IDamagable>();

        if (damagable != null)
        {
            damagable.TakeDamage(Damage);

            _canDamage = false;
        }
    }
}
