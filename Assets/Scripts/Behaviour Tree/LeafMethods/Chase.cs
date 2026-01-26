using Nodes;
using UnityEngine;

public class Chase : NodeLeaf
{
    public Chase(Blackboard blackboard) : base(blackboard) { }

    public override NodeReturnType Execute()
    {
        BlackboardKey playerKey = _blackboard.GetOrRegisterKey("Player");
        BlackboardKey enemyKey = _blackboard.GetOrRegisterKey("Enemy");

        if (!_blackboard.TryGetValue(playerKey, out GameObject player) || 
            !_blackboard.TryGetValue(enemyKey, out GameObject enemy)) 
            return NodeReturnType.Failure;
        
        // Move Enemy towards player
        enemy.transform.position = Vector3.MoveTowards(
            enemy.transform.position, 
            player.transform.position, 
            Time.deltaTime * 1f
        );
        
        enemy.transform.LookAt(player.transform);

        // Check if reached waypoint
        if (!(Vector3.Distance(enemy.transform.position, player.transform.position) < 0.1f)) 
            return NodeReturnType.Running;
        
        return NodeReturnType.Success;
    }
}