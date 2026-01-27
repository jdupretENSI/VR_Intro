using System.Collections.Generic;
using Behaviour_Tree.Nodes;

namespace Behaviour_Tree
{
    /// <summary>
    /// Simple context passed during tree execution
    /// </summary>
    public class TickContext
    {
        public float DeltaTime { get; set; }
        public int FrameCount { get; set; }
        
        // Optional: Store which nodes are currently running
        private HashSet<NodeBase> _runningNodes = new HashSet<NodeBase>();
        
        public TickContext(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
        
        public void MarkNodeRunning(NodeBase node)
        {
            _runningNodes.Add(node);
        }
        
        public void MarkNodeCompleted(NodeBase node)
        {
            _runningNodes.Remove(node);
        }
        
        public bool IsNodeRunning(NodeBase node)
        {
            return _runningNodes.Contains(node);
        }
    }
}