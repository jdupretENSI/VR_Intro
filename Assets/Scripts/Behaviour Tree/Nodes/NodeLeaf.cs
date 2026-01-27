using UnityEngine;

namespace Behaviour_Tree.Nodes
{
    /// <summary>
    /// There are 2 types of leaves
    /// One that Returns a condition
    /// One that executes an Action
    /// Both return either Success, Failure or Running
    /// </summary>
    public abstract class NodeLeaf : NodeBase
    {

        protected Blackboard.Blackboard _blackboard;
        protected GameObject _gameObject;

        protected NodeLeaf(Blackboard.Blackboard blackboard, GameObject  gameObject)
        {
            _blackboard = blackboard;
            _gameObject = gameObject;
        }
    }
}