


// A Node tree consists of a Root, Control Nodes and Leafs

namespace Nodes
{
    /// <summary>
    /// This is the General wrapper to a Node.
    /// Every node will have to Execute the base method
    /// This execute can be just a logic check?
    /// </summary>
    public abstract class NodeBase
    { 
        public abstract NodeReturnType Execute();
    }
}