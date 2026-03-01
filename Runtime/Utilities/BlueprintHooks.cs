using System;
using System.Collections.Generic;
using IndicatorEngine.Abstraction;
using IndicatorEngine.Core;

namespace IndicatorEngine.Utilities
{
    public static class BlueprintHooks
    {
        private static Func<IndicatorId, IReadOnlyList<IIndicatorBlueprint>> _resolve;
        public static IReadOnlyList<IIndicatorBlueprint> Get(IndicatorId id) => _resolve?.Invoke(id);
        public static void Set(Func<IndicatorId, IReadOnlyList<IIndicatorBlueprint>> resolve) => _resolve = resolve;
    }
}