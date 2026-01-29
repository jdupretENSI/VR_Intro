using Behaviour_Tree.Blackboard;
using Behaviour_Tree.Nodes;
using UnityEngine;

namespace Behaviour_Tree.LeafMethods
{
    public class MoveToFootprint : NodeLeaf
    {
        private float _reachedDistance = 0.5f;
        
        public MoveToFootprint(Blackboard.Blackboard blackboard, GameObject gameObject) 
            : base(blackboard, gameObject) { }
        
        public override NodeReturnType Execute()
        {
            BlackboardKey selectedFootprintKey = _blackboard.GetOrRegisterKey("SelectedFootprint");
            
            if (!_blackboard.TryGetValue(selectedFootprintKey, out GameObject footprint) || 
                footprint == null)
            {
                return NodeReturnType.Failure;
            }
            
            // Move towards the footprint
            _gameObject.transform.position = Vector3.MoveTowards(
                _gameObject.transform.position,
                footprint.transform.position,
                Time.deltaTime * 1f
            );
            
            _gameObject.transform.LookAt(footprint.transform);
            
            // Check if reached
            if (Vector3.Distance(_gameObject.transform.position, footprint.transform.position) < _reachedDistance)
            {
                // Clean up this footprint
                CleanupFootprint(footprint);
                return NodeReturnType.Success;
            }
            
            return NodeReturnType.Running;
        }
        
        private void CleanupFootprint(GameObject footprint)
        {
            // Remove from footprints list
            BlackboardKey footprintsKey = _blackboard.GetOrRegisterKey("Footprints");
            if (_blackboard.TryGetValue(footprintsKey, out System.Collections.Generic.List<GameObject> footprints))
            {
                footprints.Remove(footprint);
            }
            
            // Clear selected footprint
            BlackboardKey selectedFootprintKey = _blackboard.GetOrRegisterKey("SelectedFootprint");
            _blackboard.Remove(selectedFootprintKey);
            
            // Destroy the GameObject
            GameObject.Destroy(footprint);
        }
    }
}