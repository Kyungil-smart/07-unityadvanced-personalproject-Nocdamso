using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack", story: "[Self] detetmines if [Target] is in [AttackRange]", category: "Action", id: "ce1636cca5a886b95ce4d22a00563404")]
public partial class AttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> AttackRange;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        float distance = Vector3.Distance(Self.Value.transform.position, Target.Value.transform.position);

        if (distance <= AttackRange.Value)
        {
            Debug.Log("Attack");
            return Status.Success;
        }

        return Status.Failure;
    }
    protected override void OnEnd()
    {
    }

}

