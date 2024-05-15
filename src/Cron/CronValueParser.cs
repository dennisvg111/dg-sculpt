using DG.Sculpt.Cron.Exceptions;
using System;
using System.Linq;

namespace DG.Sculpt.Cron
{
    /// <summary>
    /// A parsing utility class for <see cref="CronValue"/>.
    /// </summary>
    public class CronValueParser
    {
        private readonly string _fieldName;
        private readonly int _min;
        private readonly int _max;
        private readonly int _allowedOverflow;
        private readonly string[] _lookUp;

        /// <summary>
        /// Initializes a new instance of the <see cref="CronValueParser"/> class.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="allowedOverflow"></param>
        /// <param name="lookUp"></param>
        public CronValueParser(string fieldName, int min, int max, int allowedOverflow, params string[] lookUp)
        {
            _fieldName = fieldName;
            _min = min;
            _max = max;
            _allowedOverflow = allowedOverflow;
            _lookUp = lookUp;
        }

        /// <summary>
        /// Converts the given <paramref name="s"/> to a valid <see cref="CronValue"/>.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <exception cref="CronParsingException" />
        public CronValue Parse(string s)
        {
            var result = TryParse(s);
            return result.GetResultOrThrow();
        }

        /// <summary>
        /// Converts the given <paramref name="s"/> to a valid <see cref="CronValue"/>. A return value indicates if the conversion succeeded.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        internal ParseResult<CronValue> TryParse(string s)
        {
            if (s == CronValue.AnyIndicator)
            {
                return ParseResult.Success(CronValue.Any);
            }

            if (int.TryParse(s, out int result))
            {
                if (result < _min)
                {
                    return ParseResult<CronValue>.Throw(new CronParsingException(_fieldName, $"{s} should be at least {_min}"));
                }
                if (result > _max)
                {
                    if (result - _allowedOverflow <= _max)
                    {
                        return ParseResult.Success(new CronValue((result % (_max + 1) + _min), null));
                    }
                    return ParseResult<CronValue>.Throw(new CronParsingException(_fieldName, $"{s} should be at most {_max}"));
                }
                return ParseResult.Success(new CronValue(result, null));
            }

            if (TryGetIndex(s, out int index))
            {
                return ParseResult.Success(new CronValue(index + _min, s));
            }
            return ParseResult<CronValue>.Throw(new CronParsingException(_fieldName, $"'{s}' is not a valid value"));
        }

        private bool TryGetIndex(string key, out int index)
        {
            if (_lookUp == null || !_lookUp.Any())
            {
                index = 0;
                return false;
            }
            index = Array.IndexOf(_lookUp, key.ToUpperInvariant());
            return index >= 0;
        }

        #region Static instances
        private static readonly Lazy<CronValueParser> _minutes = new Lazy<CronValueParser>(() => new CronValueParser("minutes", 0, 59, 0));
        private static readonly Lazy<CronValueParser> _hours = new Lazy<CronValueParser>(() => new CronValueParser("hours", 0, 23, 0));
        private static readonly Lazy<CronValueParser> _dayOfMonth = new Lazy<CronValueParser>(() => new CronValueParser("day of month", 1, 31, 0));
        private static readonly Lazy<CronValueParser> _months = new Lazy<CronValueParser>(() => new CronValueParser("months", 1, 12, 0,
            "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"));
        private static readonly Lazy<CronValueParser> _dayOfWeek = new Lazy<CronValueParser>(() => new CronValueParser("day of week", 0, 6, 1,
            "SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT"));

        /// <summary>
        /// Parser used for <see cref="CronExpression.Minutes"/>.
        /// </summary>
        public static CronValueParser Minutes => _minutes.Value;

        /// <summary>
        /// Parser used for <see cref="CronExpression.Hours"/>.
        /// </summary>
        public static CronValueParser Hours => _hours.Value;

        /// <summary>
        /// Parser used for <see cref="CronExpression.DayOfMonth"/>.
        /// </summary>
        public static CronValueParser DayOfMonth => _dayOfMonth.Value;

        /// <summary>
        /// Parser used for <see cref="CronExpression.Months"/>.
        /// </summary>
        public static CronValueParser Months => _months.Value;

        /// <summary>
        /// Parser used for <see cref="CronExpression.DayOfWeek"/>.
        /// </summary>
        public static CronValueParser DayOfWeek => _dayOfWeek.Value;
        #endregion
    }
}
