using DG.Common;
using DG.Sculpt.Cron.Exceptions;
using DG.Sculpt.Utilities;
using System;

namespace DG.Sculpt.Cron
{
    public struct CronRange : IEquatable<CronRange>
    {
        private readonly CronValue _start;
        private readonly CronValue _end;
        private readonly int? _stepValue;

        public bool IsAny => !_start.HasValue && !_stepValue.HasValue;

        public CronRange(CronValue from, CronValue to, int? stepValue)
        {
            _start = from;
            _end = to;
            _stepValue = stepValue;
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
            if (IsAny)
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
