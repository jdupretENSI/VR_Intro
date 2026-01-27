using System.Linq;

namespace Behaviour_Tree.Nodes
{
    /// <summary>
    /// Selector nodes act as OR Gates, so as long as 1 of its children returns true then it will as well
    /// </summary>
    public class NodeSelector : NodeControl
    {
        public override NodeReturnType Execute(TickContext context)
        {
            if (_childNodes == null || _childNodes.Count == 0)
                return NodeReturnType.Failure;
            
            foreach (var childStatus in _childNodes.Select(child => child.Execute(context)))
            {
                switch (childStatus)
                {
                    case NodeReturnType.Success:
                        _lastStatus = NodeReturnType.Success;
                        return _lastStatus;
                    case NodeReturnType.Running:
                        _lastStatus = NodeReturnType.Running;
                        return _lastStatus;
                }
            }
            
            _lastStatus = NodeReturnType.Failure;
            return _lastStatus;
        }
    }
}