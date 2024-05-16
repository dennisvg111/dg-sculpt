using DG.Common;
using DG.Sculpt.Cron.Exceptions;
using DG.Sculpt.Utilities;
using System;
using System.Linq;

namespace DG.Sculpt.Cron
{
    /// <summary>
    /// Represents a single field in a <see cref="CronExpression"/>.
    /// </summary>
    public sealed class CronField : IReadOnlyCronField, IEquatable<CronField>
    {
        private readonly CronRange[] _ranges;

        /// <summary>
        /// Indicates if this field matches any number.
        /// </summary>
        public bool IsAny => _ranges == null || Array.TrueForAll(_ranges, r => r.IsAny);

        /// <summary>
        /// Initializes a new instance of a <see cref="CronField"/>.
        /// </summary>
        /// <param name="ranges"></param>
        public CronField(params CronRange[] ranges)
        {
            _ranges = ranges;
        }

        internal static CronField ForSingleValue(int? value)
        {
            return new CronField(new CronRange(new CronValue(value), CronValue.Any, null));
        }

        internal static ParseResult<CronField> TryParse(string value, CronValueParser parser)
        {
            if (value == "*")
            {
                return ParseResult.Success(ForSingleValue(null));
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

            return ParseResult.Success(new CronField(parsed));
        }

        /// <summary>
        /// Returns a text representation of this field.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (IsAny)
            {
                return CronValue.AnyIndicator;
            }
            return string.Join(",", _ranges.Select(r => r.ToString()));
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
            return (_ranges == null && other._ranges == null) || _ranges.SequenceEqual(other._ranges);
        }
    }
}
