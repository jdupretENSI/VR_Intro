using Behaviour_Tree.Blackboard;
using Behaviour_Tree.Nodes;
using UnityEngine;

namespace Behaviour_Tree.LeafMethods
{
    public class IsPlayerVisible : NodeLeaf
    {
        // Optional: Cache results for performance
        private GameObject _cachedPlayer;
        private float _lastCheckTime = 0f;
        private const float CHECK_COOLDOWN = 0.2f; // Check every 0.2 seconds for performance
        
        public IsPlayerVisible(Blackboard.Blackboard blackboard, GameObject gameObject) 
            : base(blackboard, gameObject) { }

        public override NodeReturnType Execute(TickContext context)
        {
            // Optional performance optimization: Skip checks if too frequent
            if (_lastCheckTime + CHECK_COOLDOWN > Time.time)
            {
                return _lastStatus; // Return cached result
            }
            
            BlackboardKey radiusKey = _blackboard.GetOrRegisterKey("Radius");
            BlackboardKey angleKey = _blackboard.GetOrRegisterKey("Angle");
            BlackboardKey targetMaskKey = _blackboard.GetOrRegisterKey("TargetMask");
            BlackboardKey obstructionMaskKey = _blackboard.GetOrRegisterKey("Obstruction Mask");

            if (!_blackboard.TryGetValue(radiusKey, out float radius) ||
                !_blackboard.TryGetValue(angleKey, out float angle) ||
                !_blackboard.TryGetValue(targetMaskKey, out LayerMask targetMask) ||
                !_blackboard.TryGetValue(obstructionMaskKey, out LayerMask obstructionMask)) 
            {
                _lastStatus = NodeReturnType.Failure;
                return _lastStatus;
            }
        
            Collider[] rangeChecks = Physics.OverlapSphere(_gameObject.transform.position, radius, targetMask);

            if (rangeChecks.Length == 0) 
            {
                _lastStatus = NodeReturnType.Failure;
                _lastCheckTime = Time.time;
                return _lastStatus;
            }
        
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - _gameObject.transform.position).normalized;

            if (!(Vector3.Angle(_gameObject.transform.forward, directionToTarget) < angle / 2)) 
            {
                _lastStatus = NodeReturnType.Failure;
                _lastCheckTime = Time.time;
                return _lastStatus;
            }
        
            float distanceToTarget = Vector3.Distance(_gameObject.transform.position, target.position);

            bool isVisible = !Physics.Raycast(_gameObject.transform.position, directionToTarget, 
                                            distanceToTarget, obstructionMask);
            
            _lastStatus = isVisible ? NodeReturnType.Success : NodeReturnType.Failure;
            _lastCheckTime = Time.time;
            
            return _lastStatus;
        }
    }
}