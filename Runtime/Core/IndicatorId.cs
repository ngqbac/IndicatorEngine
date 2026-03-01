using System;

namespace IndicatorEngine.Core
{
    public readonly struct IndicatorId : IEquatable<IndicatorId>
    {
        private readonly string _value;
        public bool IsValid => !string.IsNullOrWhiteSpace(_value);
        public IndicatorId(string value) => _value = value ?? string.Empty;
        public override string ToString() => _value;
        public bool Equals(IndicatorId other) => string.Equals(_value, other._value, StringComparison.Ordinal);
        public override bool Equals(object obj) => obj is IndicatorId other && Equals(other);
        public override int GetHashCode() => _value != null ? StringComparer.Ordinal.GetHashCode(_value) : 0;
        public static implicit operator IndicatorId(string value) => new(value);
        public static implicit operator string(IndicatorId id) => id._value;
    }
}