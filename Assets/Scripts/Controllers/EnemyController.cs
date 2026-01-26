using System.Collections.Generic;
using Behaviour_Tree.LeafMethods;
using Nodes;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private NodeRoot _root;

    [Header("Visibility Cone")] 
    [SerializeField]
    public float _radius;
    [Range(0, 360)]
    [SerializeField]
    public float _angle;
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask _obstructionMask;
    [SerializeField] private GameObject _enemy;
    
    private readonly List<Transform> _waypoints = new();
    
    private readonly Blackboard _blackboard = new();

    private void OnEnable()
    {
        // Patrol
        BlackboardKey movementKey =  _blackboard.GetOrRegisterKey("Moving");
        _blackboard.SetValue(movementKey, false);

        foreach (GameObject waypoint in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            _waypoints.Add(waypoint.transform);
        }
        BlackboardKey waypointsKey = _blackboard.GetOrRegisterKey("Waypoints");
        _blackboard.SetValue(waypointsKey, _waypoints);
        
        // FOV
        BlackboardKey radiusKey = _blackboard.GetOrRegisterKey("Radius");
        BlackboardKey angleKey = _blackboard.GetOrRegisterKey("Angle");
        BlackboardKey targetMaskKey = _blackboard.GetOrRegisterKey("TargetMask");
        BlackboardKey obstructionMaskKey = _blackboard.GetOrRegisterKey("Obstruction Mask");
        BlackboardKey enemyKey = _blackboard.GetOrRegisterKey("Enemy");
        
        _blackboard.SetValue(radiusKey, _radius);
        _blackboard.SetValue(angleKey, _angle);
        _blackboard.SetValue(targetMaskKey, _targetMask);
        _blackboard.SetValue(obstructionMaskKey, _obstructionMask);
        _blackboard.SetValue(enemyKey, _enemy);
    }

    private void Start()
    {
        // Add first node to root node
        _root = new NodeRoot();
        NodeSelector tree = new NodeSelector();
        _root.SetChild(tree);

        // Part on Chase
        NodeSequence aware = new NodeSequence();
        tree.AddChild(aware);
        
        NodeLeaf look = new IsPlayerVisible(_blackboard);
        NodeLeaf chase = new Chase(_blackboard);
        aware.AddChild(look);
        aware.AddChild(chase);

        // Part on Patrol
        NodeLeaf patrol = new Patrol(_blackboard);
        tree.AddChild(patrol);

        // _root -> selector -> a -> aa
        //                        -> ab
        //                   -> bb
    }

    private void Update()
    {
        _root.Execute();
    }
}