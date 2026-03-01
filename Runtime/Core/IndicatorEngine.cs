using IndicatorEngine.Abstraction;
using IndicatorEngine.Utilities;

namespace IndicatorEngine.Core
{
    public class IndicatorEngine : IIndicatorEngine
    {
        private IndicatorTree IndicatorTree { get; set; }
        private IndicatorContext Context { get; set; }
        private IndicatorVisual IndicatorVisual { get; set; }
        private AbsIndicatorLogger Logger { get; set; }
        
        public IndicatorEngine(IndicatorTree indicatorTree, IndicatorContext context, AbsIndicatorLogger logger)
        {
            Logger = logger;
            IndicatorTree = indicatorTree;
            Context = context;
            IndicatorVisual = new IndicatorVisual(IndicatorTree, Context, Logger);
        }

        public void Bind(IIndicatorHost host, IndicatorId id)
        {
            if (host == null || !id.IsValid) return;
            IndicatorVisual.Bind(host, id);
        }

        public void Unbind(IIndicatorHost host)
        {
            if (host == null) return;
            IndicatorVisual.Unbind(host);
        }

        public void UnbindById(IndicatorId id)
        {
            if (!id.IsValid) return;
            IndicatorVisual.UnbindById(id);
        }

        public void SetState(IndicatorId id, bool state)
        {
            if (!id.IsValid) return;
            Ensure(id);
            IndicatorTree.SetState(id, state);
        }

        public void SetStateCount(IndicatorId id, int count)
        {
            if (!id.IsValid) return;
            Ensure(id);
            IndicatorTree.SetStateCount(id, count);
        }

        public void UpdateStateCount(IndicatorId id, int delta)
        {
            if (!id.IsValid) return;
            Ensure(id);
            IndicatorTree.UpdateStateCount(id, delta);
        }

        public void RefreshFromRoot(IndicatorId id)
        {
            if (!id.IsValid) return;
            var blueprints = BlueprintHooks.Get(id);
            foreach (var blueprint in blueprints)
            {
                blueprint.Refresh(Context);
            }
        }

        public void Reparent(IndicatorId child, IndicatorId parent)
        {
            if (!child.IsValid || !parent.IsValid) return;
            Ensure(child);
            Ensure(parent);
            IndicatorTree.Reparent(child, parent);
        }

        public void Prune(IndicatorId id)
        {
            if (!id.IsValid) return;
            IndicatorTree.Prune(id);
        }

        public bool GetState(IndicatorId id) => id.IsValid && IndicatorTree.GetState(id);
        public int GetStateCount(IndicatorId id) => id.IsValid ? IndicatorTree.GetStateCount(id) : 0;

        private void Ensure(IndicatorId id) => IndicatorVisual?.Ensure(id);
        
        public void CleanUp()
        {
            IndicatorVisual.CleanUp();
            Logger.Log("Reset visual");
        }
    }
}