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
        
        private PatrolPhase _currentPhase = PatrolPhase.MovingToWaypoint;
        private float _loiterTimer = 0f;
        private float _loiterDuration = 0f;
        private Vector3 _originalLookDirection;
        private float _lookAngle = 0f;
        private Transform _currentWaypoint;
        
        public Patrol(Blackboard.Blackboard blackboard, GameObject gameObject) 
            : base(blackboard, gameObject) { }
        
        public override NodeReturnType Execute(TickContext context)
        {
            switch (_currentPhase)
            {
                case PatrolPhase.MovingToWaypoint:
                    return HandleMovingToWaypoint(context);
                    
                case PatrolPhase.ReachedWaypoint:
                    return HandleReachedWaypoint(context);
                    
                case PatrolPhase.LoiteringAtWaypoint:
                    return HandleLoiteringAtWaypoint(context);
                    
                default:
                    return NodeReturnType.Failure;
            }
        }
        
        private NodeReturnType HandleMovingToWaypoint(TickContext context)
        {
            BlackboardKey waypointsKey = _blackboard.GetOrRegisterKey("Waypoints");
            BlackboardKey lastWaypointKey = _blackboard.GetOrRegisterKey("LastWaypoint");
            BlackboardKey enemyKey = _blackboard.GetOrRegisterKey("Enemy");
            BlackboardKey movingKey = _blackboard.GetOrRegisterKey("Moving");
            
            if (!_blackboard.TryGetValue(waypointsKey, out List<Transform> waypoints))
            {
                return NodeReturnType.Failure;
            }
            
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
            
            // ⭐ IMPORTANT CHANGE: Use context.DeltaTime instead of Time.deltaTime
            _gameObject.transform.position = Vector3.MoveTowards(
                _gameObject.transform.position, 
                _currentWaypoint.position, 
                context.DeltaTime * 1f  // Changed here
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
            
            _lastStatus = NodeReturnType.Running;
            return _lastStatus;
        }
        
        private NodeReturnType HandleReachedWaypoint(TickContext context)
        {
            // Now at exact waypoint position
            // Decide whether to loiter or move to next waypoint immediately
            
            if (Random.Range(0, 10) > 3) // 70% chance to loiter
            {
                StartLoitering();
                return NodeReturnType.Running;
            }
            else
            {
                // Move immediately to next waypoint
                MoveToNextWaypoint();
                return NodeReturnType.Running;
            }
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
        
        private NodeReturnType HandleLoiteringAtWaypoint(TickContext context)
        {
            // ⭐ IMPORTANT CHANGE: Use context.DeltaTime instead of Time.deltaTime
            _loiterTimer += context.DeltaTime;  // Changed here
            
            // Perform looking around
            PerformLookAround();
            
            if (_loiterTimer >= _loiterDuration)
            {
                // Loitering complete, move to next waypoint
                EndLoitering();
                MoveToNextWaypoint();
                _lastStatus = NodeReturnType.Running;
                return _lastStatus;
            }
            
            _lastStatus = NodeReturnType.Running;
            return _lastStatus;
        }
        
        private void PerformLookAround()
        {
            if (_gameObject == null) return;
            
            // Smooth sinusoidal looking around
            // ⭐ Note: We're using _loiterTimer which now uses context.DeltaTime
            float t = _loiterTimer * 90f * Mathf.Deg2Rad;
            _lookAngle = Mathf.Sin(t) * 45f; // ±45 degrees
            
            Quaternion rotation = Quaternion.AngleAxis(_lookAngle, Vector3.up);
            _gameObject.transform.forward = rotation * _originalLookDirection;
        }
        
        private void EndLoitering()
        {
            // Reset to original look direction
            if (_gameObject != null && _originalLookDirection != Vector3.zero)
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
        
        // Optional: Add node lifecycle methods
        protected override void OnStart(TickContext context)
        {
            base.OnStart(context);
            // Initialize patrol state
            _currentPhase = PatrolPhase.MovingToWaypoint;
        }
        
        protected override void OnStop(TickContext context)
        {
            base.OnStop(context);
            // Cleanup if needed
            _loiterTimer = 0f;
        }
    }
}