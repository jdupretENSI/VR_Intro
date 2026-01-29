using Behaviour_Tree.Blackboard;
using Behaviour_Tree.Nodes;
using UnityEngine;

namespace Behaviour_Tree.LeafMethods
{
    public class LookAround : NodeLeaf
    {
        private float _lookTimer = 0f;
        private float _lookDuration = 1.5f;
        private float _lookAngle = 0f;
        private Vector3 _originalForward;
        
        public LookAround(Blackboard.Blackboard blackboard, GameObject gameObject) 
            : base(blackboard, gameObject) { }
        
        public override NodeReturnType Execute()
        {
            // Initialize on first call
            if (_lookTimer == 0f)
            {
                _originalForward = _gameObject.transform.forward;
            }
            
            _lookTimer += Time.deltaTime;
            
            // Perform looking around
            PerformLookAround();
            
            if (_lookTimer >= _lookDuration)
            {
                // Reset for next time
                ResetLook();
                return NodeReturnType.Success;
            }
            
            return NodeReturnType.Running;
        }
        
        private void PerformLookAround()
        {
            // Smooth sinusoidal looking (left to right)
            float t = _lookTimer / _lookDuration * Mathf.PI * 2f; // Full sine wave
            _lookAngle = Mathf.Sin(t) * 60f; // ±60 degrees
            
            Quaternion rotation = Quaternion.AngleAxis(_lookAngle, Vector3.up);
            _gameObject.transform.forward = rotation * _originalForward;
        }
        
        private void ResetLook()
        {
            _lookTimer = 0f;
            _lookAngle = 0f;
            if (_originalForward != Vector3.zero)
            {
                _gameObject.transform.forward = _originalForward;
            }
        }
    }
}