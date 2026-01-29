using Behaviour_Tree.Blackboard;
using Behaviour_Tree.Nodes;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour_Tree.LeafMethods
{
    public class DetermineTrailDirection : NodeLeaf
    {
        private float _scanRadius = 4.0f;
        
        public DetermineTrailDirection(Blackboard.Blackboard blackboard, GameObject gameObject) 
            : base(blackboard, gameObject) { }
        
        public override NodeReturnType Execute()
        {
            BlackboardKey footprintsKey = _blackboard.GetOrRegisterKey("Footprints");
            if (!_blackboard.TryGetValue(footprintsKey, out List<GameObject> footprints) || 
                footprints == null || 
                footprints.Count == 0)
            {
                return NodeReturnType.Failure;
            }
            
            Vector3 currentPosition = _gameObject.transform.position;
            
            // Count footprints in front vs behind
            int footprintsInFront = 0;
            int footprintsBehind = 0;
            
            foreach (GameObject footprint in footprints)
            {
                if (footprint == null) continue;
                
                Vector3 toFootprint = footprint.transform.position - currentPosition;
                float distance = toFootprint.magnitude;
                
                if (distance > _scanRadius) continue;
                
                // Check if footprint is in front (dot product > 0) or behind
                float dot = Vector3.Dot(_gameObject.transform.forward, toFootprint.normalized);
                
                if (dot > 0.3f) // In front (with some tolerance)
                    footprintsInFront++;
                else if (dot < -0.3f) // Behind
                    footprintsBehind++;
            }
            
            // Decide direction based on footprint density
            BlackboardKey trailDirectionKey = _blackboard.GetOrRegisterKey("TrailDirection");
            
            if (footprintsInFront > footprintsBehind)
            {
                // More footprints in front, keep moving forward
                _blackboard.SetValue(trailDirectionKey, _gameObject.transform.forward);
            }
            else if (footprintsBehind > footprintsInFront)
            {
                // More footprints behind, turn around
                _blackboard.SetValue(trailDirectionKey, -_gameObject.transform.forward);
            }
            else
            {
                // Equal or none, pick a random direction
                Vector3 randomDir = Random.onUnitSphere;
                randomDir.y = 0; // Keep it horizontal
                _blackboard.SetValue(trailDirectionKey, randomDir.normalized);
            }
            
            return NodeReturnType.Success;
        }
    }
}