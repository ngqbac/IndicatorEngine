using System;
using System.Collections.Generic;
using IndicatorEngine.Abstraction;

namespace IndicatorEngine.Core
{
    public sealed partial class IndicatorTree
    {
        public event Action<IndicatorId> Removed;
        public event Action<IndicatorId, bool> ActiveChanged;
        
        private sealed class Node
        {
            public readonly IndicatorId Id;
            public IndicatorId Parent;
            public readonly HashSet<IndicatorId> Children = new();

            public bool State;
            public int StateCount;
            public bool Muted;

            public bool Active;
            public int ActiveChildCount;

            public Node(IndicatorId id)
            {
                Id = id;
                Parent = default;
                State = false;
                StateCount = 0;
                Muted = false;
                Active = false;
            }
        }

        private readonly Dictionary<IndicatorId, Node> _nodes = new();
        
        private readonly HashSet<IndicatorId> _built = new();
        private readonly HashSet<IndicatorId> _building = new();
        private readonly Dictionary<IndicatorId, List<IIndicatorBlueprint>> _pending = new();
        
        private Node Get(IndicatorId id)
        {
            if (_nodes.TryGetValue(id, out var node)) return node;
            _nodes.Add(id, new Node(id));
            return _nodes[id];
        }
        
        /// <summary>
        /// Recomputes effective Active for id and all its ancestors until stable.
        /// Fires ActiveChanged for nodes whose Active changes.
        /// </summary>
        private void RecomputeFrom(IndicatorId id)
        {
            var currentId = id;

            while (currentId.IsValid && _nodes.TryGetValue(currentId, out var node))
            {
                var oldActive = node.Active;
                var newActive = ComputeActive(node);

                if (oldActive == newActive) break;

                node.Active = newActive;
                _logger.Log($"Update active to {newActive}", node.Id);
                
                ActiveChanged?.Invoke(node.Id, newActive);
                
                var parentId = node.Parent;
                if (!parentId.IsValid || !_nodes.TryGetValue(parentId, out var parentNode)) break;

                if (newActive)
                {
                    parentNode.ActiveChildCount++;
                }
                else
                {
                    parentNode.ActiveChildCount--;
                }

                if (parentNode.ActiveChildCount < 0) parentNode.ActiveChildCount = 0;

                _logger.Log($"Recompute active count to {parentNode.ActiveChildCount}", parentNode.Id);

                currentId = parentId;
            }
        }

        private static bool ComputeActive(Node node)
        {
            if (node.Muted) return false;
            var selfActive = node.Children.Count == 0 && IsNodeActive(node);
            return selfActive || node.ActiveChildCount > 0;
        }

        private static bool IsNodeActive(Node node) => node.State || node.StateCount > 0;

        private void RemoveSubtree(IndicatorId rootId)
        {
            if (!_nodes.ContainsKey(rootId)) return;

            var stack = new Stack<IndicatorId>();
            stack.Push(rootId);

            while (stack.Count > 0)
            {
                var id = stack.Pop();
                if (!_nodes.TryGetValue(id, out var node)) continue;

                foreach (var childId in node.Children)
                {
                    stack.Push(childId);
                }

                _logger.Log("Remove subtree", node.Id);
                node.Children.Clear();
                node.Parent = default;

                _nodes.Remove(id);
                Removed?.Invoke(id);
            }
        }
    }
}