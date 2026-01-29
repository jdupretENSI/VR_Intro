using System.Collections;
using System.Collections.Generic;
using Behaviour_Tree.Blackboard;
using Behaviour_Tree.LeafMethods;
using Behaviour_Tree.Nodes;
using UnityEngine;

namespace Controllers
{
    public class EnemyController : MonoBehaviour
    {
        private NodeRoot _root;
        
        [Header("Tick Speed")]
        [Range(0f, 100f)]
        [SerializeField] private float _tickSpeed;
        private float _tickTimer;

        [Header("Visibility Cone")] 
        [SerializeField]
        public float Radius;
        [Range(0, 360)]
        [SerializeField]
        public float Angle;
        [SerializeField] private LayerMask _targetMask;
        [SerializeField] private LayerMask _obstructionMask;
    
        private readonly List<Transform> _waypoints = new();
    
        public static Blackboard Blackboard = new();

        private BlackboardKey _soundEventKey = Blackboard.GetOrRegisterKey("LastSoundEvent");

        private void OnEnable()
        {
          // Patrol
            BlackboardKey movementKey =  Blackboard.GetOrRegisterKey("Moving");
            Blackboard.SetValue(movementKey, false);

            foreach (GameObject waypoint in GameObject.FindGameObjectsWithTag("Waypoint"))
            {
                _waypoints.Add(waypoint.transform);
            }
            BlackboardKey waypointsKey = Blackboard.GetOrRegisterKey("Waypoints");
            Blackboard.SetValue(waypointsKey, _waypoints);
        
            // FOV
            BlackboardKey radiusKey = Blackboard.GetOrRegisterKey("Radius");
            BlackboardKey angleKey = Blackboard.GetOrRegisterKey("Angle");
            BlackboardKey targetMaskKey = Blackboard.GetOrRegisterKey("TargetMask");
            BlackboardKey obstructionMaskKey = Blackboard.GetOrRegisterKey("Obstruction Mask");
        
            Blackboard.SetValue(radiusKey, Radius);
            Blackboard.SetValue(angleKey, Angle);
            Blackboard.SetValue(targetMaskKey, _targetMask);
            Blackboard.SetValue(obstructionMaskKey, _obstructionMask);
            
            // Sounds
            EventBus.Sound += Sound;
        }

        private void Sound(Transform obj)
        {
            StartCoroutine(SoundCoroutine(obj));
        }

        private IEnumerator SoundCoroutine(Transform obj)
        {
            Blackboard.SetValue(_soundEventKey, obj);
            
            yield return new WaitForSeconds(10f);
            
            Blackboard.Remove(_soundEventKey);
        }

        private void Start()
        {
            // Add first node to root node
            _root = new NodeRoot();
            NodeSelector tree = new();
            _root.SetChild(tree);

            // Part on Chase
            NodeSequence aware = new();
            tree.AddChild(aware);
        
            NodeLeaf look = new IsPlayerVisible(Blackboard, this.gameObject);
            NodeLeaf inspect = new PlayerVisibilityBar(Blackboard, this.gameObject);
            NodeLeaf chase = new Chase(Blackboard, this.gameObject);
            NodeLeaf attack = new AttackPlayer(Blackboard, this.gameObject);
            aware.AddChild(look);
            aware.AddChild(inspect);
            aware.AddChild(chase);
            aware.AddChild(attack);
            
            // Hearing and Investigating Sounds
            NodeSequence investigateSound = new();
            tree.AddChild(investigateSound);

            NodeLeaf heard = new HeardSound(Blackboard, this.gameObject);
            NodeLeaf investigate = new TravelToSound(Blackboard, this.gameObject);
            NodeLeaf lookAround = new SearchArea(Blackboard, this.gameObject);
            investigateSound.AddChild(heard);
            investigateSound.AddChild(investigate);
            investigateSound.AddChild(lookAround);
            
            // Part on Footprint Investigation (medium priority)
            NodeSequence investigateSequence = new();
            tree.AddChild(investigateSequence);
    
            NodeLeaf checkFootprints = new CanSeeFootprint(Blackboard, this.gameObject);
            NodeLeaf pickFootprint = new PickFootprint(Blackboard, this.gameObject);
            NodeLeaf moveToFootprint = new MoveToFootprint(Blackboard, this.gameObject);
            NodeLeaf searchAround = new LookAround(Blackboard, this.gameObject);
            NodeLeaf determineDirection = new DetermineTrailDirection(Blackboard, this.gameObject);
            NodeLeaf followTrail = new FollowTrail(Blackboard, this.gameObject);
    
            investigateSequence.AddChild(checkFootprints);
            investigateSequence.AddChild(pickFootprint);
            investigateSequence.AddChild(moveToFootprint);
            investigateSequence.AddChild(searchAround);
            investigateSequence.AddChild(determineDirection);
            investigateSequence.AddChild(followTrail);
            
            // Part on Patrol
            NodeLeaf patrol = new Patrol(Blackboard, this.gameObject);
            tree.AddChild(patrol);

            // _root -> selector -> chase -> look
            //                            -> chaseafter
            //                   -> patrol
        }

        private void Update()
        {
            // Simple tick timer
            _tickTimer += Time.deltaTime;
            
            // Calculate time between ticks
            float timeBetweenTicks = 1f / _tickSpeed;
            
            // Execute tree on tick
            if (!(_tickTimer >= timeBetweenTicks)) return;
            
            _root.Execute();
            _tickTimer = 0f;
        }
    }
}

// - Architecture du BT (4 Points)
//      Node tree ✅
//      Blackboard ✅
//      Ticks ✅
// - Patrouille et Navigation (2 Points) ✅
//      Patrols from point to point ✅
//      Take some time at the point to loiter ✅
//
// - Perception Visuelle (3 Points)
//      View Cone ✅
//      Player Visibility Bar ✅
//      Go to last point seen
//
// - Perception Sonore (3 Points)
//      Enemy moves towards sounds based on distance and obstructions ✅
//      Multiple sounds Alert Enemy
//          Ignores new sounds
//
// - Investigation et Recherche (2 Points)
//      Footprints Enemy follows ✅
//
// - Interaction Joueur (2 Points)
//      Chase after Player ✅
//      Attacks player ✅
//
// - Cachettes et Dissimulation (2 Point)
//      Player can hide from the enemy in a closet
//
// - Apprentissage et Adaptation (2 Point)
//      Enemy stops following sounds and footprints if too many have been found
//      Enemy will expand his patrol speed, less loitering and more patrol points.