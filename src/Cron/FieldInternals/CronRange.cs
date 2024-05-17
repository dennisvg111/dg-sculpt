using DG.Common;
using DG.Sculpt.Cron.Exceptions;
using DG.Sculpt.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DG.Sculpt.Cron.FieldInternals
{
    /// <summary>
    /// Represents an allowed range in a <see cref="CronField"/>.
    /// </summary>
    internal readonly struct CronRange : IEquatable<CronRange>
    {
        private readonly CronValue _start;
        private readonly CronValue _end;
        private readonly int? _stepValue;

        /// <summary>
        /// Indicates if this range allows any value.
        /// </summary>
        public bool IsWildcard => !_start.HasValue && !_stepValue.HasValue;

        /// <summary>
        /// <para>Initializes a new instance of <see cref="CronRange"/>.</para>
        /// <para>Throws an <see cref="ArgumentException"/> if <paramref name="start"/> and <paramref name="stepValue"/> have a value but <paramref name="end"/> does not.</para>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="stepValue"></param>
        /// <exception cref="ArgumentException"></exception>
        internal CronRange(CronValue start, CronValue end, int? stepValue)
        {
            if (start.HasValue && stepValue.HasValue && !end.HasValue)
            {
                throw new ArgumentException("range end cannot be empty if start end step have value");
            }
            _start = start;
            _end = end;
            _stepValue = stepValue;
        }

        public IEnumerable<int> GetAllowedValues(int min, int max)
        {
            if (_start.HasValue && !_end.HasValue && !_stepValue.HasValue)
            {
                return new int[] { _start.Value };
            }
            var range = Enumerable.Range(min, max - min + 1);
            range = FilterOnStartEnd(range);
            range = FilterOnSteps(range);

            return range;
        }

        private IEnumerable<int> FilterOnStartEnd(IEnumerable<int> range)
        {
            if (!_start.HasValue)
            {
                return range;
            }
            var startValue = _start.Value;
            if (!_end.HasValue)
            {
                return range.Where(v => v == startValue);
            }
            var endValue = _end.Value;
            if (startValue <= endValue)
            {
                return range.Where(v => v >= startValue && v <= endValue);
            }
            return Enumerable.Concat(range.Where(v => v >= startValue), range.Where(v => v <= endValue));
        }

        private IEnumerable<int> FilterOnSteps(IEnumerable<int> range)
        {
            if (!_stepValue.HasValue)
            {
                return range;
            }
            var stepValue = _stepValue.Value;
            return range.Where((x, i) => i % stepValue == 0);
        }

        internal static ParseResult<CronRange> TryParse(string s, CronValueParser parser)
        {
            int? stepValue = null;
            CronValue end = CronValue.Any;
            if (s.TrySplitOn("/", out s, out string stepString))
            {
                if (stepString.Length == 0 || !int.TryParse(stepString, out int parsedStepValue) || parsedStepValue < 1 || parsedStepValue > parser.MaxStepValue)
                {
                    return ParseResult.Throw<CronRange>(new CronParsingException($"{parser.FieldName} step value", $"invalid value '{stepString}'"));
                }
                stepValue = parsedStepValue;
            }

            if (s.TrySplitOn("-", out s, out string endString))
            {
                var endParsed = ParseValue(endString, "range end", parser);
                if (!endParsed.TryGetResult(out end))
                {
                    return endParsed.CopyExceptionResult<CronRange>();
                }
            }

            var startParsed = ParseValue(s, "range start", parser);
            if (!startParsed.TryGetResult(out CronValue start))
            {
                return startParsed.CopyExceptionResult<CronRange>();
            }

            if (!end.HasValue && start.HasValue && stepValue.HasValue)
            {
                end = new CronValue(parser.Max);
            }

            return ParseResult.Success(new CronRange(start, end, stepValue));
        }

        private static ParseResult<CronValue> ParseValue(string s, string rangePart, CronValueParser parser)
        {
            if (string.IsNullOrEmpty(s))
            {
                return ParseResult.Throw<CronValue>(new CronParsingException($"{parser.FieldName} {rangePart}", $"value cannot be empty"));
            }
            var parseResult = parser.TryParse(s);
            if (!parseResult.HasResult)
            {
                return ParseResult.Throw<CronValue>(new CronParsingException($"{parser.FieldName} {rangePart}", $"invalid value '{s}'"));
            }
            return parseResult;
        }

        /// <summary>
        /// Returns a text representation of this range.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (IsWildcard)
            {
                return CronValue.AnyIndicator;
            }
            var result = _start.ToString();
            if (_end.HasValue)
            {
                result += "-" + _end;
            }
            if (_stepValue.HasValue)
            {
                result += "/" + _stepValue;
            }
            return result;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Of(_start)
                .And(_end)
                .And(_stepValue);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (!(obj is CronRange))
            {
                return false;
            }
            return Equals((CronRange)obj);
        }

        /// <inheritdoc/>
        public bool Equals(CronRange other)
        {
            return _start.Equals(other._start) && _end.Equals(other._end) && _stepValue == other._stepValue;
        }
    }
}
