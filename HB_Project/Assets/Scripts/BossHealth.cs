using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BossHealth : HealthPoint
{
    private Collider _bossCollider;
    private NavMeshAgent _agent;
    private Vector3 _startPosition;

    protected override void Awake()
    {
        base.Awake();
        _bossCollider = GetComponent<Collider>();
        _agent = GetComponent<NavMeshAgent>();
        _startPosition = transform.position;
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

        GameSceneManager.Instance.BossDefeated();
    }

    public void ResetBoss()
    {
        _currentHp = MaxHp;

        if(_agent != null && _agent.enabled)
        {
            _agent.Warp(_startPosition);
        }
        else
        {
            transform.position = _startPosition;
        }

        if (_bossCollider != null) _bossCollider.enabled = true;
        if (_agent != null)
        {
            _agent.enabled = true;
            _agent.isStopped = false;
        }

        if (_animator != null)
        {
            _animator.Rebind();
            _animator.Update(0f);
        }
    }
}
