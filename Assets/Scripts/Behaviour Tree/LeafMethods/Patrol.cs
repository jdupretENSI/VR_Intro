using System.Collections.Generic;
using Behaviour_Tree.Blackboard;
using Behaviour_Tree.Nodes;
using UnityEngine;

namespace Behaviour_Tree.LeafMethods
{
    public class Patrol : NodeLeaf
    {
        private enum PatrolPhase
        {
            MovingToWaypoint,
            ReachedWaypoint,    // At exact waypoint position
            LoiteringAtWaypoint // Looking around
        }
        
        // Visibility decay constants (matching PlayerVisibilityBar)
        private const float VISIBILITY_DECAY_RATE = 0.2f; // Points per second when player is not visible
        
        private PatrolPhase _currentPhase = PatrolPhase.MovingToWaypoint;
        private float _loiterTimer = 1f;
        private float _loiterDuration = 1f;
        private Vector3 _originalLookDirection;
        private float _lookAngle = 30f;
        private Transform _currentWaypoint;
        
        public Patrol(Blackboard.Blackboard blackboard, GameObject gameObject) 
            : base(blackboard, gameObject) { }
        
        public override NodeReturnType Execute()
        {
            // Decrement visibility during patrol (player not visible)
            DecrementPlayerVisibility();
            
            return _currentPhase switch
            {
                PatrolPhase.MovingToWaypoint => HandleMovingToWaypoint(),
                PatrolPhase.ReachedWaypoint => HandleReachedWaypoint(),
                PatrolPhase.LoiteringAtWaypoint => HandleLoiteringAtWaypoint(),
                _ => NodeReturnType.Failure
            };
        }
        
        /// <summary>
        /// Decrements player visibility level over time during patrol
        /// </summary>
        private void DecrementPlayerVisibility()
        {
            BlackboardKey visibilityKey = _blackboard.GetOrRegisterKey("PlayerVisibility");
            
            if (!_blackboard.TryGetValue(visibilityKey, out float currentVisibility))
            {
                currentVisibility = 0f;
            }
            
            // Decrement visibility
            currentVisibility = Mathf.Max(0f, currentVisibility - VISIBILITY_DECAY_RATE * Time.deltaTime);
            
            // Update blackboard
            _blackboard.SetValue(visibilityKey, currentVisibility);
            
            // Update suspicion state
            BlackboardKey isSuspiciousKey = _blackboard.GetOrRegisterKey("IsSuspicious");
            _blackboard.SetValue(isSuspiciousKey, currentVisibility > 0.1f);
        }
        
        /// <summary>
        /// Crux of the Leaf
        /// The enemy has a list of waypoints that he goes to from one point to the next
        /// </summary>
        private NodeReturnType HandleMovingToWaypoint()
        {
            // ... [existing HandleMovingToWaypoint code remains exactly the same] ...
            BlackboardKey waypointsKey = _blackboard.GetOrRegisterKey("Waypoints");
            BlackboardKey lastWaypointKey = _blackboard.GetOrRegisterKey("LastWaypoint");
            BlackboardKey movingKey = _blackboard.GetOrRegisterKey("Moving");

            if (!_blackboard.TryGetValue(waypointsKey, out List<Transform> waypoints)) return NodeReturnType.Failure;

            _blackboard.TryGetValue(lastWaypointKey, out Transform lastWaypoint); 
            _blackboard.TryGetValue(movingKey, out bool isMoving);
            
            // Get next waypoint if needed
            if (!isMoving)
            {
                int waypointIndex = waypoints.FindIndex(waypoint => waypoint == lastWaypoint);
                if (waypointIndex + 1 >= waypoints.Count) 
                    waypointIndex = -1;
                
                _currentWaypoint = waypoints[waypointIndex + 1];
                _blackboard.SetValue(movingKey, true);
                _blackboard.SetValue(lastWaypointKey, _currentWaypoint);
                
                // Store original look direction for loitering
                _originalLookDirection = _gameObject.transform.forward;
            }
            else
            {
                _currentWaypoint = lastWaypoint;
            }
            
            // Move enemy towards waypoint
            _gameObject.transform.position = Vector3.MoveTowards(
                _gameObject.transform.position, 
                _currentWaypoint.position, 
                Time.deltaTime * 1f
            );
            
            // Look at waypoint while moving
            _gameObject.transform.LookAt(_currentWaypoint);
            
            // Check if reached waypoint (exact position)
            if (Vector3.Distance(_gameObject.transform.position, _currentWaypoint.position) < 0.01f)
            {
                // Snap to exact position
                _gameObject.transform.position = _currentWaypoint.position;
                _gameObject.transform.LookAt(_currentWaypoint);
                
                // Move to next phase
                _currentPhase = PatrolPhase.ReachedWaypoint;
            }
            
            return NodeReturnType.Running;
        }
        
        private NodeReturnType HandleReachedWaypoint()
        {
            // Now at exact waypoint position
            // Decide whether to loiter or move to next waypoint immediately
            
            if (Random.Range(0, 10) > 3) // 70% chance to loiter
            {
                StartLoitering();
            }
            else
            {
                MoveToNextWaypoint();
            }

            // Move immediately to next waypoint
            return NodeReturnType.Success;
        }
        
        private void StartLoitering()
        {
            _loiterDuration = Random.Range(0.5f, 2.0f);
            _loiterTimer = 0f;
            _lookAngle = 0f;
            _currentPhase = PatrolPhase.LoiteringAtWaypoint;
            
            // Log for debugging
            // UnityEngine.Debug.Log($"Starting to loiter for {_loiterDuration:F2} seconds");
        }
        
        private NodeReturnType HandleLoiteringAtWaypoint()
        {
            _loiterTimer += Time.deltaTime;
            
            // Perform looking around
            LookAround();

            if (!(_loiterTimer >= _loiterDuration)) return NodeReturnType.Running;
            
            // Loitering complete, move to next waypoint
            EndLoitering();
            MoveToNextWaypoint();

            return NodeReturnType.Running;
        }
        
        /// <summary>
        /// Enemy takes a moment to look around his location
        /// </summary>
        private void LookAround()
        {
            if (!_gameObject) return;
            
            // Smooth sinusoidal looking around
            float t = _loiterTimer * 90f * Mathf.Deg2Rad;
            _lookAngle = Mathf.Sin(t) * 45f; // ±45 degrees
            
            Quaternion rotation = Quaternion.AngleAxis(_lookAngle, Vector3.up);
            _gameObject.transform.forward = rotation * _originalLookDirection;
        }
        
        private void EndLoitering()
        {
            // Reset to original look direction
            if (_gameObject && _originalLookDirection != Vector3.zero)
            {
                _gameObject.transform.forward = _originalLookDirection;
            }
        }
        
        private void MoveToNextWaypoint()
        {
            // Set moving to false so next iteration gets a new waypoint
            BlackboardKey movingKey = _blackboard.GetOrRegisterKey("Moving");
            _blackboard.SetValue(movingKey, false);
            
            // Go back to moving phase
            _currentPhase = PatrolPhase.MovingToWaypoint;
        }
    }
}