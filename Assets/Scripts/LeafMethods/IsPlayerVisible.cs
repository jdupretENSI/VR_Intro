using System.Collections;
using Nodes;
using UnityEngine;

public class IsPlayerVisible : NodeLeaf
{
    public IsPlayerVisible(Blackboard blackboard) : base(blackboard) { }

    public override NodeReturnType Execute()
    {
        BlackboardKey playerVisibleKey = _blackboard.GetOrRegisterKey("PlayerVisible");
        return NodeReturnType.Failure;
    }
}