using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveToTarget", story: "Determine the distance between [Self] and [Target] with [MoveSpeed]", category: "Action", id: "cae4d1d9bc52ccf4d6ac5d6d38250330")]
public partial class MoveToTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> MoveSpeed;
    [SerializeReference] public BlackboardVariable<float> AttackRange;
    [SerializeReference] public BlackboardVariable<float> DetectRange;

    private NavMeshAgent _agent;

    protected override Status OnStart()
    {
        if (Self.Value == null || Target.Value == null)
        {
            Debug.Log("초기화 체크");
            return Status.Failure;    
        } 

        _agent = Self.Value.GetComponent<NavMeshAgent>();

        if(_agent == null)
        {
            Debug.Log("NavMeshAgent체크");
            return Status.Failure;
        }

        Debug.Log("대상에게 이동");
        _agent.speed = MoveSpeed.Value;
        _agent.isStopped = false;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {

        if (Target.Value == null)
        {
            return Status.Failure;
        }

        float distance = Vector3.Distance(Self.Value.transform.position, Target.Value.transform.position);

        if (distance <= AttackRange.Value)
        {
            Debug.Log("공격 범위");
            _agent.isStopped = true;
            return Status.Success;
        }

        if(distance > DetectRange.Value)
        {
            Debug.Log("사거리 밖으로 벗어남");
            _agent.isStopped = true;
            return Status.Failure;   
        }

        _agent.SetDestination(Target.Value.transform.position);

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (_agent != null && _agent.isOnNavMesh)
        {
            _agent.isStopped = true;
        }
    }
}

