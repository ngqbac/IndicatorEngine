using IndicatorEngine.Core;

namespace IndicatorEngine.Abstractions
{
    public abstract class AbsIndicatorBlueprint : IIndicatorBlueprint
    {
        public bool TryCompose(IndicatorContext ctx)
        {
            if (!AbleToCompose(ctx)) return false;
            OnCompose(ctx);
            OnRefresh(ctx);
            return true;
        }

        public void Refresh(IndicatorContext ctx)
        {
            if (!AbleToCompose(ctx)) return;
            OnRefresh(ctx);
        }

        protected abstract bool AbleToCompose(IndicatorContext ctx);
        protected abstract void OnCompose(IndicatorContext ctx);
        protected abstract void OnRefresh(IndicatorContext ctx);
    }
}