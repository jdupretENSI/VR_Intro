using Behaviour_Tree.Blackboard;
using UnityEngine;

namespace Controllers
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private GameObject _player;
    

        private void OnEnable()
        {
            BlackboardKey playerKey = EnemyController.Blackboard.GetOrRegisterKey("Player");
            EnemyController.Blackboard.SetValue(playerKey, _player);
        }
    }
}
