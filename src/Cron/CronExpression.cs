using DG.Sculpt.Utilities;
using System;

namespace DG.Sculpt.Cron
{
    public class CronExpression
    {
        private readonly static CronValueParser[] _parsers = new CronValueParser[] { CronValueParser.Minutes, CronValueParser.Hours, CronValueParser.DayOfMonth, CronValueParser.Months, CronValueParser.DayOfWeek };

        private readonly CronField _minutes;
        private readonly CronField _hours;
        private readonly CronField _dayOfMonth;
        private readonly CronField _months;
        private readonly CronField _dayOfWeek;

        public CronField Minutes => _minutes;
        public CronField Hours => _hours;
        public CronField DayOfMonth => _dayOfMonth;
        public CronField Months => _months;
        public CronField DayOfWeek => _dayOfWeek;

        public CronExpression(CronField minutes, CronField hours, CronField dayOfMonth, CronField month, CronField dayOfWeek)
        {
            _minutes = minutes;
            _hours = hours;
            _dayOfMonth = dayOfMonth;
            _months = month;
            _dayOfWeek = dayOfWeek;
        }

        /// <summary>
        /// Converts the given <paramref name="s"/> to a valid <see cref="CronExpression"/>.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static CronExpression Parse(string s)
        {
            var result = TryParse(s);
            return result.GetResultOrThrow();
        }

        /// <summary>
        /// Converts the given <paramref name="s"/> to a valid <see cref="CronExpression"/>. A return value indicates if the conversion succeeded.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool TryParse(string s, out CronExpression expression)
        {
            var result = TryParse(s);
            return result.TryGetResult(out expression);
        }

        private static ParseResult<CronExpression> TryParse(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return ParseResult.Throw<CronExpression>(new ArgumentException("Expression cannot be null or empty.", nameof(s)));
            }
            var fields = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (fields.Length != _parsers.Length)
            {
                return ParseResult.Throw<CronExpression>(new ArgumentException($"Expression should contain exactly {_parsers.Length} fields.", nameof(s)));
            }

            CronField[] parsed = new CronField[_parsers.Length];
            for (int i = 0; i < _parsers.Length; i++)
            {
                var parseResult = CronField.TryParse(fields[i], _parsers[i]);
                if (!parseResult.TryGetResult(out var field))
                {
                    return parseResult.CopyExceptionResult<CronExpression>();
                }
                parsed[i] = field;
            }

            return ParseResult.Success(new CronExpression(parsed[0], parsed[1], parsed[2], parsed[3], parsed[4]));
        }

        /// <summary>
        /// <para>Returns a text representation of this <see cref="CronExpression"/>.</para>
        /// <para>Note that the result of this function can be parsed using <see cref="TryParse(string, out CronExpression)"/>.</para>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{_minutes} {_hours} {_dayOfMonth} {_months} {_dayOfWeek}";
        }

        #region Static instances
        private CronExpression(int? minutes, int? hours, int? dayOfMonth, int? month, int? dayOfWeek)
            : this(CronField.ForSingleValue(minutes), CronField.ForSingleValue(hours), CronField.ForSingleValue(dayOfMonth), CronField.ForSingleValue(month), CronField.ForSingleValue(dayOfWeek))
        { }

        private static readonly Lazy<CronExpression> _yearly = new Lazy<CronExpression>(() => new CronExpression(0, 0, 1, 1, null));
        private static readonly Lazy<CronExpression> _monthly = new Lazy<CronExpression>(() => new CronExpression(0, 0, 1, null, null));
        private static readonly Lazy<CronExpression> _weekly = new Lazy<CronExpression>(() => new CronExpression(0, 0, null, null, 0));
        private static readonly Lazy<CronExpression> _daily = new Lazy<CronExpression>(() => new CronExpression(0, 0, null, null, null));
        private static readonly Lazy<CronExpression> _hourly = new Lazy<CronExpression>(() => new CronExpression(0, null, null, null, null));

        /// <summary>
        /// Run once a year at midnight of 1 January.
        /// </summary>
        public static CronExpression Yearly => _yearly.Value;

        /// <summary>
        /// Run once a month at midnight of the first day of the month.
        /// </summary>
        public static CronExpression Monthly => _monthly.Value;

        /// <summary>
        /// Run once a week at midnight on Sunday.
        /// </summary>
        public static CronExpression Weekly => _weekly.Value;

        /// <summary>
        /// Run once a day at midnight.
        /// </summary>
        public static CronExpression Daily => _daily.Value;

        /// <summary>
        /// Run once an hour at the beginning of the hour.
        /// </summary>
        public static CronExpression Hourly => _hourly.Value;
        #endregion
    }
}
