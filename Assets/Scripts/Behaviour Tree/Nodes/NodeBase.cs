// A Node tree consists of a Root, Control Nodes and Leafs

namespace Behaviour_Tree.Nodes
{
    /// <summary>
    /// This is the General wrapper to a Node.
    /// Every node will have to Execute the base method
    /// </summary>
    public abstract class NodeBase
    {
        // Track node state
        protected NodeReturnType _lastStatus = NodeReturnType.Failure;
        
        /// <summary>
        /// Execute node with tick context
        /// </summary>
        public abstract NodeReturnType Execute(TickContext context);
        
        /// <summary>
        /// Get last execution status (useful for debugging)
        /// </summary>
        public NodeReturnType GetLastStatus() => _lastStatus;
        
        /// <summary>
        /// Called when node starts
        /// </summary>
        protected virtual void OnStart(TickContext context) { }
        
        /// <summary>
        /// Called when node finishes
        /// </summary>
        protected virtual void OnStop(TickContext context) { }
    }
}