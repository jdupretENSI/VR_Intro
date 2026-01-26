using System.Collections.Generic;
using Nodes;
using UnityEngine;

public class Patrol : NodeLeaf
{
    public Patrol(Blackboard blackboard) : base(blackboard) { }
    
    public override NodeReturnType Execute()
    {
        // 1. Get the key for the data you need
        BlackboardKey waypointsKey = _blackboard.GetOrRegisterKey("Waypoints");
        BlackboardKey lastWaypointKey = _blackboard.GetOrRegisterKey("LastWaypoint");
        BlackboardKey enemyKey = _blackboard.GetOrRegisterKey("Enemy");
        BlackboardKey movingKey = _blackboard.GetOrRegisterKey("Moving");
        Transform currentWaypoint;

        
        // 2. Try to get the values from blackboard
        if (!_blackboard.TryGetValue(waypointsKey, out List<Transform> waypoints) ||
            !_blackboard.TryGetValue(movingKey, out bool moving))
            return NodeReturnType.Failure;
        
        _blackboard.TryGetValue(lastWaypointKey, out Transform lastWaypoint);
        
        if (!moving)
        {
            // Get next waypoint
            int waypointIndex = waypoints.FindIndex(waypoint => waypoint == lastWaypoint);
            if (waypointIndex + 1 >= waypoints.Count) waypointIndex = -1;
            
            currentWaypoint = waypoints[waypointIndex + 1];
            
            _blackboard.SetValue(movingKey, true);
            _blackboard.SetValue(lastWaypointKey, currentWaypoint);
        }
        else
        {
            currentWaypoint = lastWaypoint;
        }
        
        if (!_blackboard.TryGetValue(enemyKey, out GameObject enemy)) return NodeReturnType.Failure;
        
        // Move Enemy towards waypoint
        enemy.transform.position = Vector3.MoveTowards(
            enemy.transform.position, 
            currentWaypoint.position, 
            Time.deltaTime * 1f
        );
        
        enemy.transform.LookAt(currentWaypoint);

        // Check if reached waypoint
        if (!(Vector3.Distance(enemy.transform.position, currentWaypoint.position) < 0.1f)) 
            return NodeReturnType.Running;
        
        _blackboard.SetValue(movingKey, false);
        return NodeReturnType.Success;

    }
}