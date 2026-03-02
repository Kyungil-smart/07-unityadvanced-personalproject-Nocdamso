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

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
    float distance = Vector3.Distance(Self.Value.transform.position, Target.Value.transform.position);

    if (distance <= AttackRange.Value)
    {
        return Status.Success;
    }

    if(distance > DetectRange.Value)
    {
        return Status.Failure;   
    }

    Self.Value.transform.position = Vector3.MoveTowards(Self.Value.transform.position, Target.Value.transform.position, MoveSpeed.Value * Time.deltaTime);

    return Status.Running;
    }

    protected override void OnEnd()
    {

    }
}

