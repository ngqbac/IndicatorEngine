using IndicatorEngine.Core;

namespace IndicatorEngine.Abstractions
{
    public interface IIndicatorHost
    {
        string HostKey { get; }
        void SetState(bool state);
        void SetIndicatorId(IndicatorId id);
    }
}