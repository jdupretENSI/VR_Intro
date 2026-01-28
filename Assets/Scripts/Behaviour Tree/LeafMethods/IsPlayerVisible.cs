using Behaviour_Tree.Blackboard;
using Behaviour_Tree.Nodes;
using UnityEngine;

namespace Behaviour_Tree.LeafMethods
{
    public class IsPlayerVisible : NodeLeaf
    {
        public IsPlayerVisible(Blackboard.Blackboard blackboard, GameObject gameObject) : base(blackboard,  gameObject) { }

        public override NodeReturnType Execute()
        {
            BlackboardKey radiusKey = _blackboard.GetOrRegisterKey("Radius");
            BlackboardKey angleKey = _blackboard.GetOrRegisterKey("Angle");
            BlackboardKey targetMaskKey = _blackboard.GetOrRegisterKey("TargetMask");
            BlackboardKey obstructionMaskKey = _blackboard.GetOrRegisterKey("Obstruction Mask");
            BlackboardKey playerVisibleKey = _blackboard.GetOrRegisterKey("PlayerVisible");
            
            if (!_blackboard.TryGetValue(radiusKey, out float radius) ||
                !_blackboard.TryGetValue(angleKey, out float angle) ||
                !_blackboard.TryGetValue(targetMaskKey, out LayerMask targetMask) ||
                !_blackboard.TryGetValue(obstructionMaskKey, out LayerMask obstructionMask)) 
                return NodeReturnType.Failure;
        
        
            Collider[] rangeChecks = Physics.OverlapSphere(_gameObject.transform.position, radius, targetMask);

            if (rangeChecks.Length == 0) return NodeReturnType.Failure;
        
            Transform target = rangeChecks[0].transform; //Only looking for player so the first object in this mask is the player
            Vector3 directionToTarget = (target.position - _gameObject.transform.position).normalized;

            if (!(Vector3.Angle(_gameObject.transform.forward, directionToTarget) < angle / 2)) return NodeReturnType.Failure;
        
            float distanceToTarget = Vector3.Distance(_gameObject.transform.position, target.position);

            if (Physics.Raycast(_gameObject.transform.position, directionToTarget, distanceToTarget, obstructionMask))
                return NodeReturnType.Failure;
            
            _blackboard.SetValue(playerVisibleKey, true);
            return NodeReturnType.Success;

        }
    }
}