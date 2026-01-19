using Nodes;
using UnityEngine;

public class Chase : NodeLeaf
{
    public Chase(Blackboard blackboard) : base(blackboard) { }

    public override NodeReturnType Execute()
    {
        Debug.Log("Chase");
        return NodeReturnType.Failure;
    }
}