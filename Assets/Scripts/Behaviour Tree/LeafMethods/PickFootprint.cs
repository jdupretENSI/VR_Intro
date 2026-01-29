using Behaviour_Tree.Blackboard;
using Behaviour_Tree.Nodes;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour_Tree.LeafMethods
{
    public class PickFootprint : NodeLeaf
    {
        public PickFootprint(Blackboard.Blackboard blackboard, GameObject gameObject) 
            : base(blackboard, gameObject) { }
        
        public override NodeReturnType Execute()
        {
            BlackboardKey footprintsKey = _blackboard.GetOrRegisterKey("Footprints");
            BlackboardKey selectedFootprintKey = _blackboard.GetOrRegisterKey("SelectedFootprint");
            
            if (!_blackboard.TryGetValue(footprintsKey, out List<GameObject> footprints) || 
                footprints == null)
            {
                return NodeReturnType.Failure;
            }
            
            // Find closest footprint
            GameObject closestFootprint = null;
            float closestDistance = float.MaxValue;
            
            foreach (GameObject footprint in footprints)
            {
                if (footprint == null) continue;
                
                float distance = Vector3.Distance(_gameObject.transform.position, footprint.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestFootprint = footprint;
                }
            }
            
            if (closestFootprint == null)
            {
                return NodeReturnType.Failure;
            }
            
            // Store selected footprint in blackboard
            _blackboard.SetValue(selectedFootprintKey, closestFootprint);
            return NodeReturnType.Success;
        }
    }
}