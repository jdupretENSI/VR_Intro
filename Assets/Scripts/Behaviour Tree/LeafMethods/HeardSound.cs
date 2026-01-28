using Behaviour_Tree.Blackboard;
using Behaviour_Tree.Nodes;
using UnityEngine;

namespace Behaviour_Tree.LeafMethods
{
    public class HeardSound : NodeLeaf
    {
        private readonly float _hearingRange;
        private readonly LayerMask _obstructionMask;
        
        public HeardSound(Blackboard.Blackboard blackboard, GameObject gameObject) 
            : base(blackboard, gameObject) 
        {
            // Get hearing parameters from blackboard or use defaults
            BlackboardKey rangeKey = _blackboard.GetOrRegisterKey("HearingRange");
            BlackboardKey obstructionKey = _blackboard.GetOrRegisterKey("Obstruction Mask");
            
            if (!_blackboard.TryGetValue(rangeKey, out _hearingRange))
            {
                _hearingRange = 10f; // Default hearing range
            }
            
            if (!_blackboard.TryGetValue(obstructionKey, out _obstructionMask))
            {
                _obstructionMask = LayerMask.GetMask("Default"); // Default mask
            }
        }
        
        public override NodeReturnType Execute()
        {
            BlackboardKey soundEventKey = _blackboard.GetOrRegisterKey("LastSoundEvent");
            
            if (!_blackboard.TryGetValue(soundEventKey, out Transform soundEvent))
            {
                // No sound event registered
                return NodeReturnType.Failure;
            }
            
            Vector3 enemyPosition = _gameObject.transform.position;
            Vector3 soundPosition = soundEvent.position;
            float distanceToSound = Vector3.Distance(enemyPosition, soundPosition);
            
            // First check: Is sound within hearing range?
            if (distanceToSound > _hearingRange)
            {
                // Sound is too far to hear
                return NodeReturnType.Failure;
            }
            
            // Second check: Is there a clear path to the sound? (Raycast)
            Vector3 directionToSound = (soundPosition - enemyPosition).normalized;
            float distance = Mathf.Min(distanceToSound, _hearingRange);
            
            bool hasClearPath = !Physics.Raycast(
                enemyPosition, 
                directionToSound, 
                distance, 
                _obstructionMask
            );
            
            // Debug visualization
            Debug.DrawRay(enemyPosition, directionToSound * distance, 
                hasClearPath ? Color.green : Color.red, 0.1f);
            
            return hasClearPath ? NodeReturnType.Success : NodeReturnType.Failure;
        }
    }
}