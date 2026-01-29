using Behaviour_Tree.Blackboard;
using UnityEngine;

namespace Controllers
{
    public class PlayerController : MonoBehaviour, IDamageable
    {
        [SerializeField] private GameObject _player;
        [SerializeField] private float _health;
        [SerializeField] private GameObject _footprintPrefab; // Add this in Inspector
        [SerializeField] private float _footprintDropDistance = 1.0f;
    
        private Vector3 _lastFootprintPosition;
        private float _distanceTraveled;

        private void OnEnable()
        {
            BlackboardKey playerKey = EnemyController.Blackboard.GetOrRegisterKey("Player");
            EnemyController.Blackboard.SetValue(playerKey, _player);
            
            // Initialize footprint tracking
            _lastFootprintPosition = _player.transform.position;
            _distanceTraveled = 0f;
        }

        private void Update()
        {
            if (!_footprintPrefab) return;
            
            // Calculate distance since last footprint
            Vector3 currentPosition = _player.transform.position;
            _distanceTraveled += Vector3.Distance(_lastFootprintPosition, currentPosition);
            
            // Drop footprint every meter
            if (_distanceTraveled >= _footprintDropDistance)
            {
                DropFootprint(currentPosition);
                _distanceTraveled = 0f;
            }
            
            _lastFootprintPosition = currentPosition;
        }

        private void DropFootprint(Vector3 position)
        {
            // Instantiate footprint at player's position
            GameObject footprint = Instantiate(
                _footprintPrefab, 
                position + Vector3.down/2, 
                Quaternion.identity
            );
            
            // Optional: Add to blackboard for enemy tracking
            BlackboardKey footprintsKey = EnemyController.Blackboard.GetOrRegisterKey("Footprints");
            
            if (EnemyController.Blackboard.TryGetValue(footprintsKey, out System.Collections.Generic.List<GameObject> footprints))
            {
                footprints.Add(footprint);
            }
            else
            {
                footprints = new System.Collections.Generic.List<GameObject> { footprint };
                EnemyController.Blackboard.SetValue(footprintsKey, footprints);
            }
            
            // Optional: Add footprint lifetime
            Destroy(footprint, 10f); // Footprints disappear
        }
    

        public void TakeDamage(float damage)
        {
            _health -= damage;
        }
    }
}
