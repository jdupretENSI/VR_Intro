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
            
            yield return new WaitForSeconds(0.5f);
            
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
            aware.AddChild(look);
            aware.AddChild(inspect);
            aware.AddChild(chase);
            
            // Hearing and Investigating Sounds
            NodeSequence investigateSound = new();
            tree.AddChild(investigateSound);

            NodeLeaf heard = new HeardSound(Blackboard, this.gameObject);
            investigateSound.AddChild(heard);

            // Part on Patrol
            NodeLeaf patrol = new Patrol(Blackboard, this.gameObject);
            tree.AddChild(patrol);

            // _root -> selector -> chase -> look
            //                            -> chaseafter
            //                   -> patrol
        }

        private void Update()
        {
            _root.Execute();
        }
    }
}

// - Architecture du BT (4 Points)
// - Patrouille et Navigation (2 Points)
//      Patrols from point to point ✅
//      Take some time at the point to loiter ✅
//
// - Perception Visuelle (3 Points)
//      View Cone ✅
//      Player Visibility Bar ✅
//      Go to last point seen
//
// - Perception Sonore (3 Points)
//      Enemy moves towards nearest sound
//      Multiple sounds Alert Enemy
//          Ignores new sounds
//
// - Investigation et Recherche (2 Points)
//      Footprints Enemy follows
//
// - Interaction Joueur (2 Points)
//      Chase after Player ✅
//      Attacks player
//
// - Cachettes et Dissimulation (2 Point)
//      Player can hide from the enemy
//
// - Apprentissage et Adaptation (2 Point)
//      Enemy stops following sounds and footprints if too many have been found
//      Enemy will expand his patrol speed, less loitering and more patrol points.