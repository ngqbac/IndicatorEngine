using IndicatorEngine.Core;

namespace IndicatorEngine.Abstractions
{
    public interface IIndicatorBlueprint
    {
        bool TryCompose(IndicatorContext ctx);
        void Refresh(IndicatorContext ctx);
    }
}