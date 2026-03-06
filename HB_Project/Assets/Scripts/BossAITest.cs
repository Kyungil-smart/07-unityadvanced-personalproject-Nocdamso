using UnityEngine;
using UnityEngine.AI;

public class BossAITest : MonoBehaviour
{
    public enum State { Idle, Move, Attack }
    [Header("속도")]
    [SerializeField, Range(0, 20)] public float RotateSpeed = 10f;

    [Header("거리 설정")]
    public float DetectRange = 10f;
    public float AttackRange = 2.0f;

    [Header("공격 딜레이")]
    public float MinAttackDelay = 1f;
    public float MaxAttackDelay = 4f;
    private float _currentAttackCooldown;
    private float _attackTimer = 0f;

    [Header("공격 판정")]
    [SerializeField] private AttackSensor _bossWeapon;

    private Transform Target;
    private NavMeshAgent _agent;
    private Animator _animator;
    private BossHealth _bossHealth;

    private State _currentState = State.Idle;
    private string _targetTag = "Player";
    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        _bossHealth = GetComponent<BossHealth>();

        RandomAttackDelay();
    }

    void Update()
    {
        if(_bossHealth != null && _bossHealth._currentHp <= 0) return;
        
        _attackTimer += Time.deltaTime;

        if(Target == null)
        {
            DetectTarget();
            return;
        }

        float distance = Vector3.Distance(transform.position, Target.position);

        switch (_currentState)
        {
            case State.Idle:
                // 공격범위 내에 플레이어가 있으면 바로 공격
                if (distance <= AttackRange)
                {
                    ChangeState(State.Attack);
                }
                // 공격범위가 아닌 탐지 범위면 추적
                else if(distance <= DetectRange)
                {
                    ChangeState(State.Move);
                } 
                break;
            
            case State.Move:
                // 공격 범위 안에 있으면 공격
                if (distance <= AttackRange)
                {
                    ChangeState(State.Attack);
                }
                // 공격 범위 밖이면 멈춤
                else if (distance > DetectRange)
                {
                    ChangeState(State.Idle);
                }
                // 둘 다 아니면 플레이어 추적
                else
                {
                    _agent.SetDestination(Target.position);
                }
                break;
            
            case State.Attack:
                // 공격중 Player 바라봄
                LookAtTarget();

                if(_attackTimer >= _currentAttackCooldown)
                {
                    // 4가지 패턴 중 랜덤하게 공격, 공격간의 딜레이 랜덤
                    RandomAttack();
                    _attackTimer = 0f;
                    RandomAttackDelay();
                }

                if (distance > AttackRange + 0.5f)
                {
                    // 공격범위 밖이면 플레이어 쪽으로 이동
                    ChangeState(State.Move);
                }
                break;
        }
    }
    
    private void DetectTarget()
    {
        GameObject detect = GameObject.FindWithTag(_targetTag);
        if (detect != null) Target = detect.transform;
    }

    private void ChangeState(State newState)
    {
        if (_currentState == newState) return;

        _currentState = newState;

        _animator.SetInteger("State", (int)newState);

        if (newState == State.Move)
        {
            _agent.isStopped = false;
        }

        else
        {
            // Move 상태가 아니라면 멈추기
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
        }
    }

    private void RandomAttack()
    {
        // 4가지 패턴 중 무작위로 공격
        int randomIndex = Random.Range(0, 4);

        _animator.SetInteger("AttackPattern", randomIndex);
        _animator.SetTrigger("Attack");
    }

    private void RandomAttackDelay()
    {
        // 공격 딜레이 설정
        _currentAttackCooldown = Random.Range(MinAttackDelay, MaxAttackDelay);
    }


    private void LookAtTarget()
    {
        if (Target == null) return;
        Vector3 direction = (Target.position - transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * RotateSpeed);
        }
    }

    public void EnableBossAttack() 
    {
        if (_bossWeapon != null) _bossWeapon.EnableAttack();
    }

    public void DisableBossAttack() 
    {
        if (_bossWeapon != null) _bossWeapon.DisableAttack();
    }

    public void SetAttackDamage(float damage)
    {
        if (_bossWeapon != null)
        {
            _bossWeapon.SetDamage(damage);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}
