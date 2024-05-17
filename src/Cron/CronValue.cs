using DG.Common;
using System;

namespace DG.Sculpt.Cron
{
    /// <summary>
    /// Represents a value in a <see cref="CronRange"/>.
    /// </summary>
    public struct CronValue : IEquatable<CronValue>
    {
        internal const string AnyIndicator = "*";

        private readonly static CronValue _any = new CronValue();

        private readonly int? _value;
        private readonly string _alternateName;

        /// <summary>
        /// The actual value of this <see cref="CronValue"/>.
        /// </summary>
        public int Value => _value.Value;

        /// <summary>
        /// Indicates if this <see cref="CronValue"/> contains a value.
        /// </summary>
        public bool HasValue => _value.HasValue;

        /// <summary>
        /// Initializes a new instance of <see cref="CronValue"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="alternateName"></param>
        public CronValue(int? value, string alternateName = null)
        {
            _value = value;
            _alternateName = alternateName;
        }

        /// <summary>
        /// Returns a default (wildcard) value.
        /// </summary>
        public static CronValue Any => _any;

        /// <summary>
        /// Returns a text representation of this value. This can be the value itself, an alternative string representation, or a wildcard (<c>*</c>)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_value == null)
            {
                return AnyIndicator;
            }
            return _alternateName ?? _value.ToString();
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Of(_value)
                .And(_alternateName);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is CronValue))
            {
                return false;
            }
            return Equals((CronValue)obj);
        }

        /// <inheritdoc />
        public bool Equals(CronValue other)
        {
            return _value == other._value
                && _alternateName == other._alternateName;
        }
    }
}
