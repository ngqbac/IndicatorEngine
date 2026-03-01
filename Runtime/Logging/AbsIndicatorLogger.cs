using System.Collections.Generic;
using System.Text;
using IndicatorEngine.Utilities;
using IndicatorEngine.Core;

namespace IndicatorEngine.Logging
{
    public abstract class AbsIndicatorLogger
    {
        public abstract bool IsEnabled { get; }

        protected readonly HashSet<IndicatorId> Focus = new();
        
        public void AddFocus(IndicatorId id) { if (id.IsValid) Focus.Add(id); }
        public void RemoveFocus(IndicatorId id) { Focus.Remove(id); }
        public void ClearFocus() => Focus.Clear();
        
        public void Log(string message)
        {
            if (!IsEnabled) return;
            OnLog($"{Prefix()} {message}");
        }

        public void Log(string message, params IndicatorId[] ids)
        {
            if (!ShouldLog(ids)) return;
            var ctx = FormatNodes(ids);
            OnLog($"{Prefix()} {ctx} {message}");
        }

        private bool ShouldLog(IndicatorId[] ids)
        {
            if (IsEnabled) return true;
            if (ids == null) return false;
            foreach (var id in ids)
            {
                if (id.IsValid && IsFocused(id)) return true;
            }

            return false;
        }

        private bool IsFocused(IndicatorId id)
        {
            foreach (var hierarchyId in id.GetHierarchy())
            {
                if (Focus.Contains(hierarchyId)) return true;
            }
            return false;
        }
        
        protected virtual string FormatNodes(IndicatorId[] ids)
        {
            if (ids == null || ids.Length == 0) return string.Empty;

            var sb = new StringBuilder(64);
            var first = true;

            foreach (var id in ids)
            {
                if (!id.IsValid) continue;

                if (!first) sb.Append(' ');
                first = false;

                sb.Append(Node(id));
            }

            return first ? string.Empty : sb.ToString();
        }
        
        protected abstract void OnLog(string message);
        protected virtual string Prefix() => "<color=orange><INDICATOR></color>";
        protected virtual string Node(IndicatorId id) => $"<color=green>[{id}]</color>";
    }
}