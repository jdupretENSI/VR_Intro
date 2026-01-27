namespace Behaviour_Tree.Nodes
{
    /// <summary>
    /// The root just exits as a Reference to the tree
    /// Could also be used for debug
    /// </summary>
    public class NodeRoot : NodeBase
    {
        private NodeBase _child;
        
        public override NodeReturnType Execute(TickContext context)
        {
            if (_child == null)
                return NodeReturnType.Failure;
            
            // Simply delegate to child with context
            _lastStatus = _child.Execute(context);
            return _lastStatus;
        }
        
        public void SetChild(NodeBase child) { _child = child; }
    }
}