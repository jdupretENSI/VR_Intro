using Behaviour_Tree.Blackboard;
using Behaviour_Tree.Nodes;
using UnityEngine;

namespace Behaviour_Tree.LeafMethods
{
    public class Chase : NodeLeaf
    {
        public Chase(Blackboard.Blackboard blackboard, GameObject gameObject) : base(blackboard,  gameObject) { }

        public override NodeReturnType Execute()
        {
            BlackboardKey playerKey = _blackboard.GetOrRegisterKey("Player");

            if (!_blackboard.TryGetValue(playerKey, out GameObject player)) 
                return NodeReturnType.Failure;
        
            // Move Enemy towards player
            _gameObject.transform.position = Vector3.MoveTowards(
                _gameObject.transform.position, 
                player.transform.position, 
                Time.deltaTime * 1f
            );
        
            _gameObject.transform.LookAt(player.transform);

            // Check if reached waypoint
            return !(Vector3.Distance(_gameObject.transform.position, player.transform.position) < 0.1f) 
                ? NodeReturnType.Running 
                : NodeReturnType.Success;
        }
    }
}