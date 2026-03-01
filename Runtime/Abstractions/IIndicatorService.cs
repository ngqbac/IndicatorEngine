using IndicatorEngine.Core;

namespace IndicatorEngine.Abstractions
{
    public interface IIndicatorService
    {
        void Bind(IIndicatorHost host, IndicatorId id);
        void Unbind(IIndicatorHost host);
        void UnbindById(IndicatorId id);

        void SetState(IndicatorId id, bool state);
        void SetStateCount(IndicatorId id, int count);
        void UpdateStateCount(IndicatorId id, int delta);
        void RefreshFromRoot(IndicatorId id);

        void Reparent(IndicatorId child, IndicatorId parent);
        void Prune(IndicatorId id);

        bool GetState(IndicatorId id);
        int GetStateCount(IndicatorId id);
    }
}