using Behaviour_Tree.Blackboard;
using Behaviour_Tree.Nodes;
using UnityEngine;

namespace Behaviour_Tree.LeafMethods
{
    public class Chase : NodeLeaf
    {
        // Could probably put these in the dictionary to increase them maybe if the enemy is agitated or something
        private readonly float _chaseSpeed = 1f;
        private readonly float _reachedDistance = 0.1f;
        
        public Chase(Blackboard.Blackboard blackboard, GameObject gameObject) 
            : base(blackboard, gameObject) { }
        
        public Chase(Blackboard.Blackboard blackboard, GameObject gameObject, float speed, float reachedDistance) 
            : base(blackboard, gameObject)
        {
            _chaseSpeed = speed;
            _reachedDistance = reachedDistance;
        }

        public override NodeReturnType Execute(TickContext context)
        {
            BlackboardKey playerKey = _blackboard.GetOrRegisterKey("Player");

            if (!_blackboard.TryGetValue(playerKey, out GameObject player)) 
                return NodeReturnType.Failure;
            
            // Use context.DeltaTime instead of Time.deltaTime
            _gameObject.transform.position = Vector3.MoveTowards(
                _gameObject.transform.position, 
                player.transform.position, 
                context.DeltaTime * _chaseSpeed
            );
            
            _gameObject.transform.LookAt(player.transform);

            // Check if reached player
            float distance = Vector3.Distance(_gameObject.transform.position, player.transform.position);
            
            _lastStatus = distance < _reachedDistance ? NodeReturnType.Success : NodeReturnType.Running;
            
            return _lastStatus;
        }
    }
}