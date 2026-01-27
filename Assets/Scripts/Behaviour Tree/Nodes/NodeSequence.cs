using System.Linq;

namespace Behaviour_Tree.Nodes
{
    /// <summary>
    /// Sequence Node are like AND Gates, meaning all the child nodes of this one MUST return success for it to return
    /// Success as well. Any Failure will return a Failure.
    /// </summary>
    public class NodeSequence : NodeControl
    {
        public override NodeReturnType Execute(TickContext context)
        {
            if (_childNodes == null || _childNodes.Count == 0)
                return NodeReturnType.Failure;
            
            foreach (var childStatus in _childNodes.Select(child => child.Execute(context)))
            {
                switch (childStatus)
                {
                    case NodeReturnType.Failure:
                        _lastStatus = NodeReturnType.Failure;
                        return _lastStatus;
                    case NodeReturnType.Running:
                        _lastStatus = NodeReturnType.Running;
                        return _lastStatus;
                }
            }
            
            _lastStatus = NodeReturnType.Success;
            return _lastStatus;
        }
    }
}