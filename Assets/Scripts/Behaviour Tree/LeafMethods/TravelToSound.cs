using Behaviour_Tree.Blackboard;
using Behaviour_Tree.Nodes;
using UnityEngine;

public class TravelToSound : NodeLeaf
{
    private Vector3? _targetSoundPosition;
    
    public TravelToSound(Blackboard blackboard, GameObject enemy) : base(blackboard, enemy) 
    {
    }

    public override NodeReturnType Execute()
    {
        BlackboardKey soundEventKey = _blackboard.GetOrRegisterKey("LastSoundEvent");
            
        if (!_blackboard.TryGetValue(soundEventKey, out Transform soundEvent))return NodeReturnType.Failure;
        
        _targetSoundPosition = soundEvent.position + Vector3.up/3;
        
        if (!_targetSoundPosition.HasValue) return NodeReturnType.Failure;
        
        // Move toward sound position
        _gameObject.transform.position = Vector3.MoveTowards(
            _gameObject.transform.position,
            _targetSoundPosition.Value,
            Time.deltaTime * 2f // Slightly faster than patrol
        );
            
        _gameObject.transform.LookAt(_targetSoundPosition.Value);
            
        // Check if we've reached the sound position
        float distance = Vector3.Distance(_gameObject.transform.position, _targetSoundPosition.Value);

        if (!(distance < 0.5f)) return NodeReturnType.Running;
        
        // Reached the sound location
        _targetSoundPosition = null;
                
        return NodeReturnType.Success;
    }
}