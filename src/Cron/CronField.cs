using DG.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DG.Sculpt.Cron
{
    public sealed class CronField : IEquatable<CronField>
    {
        private readonly CronValue _value;
        private readonly CronValue _rangeUntil;
        private readonly int? _step;
        private readonly List<CronValue> _otherValues;

        public bool IsAny => !_value.HasValue && !_step.HasValue;

        /// <summary>
        /// Initializes a new instance of a <see cref="CronField"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rangeUntil"></param>
        /// <param name="step"></param>
        /// <param name="otherValues"></param>
        public CronField(CronValue value, CronValue rangeUntil, int? step, List<CronValue> otherValues)
        {
            _value = value;
            _rangeUntil = rangeUntil;
            _step = step;

            if (otherValues != null && otherValues.Any())
            {
                _otherValues = otherValues;
            }
        }

        /// <summary>
        /// Returns a text representation of this field.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = _value.ToString();
            if (IsAny)
            {
                return result;
            }
            if (_otherValues != null && _otherValues.Any())
            {
                result += "," + string.Join(",", _otherValues);
            }
            if (_rangeUntil.HasValue)
            {
                result += "-" + _rangeUntil.ToString();
            }
            if (_step.HasValue)
            {
                result += "/" + _step;
            }
            return result;
        }

        public static bool TryParse(string value, CronValueParser parser, out CronField result)
        {
            if (value == "*")
            {
                result = new CronField(CronValue.Any, CronValue.Any, null, null);
                return true;
            }

            result = null;
            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hash = HashCode.Of(_value)
                .And(_rangeUntil)
                .And(_step);
            if (_otherValues != null)
            {
                hash = hash.AndEach(_otherValues);
            }
            return hash;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as CronField);
        }

        /// <inheritdoc/>
        public bool Equals(CronField other)
        {
            return other != null
                && _value.Equals(other._value)
                && _rangeUntil.Equals(other._rangeUntil)
                && _step == other._step
                && ((_otherValues == null && other._otherValues == null) || _otherValues.SequenceEqual(other._otherValues));
        }
    }
}
