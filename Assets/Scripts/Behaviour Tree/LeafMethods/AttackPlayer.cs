using Behaviour_Tree.Blackboard;
using Behaviour_Tree.Nodes;
using UnityEngine;

namespace Behaviour_Tree.LeafMethods
{
    public class AttackPlayer : NodeLeaf
    {
        public AttackPlayer(Blackboard.Blackboard blackboard, GameObject gameObject) : base(blackboard, gameObject) { }

        public override NodeReturnType Execute()
        {
            BlackboardKey playerKey = _blackboard.GetOrRegisterKey("Player");

            if (!_blackboard.TryGetValue(playerKey, out GameObject player))
                return NodeReturnType.Failure;

            // Try to get the IDamageable component from the player
            MonoBehaviour damageableComponent = player.GetComponent<MonoBehaviour>();
            
            // Check if the component implements IDamageable
            // We'll use a simple interface check - you'll need to implement IDamageable interface
            if (damageableComponent is not IDamageable damageable) return NodeReturnType.Failure;
            
            // Apply damage to the player
            damageable.TakeDamage(10f); // You can adjust damage value
                
            // Attack completed successfully
            return NodeReturnType.Success;

            // If no IDamageable component found, return failure
        }
    }
}