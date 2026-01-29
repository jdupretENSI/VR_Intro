using Behaviour_Tree.Blackboard;
using Behaviour_Tree.Nodes;
using UnityEngine;

namespace Behaviour_Tree.LeafMethods
{
    public class FollowTrail : NodeLeaf
    {
        private float _followDistance = 3.0f;
        private float _currentFollowDistance = 0f;
        
        public FollowTrail(Blackboard.Blackboard blackboard, GameObject gameObject) 
            : base(blackboard, gameObject) { }
        
        public override NodeReturnType Execute()
        {
            BlackboardKey trailDirectionKey = _blackboard.GetOrRegisterKey("TrailDirection");
            
            if (!_blackboard.TryGetValue(trailDirectionKey, out Vector3 direction))
            {
                return NodeReturnType.Failure;
            }
            
            // Move in the trail direction
            Vector3 targetPosition = _gameObject.transform.position + direction * _followDistance;
            
            _gameObject.transform.position = Vector3.MoveTowards(
                _gameObject.transform.position,
                targetPosition,
                Time.deltaTime * 1f
            );
            
            _gameObject.transform.forward = direction;
            
            _currentFollowDistance += Time.deltaTime * 1f;
            
            // Check if we've followed far enough
            if (_currentFollowDistance >= _followDistance)
            {
                ResetFollow();
                return NodeReturnType.Success;
            }
            
            return NodeReturnType.Running;
        }
        
        private void ResetFollow()
        {
            _currentFollowDistance = 0f;
            
            // Clear trail direction
            BlackboardKey trailDirectionKey = _blackboard.GetOrRegisterKey("TrailDirection");
            _blackboard.SetValue(trailDirectionKey, Vector3.zero);
        }
    }
}