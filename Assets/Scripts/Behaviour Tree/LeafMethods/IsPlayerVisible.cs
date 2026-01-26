using Nodes;
using UnityEngine;

namespace Behaviour_Tree.LeafMethods
{
    public class IsPlayerVisible : NodeLeaf
    {
        public IsPlayerVisible(Blackboard blackboard) : base(blackboard) { }

        public override NodeReturnType Execute()
        {
            BlackboardKey radiusKey = _blackboard.GetOrRegisterKey("Radius");
            BlackboardKey angleKey = _blackboard.GetOrRegisterKey("Angle");
            BlackboardKey targetMaskKey = _blackboard.GetOrRegisterKey("TargetMask");
            BlackboardKey obstructionMaskKey = _blackboard.GetOrRegisterKey("Obstruction Mask");
            BlackboardKey enemyKey = _blackboard.GetOrRegisterKey("Enemy");

            if (!_blackboard.TryGetValue(radiusKey, out float radius) ||
                !_blackboard.TryGetValue(angleKey, out float angle) ||
                !_blackboard.TryGetValue(targetMaskKey, out LayerMask targetMask) ||
                !_blackboard.TryGetValue(obstructionMaskKey, out LayerMask obstructionMask) ||
                !_blackboard.TryGetValue(enemyKey, out GameObject enemy)) 
                return NodeReturnType.Failure;
        
        
            Collider[] rangeChecks = Physics.OverlapSphere(enemy.transform.position, radius, targetMask);

            if (rangeChecks.Length == 0) return NodeReturnType.Failure;
        
            Transform target = rangeChecks[0].transform; //Only looking for player so the first object in this mask is the player
            Vector3 directionToTarget = (target.position - enemy.transform.position).normalized;

            if (!(Vector3.Angle(enemy.transform.forward, directionToTarget) < angle / 2)) return NodeReturnType.Failure;
        
            float distanceToTarget = Vector3.Distance(enemy.transform.position, target.position);
           
            return !Physics.Raycast(enemy.transform.position, directionToTarget, distanceToTarget, obstructionMask) ? 
                NodeReturnType.Success : NodeReturnType.Failure;

        }
    }
}