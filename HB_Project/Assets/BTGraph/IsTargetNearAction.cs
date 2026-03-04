using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "IsTargetNear", story: "[Self] Checks if [Target] is within [DetectRange]", category: "Action", id: "3fcae257c6ebbd8b85b30bcf0ab13372")]
public partial class IsTargetNearAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> DetectRange;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        float distance = Vector3.Distance(Self.Value.transform.position, Target.Value.transform.position);

        if(distance <= DetectRange.Value)
        {
            return Status.Success; 
        }

        return Status.Failure; 
    }
    protected override void OnEnd()
    {
    }
}

