using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BossHealth : HealthPoint
{
    private Collider _bossCollider;
    private NavMeshAgent _agent;

    protected override void Awake()
    {
        base.Awake();
        _bossCollider = GetComponent<Collider>();
        _agent = GetComponent<NavMeshAgent>();
    }

    protected override void Die()
    {
        base.Die();

        if (_agent != null)
        {
            _agent.isStopped = true;
            _agent.enabled = false;
        }

        if(_bossCollider != null) _bossCollider.enabled = false;
    }
}
