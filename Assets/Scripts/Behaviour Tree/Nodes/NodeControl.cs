using System.Collections.Generic;

namespace Behaviour_Tree.Nodes
{
    /// <summary>
    /// These Nodes Act as Intermediaries in the Tree
    /// You can visualise them as Logic Gates
    /// Going down the tree the Controls lead to a Leaf
    /// Upwards it leads to the root
    /// From bottom to top the root awaits a Return type
    /// </summary>
    public abstract class NodeControl : NodeBase
    {
        protected List<NodeBase> _childNodes;
    
        public void AddChild(NodeBase child)
        {
            _childNodes ??= new List<NodeBase>();
            _childNodes.Add(child);
        }
    }
}