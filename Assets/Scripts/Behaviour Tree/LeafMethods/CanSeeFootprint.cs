using Behaviour_Tree.Blackboard;
using Behaviour_Tree.Nodes;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour_Tree.LeafMethods
{
    public class CanSeeFootprint : NodeLeaf
    {
        public CanSeeFootprint(Blackboard.Blackboard blackboard, GameObject gameObject) 
            : base(blackboard, gameObject) { }
        
        public override NodeReturnType Execute()
        {
            // Get FOV parameters
            BlackboardKey radiusKey = _blackboard.GetOrRegisterKey("Radius");
            BlackboardKey angleKey = _blackboard.GetOrRegisterKey("Angle");
            BlackboardKey obstructionMaskKey = _blackboard.GetOrRegisterKey("Obstruction Mask");
            
            if (!_blackboard.TryGetValue(radiusKey, out float radius) ||
                !_blackboard.TryGetValue(angleKey, out float angle) ||
                !_blackboard.TryGetValue(obstructionMaskKey, out LayerMask obstructionMask))
            {
                return NodeReturnType.Failure;
            }
            
            // Get footprints list
            BlackboardKey footprintsKey = _blackboard.GetOrRegisterKey("Footprints");
            if (!_blackboard.TryGetValue(footprintsKey, out List<GameObject> footprints) || 
                footprints == null)
            {
                return NodeReturnType.Failure;
            }
            
            // Check if any footprint is in view cone
            foreach (GameObject footprint in footprints)
            {
                if (footprint == null) continue;
                
                Vector3 directionToFootprint = (footprint.transform.position - _gameObject.transform.position).normalized;
                float distance = Vector3.Distance(_gameObject.transform.position, footprint.transform.position);
                
                // Check view cone and obstruction
                if (Vector3.Angle(_gameObject.transform.forward, directionToFootprint) < angle / 2 &&
                    distance < radius &&
                    !Physics.Raycast(_gameObject.transform.position, directionToFootprint, distance, obstructionMask))
                {
                    return NodeReturnType.Success;
                }
            }
            
            return NodeReturnType.Failure;
        }
    }
}