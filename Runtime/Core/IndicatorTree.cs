using IndicatorEngine.Abstraction;

namespace IndicatorEngine.Core
{
    public sealed partial class IndicatorTree
    {
        private readonly AbsIndicatorLogger _logger;

        public IndicatorTree(AbsIndicatorLogger logger)
        {
            _logger = logger;
        }

        public bool GetState(IndicatorId id) => _nodes.TryGetValue(id, out var node) && node.Active;
        public int GetStateCount(IndicatorId id) => _nodes.TryGetValue(id, out var node) ? node.StateCount : 0;

        public void SetState(IndicatorId id, bool state)
        {
            if (!id.IsValid) return;
            
            var node = Get(id);
            if (node.State == state) return;

            node.State = state;
            _logger.Log($"Set state to {state}", id);
            RecomputeFrom(node.Id);
        }
        
        /// <summary>
        /// Sets a counter source for a node. Active if count > 0.
        /// Passing <= 0 clears it (treat as 0).
        /// </summary>
        public void SetStateCount(IndicatorId id, int count)
        {
            if (!id.IsValid) return;

            if (count < 0) count = 0;

            var node = Get(id);
            if (node.StateCount == count) return;

            node.StateCount = count;
            _logger.Log($"Set state count to {count}", id);
            
            RecomputeFrom(node.Id);
        }
        
        public void UpdateStateCount(IndicatorId id, int delta)
        {
            if (!id.IsValid) return;
            if (delta == 0) return;

            var node = Get(id);

            var next = node.StateCount + delta;
            if (next < 0) next = 0;

            if (node.StateCount == next) return;

            node.StateCount = next;
            RecomputeFrom(node.Id);
        }
        
        public void SetMuted(IndicatorId id, bool muted)
        {
            if (!id.IsValid) return;

            var node = Get(id);
            if (node.Muted == muted) return;

            node.Muted = muted;
            _logger.Log($"Set muted {muted}", id);
            
            RecomputeFrom(node.Id);
        }
        
        public void Reparent(IndicatorId child, IndicatorId parent)
        {
            if (!child.IsValid || !parent.IsValid) return;
            if (child.Equals(parent)) return;

            var childNode = Get(child);
            if (childNode.Parent.Equals(parent)) return;

            var oldParentId = childNode.Parent;
            var newParentNode = Get(parent);
            
            if (oldParentId.IsValid && _nodes.TryGetValue(oldParentId, out var oldParentNode))
            {
                oldParentNode.Children.Remove(child);
                if (childNode.Active) oldParentNode.ActiveChildCount--;
                if (oldParentNode.ActiveChildCount < 0) oldParentNode.ActiveChildCount = 0;
            }
            
            childNode.Parent = parent;
            newParentNode.Children.Add(child);
            if (childNode.Active) newParentNode.ActiveChildCount++;
            _logger.Log("Reparent {1} from {3} to {2}", child, parent, oldParentId);
            
            if (oldParentId.IsValid) RecomputeFrom(oldParentId);
            RecomputeFrom(parent);
        }
        
        /// <summary>
        /// Deletes node and its subtree.
        /// </summary>
        public void Prune(IndicatorId id)
        {
            if (!id.IsValid) return;
            if (!_nodes.TryGetValue(id, out var rootNode)) return;

            var parentId = rootNode.Parent;
            
            if (parentId.IsValid && _nodes.TryGetValue(parentId, out var parentNode))
            {
                parentNode.Children.Remove(id);
                if (rootNode.Active) parentNode.ActiveChildCount--;
                if (parentNode.ActiveChildCount < 0) parentNode.ActiveChildCount = 0;
            }

            rootNode.Parent = default;
            _logger.Log("Prune subtree", id);

            RemoveSubtree(id);

            if (parentId.IsValid) RecomputeFrom(parentId);
        }
    }
}