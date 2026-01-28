using Behaviour_Tree.Blackboard;
using Behaviour_Tree.Nodes;
using UnityEngine;

namespace Behaviour_Tree.LeafMethods
{
    public class PlayerVisibilityBar : NodeLeaf
    {
        // Constants for visibility calculation
        private const float MAX_VISIBILITY_DISTANCE = 10f;
        private const float MIN_VISIBILITY_DISTANCE = 2f;
        private const float MAX_VISIBILITY_RATE = 0.5f; // Points per second at closest distance
        private const float MIN_VISIBILITY_RATE = 0.1f; // Points per second at farthest distance
        
        public PlayerVisibilityBar(Blackboard.Blackboard blackboard, GameObject gameObject) 
            : base(blackboard, gameObject) { }
        
        public override NodeReturnType Execute()
        {
            // Get the current visibility level from blackboard
            BlackboardKey visibilityKey = _blackboard.GetOrRegisterKey("PlayerVisibility");
            
            if (!_blackboard.TryGetValue(visibilityKey, out float currentVisibility))
            {
                currentVisibility = 0f;
                _blackboard.SetValue(visibilityKey, currentVisibility);
            }
            
            // Get player position
            BlackboardKey playerKey = _blackboard.GetOrRegisterKey("Player");
            if (!_blackboard.TryGetValue(playerKey, out GameObject player))
            {
                // No player reference
                UpdateSuspicionState(currentVisibility);
                return NodeReturnType.Failure;
            }
            
            // Player is visible (this leaf only runs when visible) - increase visibility based on distance
            currentVisibility = IncrementVisibilityBasedOnDistance(currentVisibility, player);
            
            // Clamp between 0 and 1
            currentVisibility = Mathf.Clamp01(currentVisibility);
            
            // Update blackboard
            _blackboard.SetValue(visibilityKey, currentVisibility);
            UpdateSuspicionState(currentVisibility);

            // Return running since this is a continuous process
            return currentVisibility < 1f ? NodeReturnType.Running : NodeReturnType.Success;
        }
        
        /// <summary>
        /// Increments visibility based on distance to player (closer = faster increase)
        /// </summary>
        private float IncrementVisibilityBasedOnDistance(float currentVisibility, GameObject player)
        {
            float distance = Vector3.Distance(_gameObject.transform.position, player.transform.position);
            
            // Calculate increase rate based on distance (closer = faster increase)
            float normalizedDistance = Mathf.Clamp01(
                (distance - MIN_VISIBILITY_DISTANCE) / 
                (MAX_VISIBILITY_DISTANCE - MIN_VISIBILITY_DISTANCE)
            );
            
            float increaseRate = Mathf.Lerp(MAX_VISIBILITY_RATE, MIN_VISIBILITY_RATE, normalizedDistance);
            return currentVisibility + increaseRate * Time.deltaTime;
        }
        
        /// <summary>
        /// Updates the suspicion state in the blackboard
        /// </summary>
        private void UpdateSuspicionState(float currentVisibility)
        {
            BlackboardKey isSuspiciousKey = _blackboard.GetOrRegisterKey("IsSuspicious");
            _blackboard.SetValue(isSuspiciousKey, currentVisibility > 0.1f);
        }
    }
}