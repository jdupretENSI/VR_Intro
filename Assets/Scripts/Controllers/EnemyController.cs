using System.Collections.Generic;
using Nodes;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private NodeRoot _root;
    private Transform _sphere;
    private BlackboardKey _sphereKey;

    public float radius, angle;
    
    public GameObject player;
    
    public LayerMask targetMask, obstructionMask;
    
    public bool playerVisible;
    
    private readonly List<Transform> _waypoints = new();
    private BlackboardKey _waypointsKey;
    private BlackboardKey _movementKey;
    
    private readonly Blackboard _blackboard = new();

    private void OnEnable()
    {
        _sphere = this.transform;
        
        _sphereKey = _blackboard.GetOrRegisterKey("Sphere");
        _blackboard.SetValue(_sphereKey, _sphere);
        
        _movementKey =  _blackboard.GetOrRegisterKey("Moving");
        _blackboard.SetValue(_movementKey, false);

        foreach (GameObject waypoint in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            _waypoints.Add(waypoint.transform);
        }
        _waypointsKey = _blackboard.GetOrRegisterKey("Waypoints");
        _blackboard.SetValue(_waypointsKey, _waypoints);
    }

    private void Start()
    {
        // Add first node to root node
        _root = new NodeRoot();
        NodeSelector selector = new NodeSelector();
        _root.SetChild(selector);

        // Part on Chase
        NodeSequence a = new NodeSequence();
        selector.AddChild(a);
        NodeLeaf aa = new IsPlayerVisible(_blackboard);
        NodeLeaf ab = new Chase(_blackboard);
        a.AddChild(aa);
        a.AddChild(ab);

        // Part on Patrol
        NodeLeaf bb = new Patrol(_blackboard);
        selector.AddChild(bb);

        // _root -> selector -> a -> aa
        //                        -> ab
        //                   -> bb
    }

    private void Update()
    {
        _root.Execute();
    }
}