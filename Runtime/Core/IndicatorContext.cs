using System;

namespace IndicatorEngine.Core
{
    public sealed class IndicatorContext
    {
        public IndicatorTree Tree { get; }
        private readonly Func<Type, object> _resolve;

        public IndicatorContext(IndicatorTree tree, Func<Type, object> resolve)
        {
            Tree = tree;
            _resolve = resolve;
        }

        public T GetInstance<T>() => (T)_resolve(typeof(T));
        public object GetInstance(Type t) => _resolve(t);
    }
}