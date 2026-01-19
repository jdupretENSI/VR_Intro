namespace Nodes
{
    /// <summary>
    /// The root just exits as a Reference to the tree
    /// Could also be used for debug
    /// </summary>
    public class NodeRoot : NodeBase
    {
        private NodeBase _child;  // Only one child
    
        public override NodeReturnType Execute()
        {
            // Simply delegate to child
            return _child?.Execute() ?? NodeReturnType.Failure;
        }
    
        // Special root-only methods
        public void SetChild(NodeBase child) { _child = child; }
    }
}