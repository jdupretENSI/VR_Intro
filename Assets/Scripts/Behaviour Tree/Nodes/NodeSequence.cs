namespace Behaviour_Tree.Nodes
{
    /// <summary>
    /// Sequence Node are like AND Gates, meaning all the child nodes of this one MUST return success for it to return
    /// Success as well. Any Failure will return a Failure.
    /// </summary>
    public class NodeSequence : NodeControl
    {
        public override NodeReturnType Execute()
        {
            foreach (NodeBase child in _childNodes )
            {
                switch (child.Execute())
                {
                    case NodeReturnType.Failure:
                        return NodeReturnType.Failure;
                    case NodeReturnType.Running:
                        return NodeReturnType.Running;
                }
            }
            return NodeReturnType.Success;
        }
    }
}