using UnityEngine;

public interface IDamagable
{
    void TakeDamage(float damage);
    bool IsInvincible { get; set; }
}
