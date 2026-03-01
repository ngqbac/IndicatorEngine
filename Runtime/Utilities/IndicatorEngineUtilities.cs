using System;
using System.Collections.Generic;
using IndicatorEngine.Core;

namespace IndicatorEngine.Utilities
{
    public static class IndicatorEngineUtilities
    {
        private const string IdConnector = "|";
        
        public static IndicatorId Child(this IndicatorId id, string segment) => new($"{id}{IdConnector}{segment}");
        
        public static List<IndicatorId> GetHierarchy(this IndicatorId id)
        {
            var s = id.ToString();
            if (string.IsNullOrEmpty(s)) return new List<IndicatorId>(0);

            var stack = new Stack<IndicatorId>(8);

            while (true)
            {
                stack.Push(new IndicatorId(s));
                var last = s.LastIndexOf(IdConnector, StringComparison.Ordinal);
                if (last < 0) break;
                s = s[..last];
            }

            var result = new List<IndicatorId>(stack.Count);
            while (stack.Count > 0) result.Add(stack.Pop());
            return result;
        }
    }
}