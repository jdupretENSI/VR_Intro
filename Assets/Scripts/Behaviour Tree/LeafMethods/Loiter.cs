using Behaviour_Tree.Nodes;
using UnityEngine;

using Behaviour_Tree.Nodes;
using UnityEngine;

namespace Behaviour_Tree.LeafMethods
{
    public class Loiter : NodeLeaf
    {
        private float _loiterTimer = 0f;
        private float _loiterDuration;
        private bool _isInitialized = false;
        private Vector3 _originalLookDirection;
        private float _lookAngle = 0f;
        private const float _lookSpeed = 90f; // Degrees per second
        private const float _lookRange = 45f; // Max angle to look around

        public Loiter(Blackboard.Blackboard blackboard, GameObject gameObject) 
            : base(blackboard, gameObject) { }
        
        public override NodeReturnType Execute()
        {
            // Initialize on first execution
            if (!_isInitialized)
            {
                Initialize();
            }
            
            // Update timer
            _loiterTimer += Time.deltaTime;
            
            // Perform looking around behavior
            PerformLookAround();
            
            // Check if loitering is complete
            if (!(_loiterTimer >= _loiterDuration)) return NodeReturnType.Running;
            
            ResetToOriginalRotation();
            return NodeReturnType.Success;

        }
        
        private void Initialize()
        {
            // Set random loiter duration between 0.5 and 2 seconds
            _loiterDuration = Random.Range(0.5f, 2.0f);
            
            // Store original look direction
            if (_gameObject)
            {
                _originalLookDirection = _gameObject.transform.forward;
            }
            
            // Reset timer
            _loiterTimer = 0f;
            
            // Set initialized flag
            _isInitialized = true;
            
            //Debug.Log($"Loitering for {_loiterDuration:F2} seconds");
        }
        
        private void PerformLookAround()
        {
            if (!_gameObject) return;
            
            // Calculate sinusoidal looking pattern
            float t = _loiterTimer * _lookSpeed * Mathf.Deg2Rad;
            _lookAngle = Mathf.Sin(t) * _lookRange;
            
            // Create new rotation
            Quaternion newRotation = Quaternion.Euler(0, _lookAngle, 0);
            
            // Apply rotation relative to original direction
            Vector3 newLookDirection = newRotation * _originalLookDirection;
            _gameObject.transform.forward = newLookDirection;
            //Debug.Log("Looking at " + _gameObject.transform.position);
        }
        
        private void ResetToOriginalRotation()
        {
            if (_gameObject && _originalLookDirection != Vector3.zero)
            {
                _gameObject.transform.forward = _originalLookDirection;
            }
            
            // Reset for next execution
            _isInitialized = false;
        }
    }
}
