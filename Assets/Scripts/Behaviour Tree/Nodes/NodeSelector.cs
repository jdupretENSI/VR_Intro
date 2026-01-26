namespace Nodes
{
    /// <summary>
    /// Selector nodes act as OR Gates, so as long as 1 of its children returns true then it will as well
    /// </summary>
    public class NodeSelector : NodeControl
    {
        public override NodeReturnType Execute()
        {
            foreach (NodeBase child in _childNodes )
            {
                switch (child.Execute())
                {
                    case NodeReturnType.Success:
                        return NodeReturnType.Success;
                    case NodeReturnType.Running:
                        return NodeReturnType.Running;
                }
            }
            return NodeReturnType.Failure;
        }
    }
}