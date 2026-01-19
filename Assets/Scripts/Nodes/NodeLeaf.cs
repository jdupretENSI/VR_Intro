namespace Nodes
{
    /// <summary>
    /// There are 2 types of leaves
    /// One that Returns a condition
    /// One that executes an Action
    /// Both return either Success, Failure or Running
    /// </summary>
    public abstract class NodeLeaf : NodeBase
    {
        private NodeReturnType _returnType;
        protected Blackboard _blackboard;

        protected NodeLeaf(Blackboard blackboard)
        {
            _blackboard = blackboard;
        }
    }
}