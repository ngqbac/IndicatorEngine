using System;
using System.Collections.Generic;
using IndicatorEngine.Utilities;
using IndicatorEngine.Abstractions;
using IndicatorEngine.Blueprints;
using IndicatorEngine.Core;
using IndicatorEngine.Logging;

namespace IndicatorEngine.Visual
{
    public class IndicatorVisual
    {
        private readonly IndicatorTree _tree;
        private readonly IndicatorContext _ctx;
        private readonly AbsIndicatorLogger _logger;

        private readonly HashSet<IndicatorId> _built = new();
        private readonly HashSet<IndicatorId> _building = new();
        private readonly Dictionary<IndicatorId, List<IIndicatorBlueprint>> _pending = new();
        
        private readonly Dictionary<string, (IIndicatorHost host, IndicatorId id)> _hostToId = new();
        private readonly Dictionary<IndicatorId, (IIndicatorHost host, string hostKey)> _idToHost = new();

        public IndicatorVisual(IndicatorTree tree, IndicatorContext ctx, AbsIndicatorLogger logger)
        {
            _tree = tree;
            _ctx = ctx;
            _logger = logger;
            
            _tree.ActiveChanged += OnActiveChanged;
            _tree.Removed += OnRemoved;
        }

        public void CleanUp()
        {
            _hostToId.Clear();
            _idToHost.Clear();
        }
        
        public void Bind(IIndicatorHost host, IndicatorId id)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));

            var hostKey = host.HostKey ?? string.Empty;

            if (_hostToId.ContainsKey(hostKey)) UnbindByHostKey(hostKey);
            if (_idToHost.ContainsKey(id)) UnbindById(id);
            
            EnsureBuilt(id);
            
            _hostToId[hostKey] = (host, id);
            _idToHost[id] = (host, hostKey);
            
            host.SetIndicatorId(id);
            host.SetState(_tree.GetState(id));
            
            _logger.Log("Bind host", id);
        }

        public void Unbind(IIndicatorHost host)
        {
            if (host == null) return;
            UnbindByHostKey(host.HostKey ?? string.Empty);
        }

        public void UnbindById(IndicatorId id)
        {
            if (!_idToHost.TryGetValue(id, out var pair)) return;
            UnbindByHostKey(pair.hostKey);
            _logger.Log("Unbind host", id);
        }

        private void UnbindByHostKey(string hostKey)
        {
            if (!_hostToId.Remove(hostKey, out var b)) return;
            _idToHost.Remove(b.id);
            b.host?.SetIndicatorId(default);
            b.host?.SetState(false);
        }
        
        public void Ensure(IndicatorId id)
        {
            if (!id.IsValid) return;
            EnsureBuilt(id);
        }

        private void EnsureBuilt(IndicatorId id)
        {
            var keys = id.GetHierarchy();
            foreach (var item in keys)
            {
                EnsureBuiltInternal(item);
            }
        }
        
        private void EnsureBuiltInternal(IndicatorId id)
        {
            if (_built.Contains(id)) return;
            if (!_building.Add(id)) return;

            try
            {
                if (!_pending.TryGetValue(id, out var blueprints))
                {
                    var list = BlueprintHooks.Get(id);
                    if (list == null || list.Count == 0)
                    {
                        _built.Add(id);
                        return;
                    }

                    blueprints = new List<IIndicatorBlueprint>(list);
                    _pending[id] = blueprints;
                }

                for (var i = blueprints.Count - 1; i >= 0; i--)
                {
                    _logger.Log($"Built blueprint {i}", id);
                    if (blueprints[i].TryCompose(_ctx)) blueprints.RemoveAt(i);
                }

                if (blueprints.Count != 0) return;

                _pending.Remove(id);
                _built.Add(id);
            }
            finally
            {
                _building.Remove(id);
                _logger.Log("Built complete", id);
            }
        }

        private void OnActiveChanged(IndicatorId id, bool active)
        {
            _logger.Log("OnActiveChanged", id);
            if (_idToHost.TryGetValue(id, out var pair)) pair.host?.SetState(active);
        }

        private void OnRemoved(IndicatorId id)
        {
            UnbindById(id);
            _built.Remove(id);
            _pending.Remove(id);
            _building.Remove(id);
        }
    }
}