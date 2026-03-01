using IndicatorEngine.Core;

namespace IndicatorEngine.Abstraction
{
    public interface IIndicatorHost
    {
        string HostKey { get; }
        void SetState(bool state);
        void SetIndicatorId(IndicatorId id);
    }
}