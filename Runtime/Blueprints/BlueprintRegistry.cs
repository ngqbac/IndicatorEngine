using System.Collections.Generic;
using IndicatorEngine.Abstractions;
using IndicatorEngine.Core;

namespace IndicatorEngine.Blueprints
{
    public class BlueprintRegistry
    {
        private static readonly Dictionary<IndicatorId, List<IIndicatorBlueprint>> Blueprints = new();

        public static void Reset() => Blueprints.Clear();

        public static void Register(IndicatorId id, IIndicatorBlueprint blueprint)
        {
            if (!Blueprints.TryGetValue(id, out var list)) Blueprints[id] = list = new List<IIndicatorBlueprint>();
            list.Add(blueprint);
        }

        public static IReadOnlyList<IIndicatorBlueprint> Get(IndicatorId id) => Blueprints.GetValueOrDefault(id);
    }
}