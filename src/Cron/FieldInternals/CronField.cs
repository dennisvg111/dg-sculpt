using DG.Common;
using DG.Sculpt.Cron.Exceptions;
using DG.Sculpt.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DG.Sculpt.Cron.FieldInternals
{
    /// <summary>
    /// Represents a single field in a <see cref="CronSchedule"/>.
    /// </summary>
    internal sealed class CronField : IReadOnlyCronField, IEquatable<CronField>
    {
        private readonly CronRange[] _ranges;
        private readonly bool _isWildcard;
        private readonly Lazy<int[]> _lazyOrderdAllowedValues;

        /// <inheritdoc/>
        public bool IsWildcard => _isWildcard;

        /// <inheritdoc/>
        public IReadOnlyList<int> AllowedValues => _lazyOrderdAllowedValues.Value;

        /// <summary>
        /// Initializes a new instance of a <see cref="CronField"/>.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="ranges"></param>
        internal CronField(int min, int max, params CronRange[] ranges)
        {
            _ranges = ranges;
            _isWildcard = CheckForWildcard();
            _lazyOrderdAllowedValues = new Lazy<int[]>(() => CalculateAllowedValues(min, max));
        }

        private bool CheckForWildcard()
        {
            return _ranges == null || _ranges.Length == 0 || Array.Exists(_ranges, r => r.IsWildcard);
        }

        private int[] CalculateAllowedValues(int min, int max)
        {
            if (_ranges != null && _ranges.Length > 0)
            {
                return _ranges.SelectMany(r => r.GetAllowedValues(min, max)).Distinct().OrderBy(v => v).ToArray();
            }
            return Enumerable.Range(min, max - min + 1).ToArray();
        }

        /// <inheritdoc/>
        public bool CanBe(int value)
        {
            if (_isWildcard)
            {
                return true;
            }
            return _lazyOrderdAllowedValues.Value.Contains(value);
        }

        /// <inheritdoc/>
        public int GetLowestValue()
        {
            return _lazyOrderdAllowedValues.Value[0];
        }

        /// <inheritdoc/>
        public bool TryGetLowestOfAtLeast(int atLeast, out int foundValue)
        {
            var options = _lazyOrderdAllowedValues.Value.Where(v => v >= atLeast);
            if (!options.Any())
            {
                foundValue = atLeast;
                return false;
            }
            foundValue = options.First();
            return true;
        }

        internal static ParseResult<CronField> TryParse(string value, CronValueParser parser)
        {
            if (value == "*")
            {
                return ParseResult.Success(new CronField(parser.Min, parser.Max, new CronRange(CronValue.Any, CronValue.Any, null)));
            }
            if (string.IsNullOrEmpty(value))
            {
                return ParseResult.Throw<CronField>(new CronParsingException(parser.FieldName, "value cannot be empty"));
            }
            var ranges = value.Split(',');
            var parsed = new CronRange[ranges.Length];
            for (int i = 0; i < ranges.Length; i++)
            {
                var parseResult = CronRange.TryParse(ranges[i], parser);
                if (!parseResult.TryGetResult(out CronRange range))
                {
                    return parseResult.CopyExceptionResult<CronField>();
                }
                parsed[i] = range;
            }

            return ParseResult.Success(new CronField(parser.Min, parser.Max, parsed));
        }

        /// <inheritdoc/>
        public string AsString()
        {
            if (IsWildcard)
            {
                return CronValue.AnyIndicator;
            }
            return string.Join(",", _ranges.Select(r => r.ToString()));
        }

        /// <inheritdoc cref="IReadOnlyCronField.AsString"/>
        public override string ToString()
        {
            return AsString();
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.OfEach(_ranges);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as CronField);
        }

        /// <inheritdoc/>
        public bool Equals(CronField other)
        {
            if (other == null)
            {
                return false;
            }
            return _ranges == null && other._ranges == null || _ranges.SequenceEqual(other._ranges);
        }
    }
}
