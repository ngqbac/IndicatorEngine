using IndicatorEngine.Core;

namespace IndicatorEngine.Abstraction
{
    public interface IIndicatorBlueprint
    {
        bool TryCompose(IndicatorContext ctx);
        void Refresh(IndicatorContext ctx);
    }
}