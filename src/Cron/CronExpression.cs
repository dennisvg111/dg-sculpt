using DG.Common.Exceptions;
using DG.Sculpt.Utilities;
using System;

namespace DG.Sculpt.Cron
{
    public class CronExpression
    {
        private readonly static CronValueParser[] _parsers = new CronValueParser[] { CronValueParser.Minutes, CronValueParser.Hours, CronValueParser.DayOfMonth, CronValueParser.Months, CronValueParser.DayOfWeek };

        private readonly IReadOnlyCronField _minutes;
        private readonly IReadOnlyCronField _hours;
        private readonly IReadOnlyCronField _dayOfMonth;
        private readonly IReadOnlyCronField _months;
        private readonly IReadOnlyCronField _dayOfWeek;

        public IReadOnlyCronField Minutes => _minutes;
        public IReadOnlyCronField Hours => _hours;
        public IReadOnlyCronField DayOfMonth => _dayOfMonth;
        public IReadOnlyCronField Months => _months;
        public IReadOnlyCronField DayOfWeek => _dayOfWeek;

        public CronExpression(IReadOnlyCronField minutes, IReadOnlyCronField hours, IReadOnlyCronField dayOfMonth, IReadOnlyCronField month, IReadOnlyCronField dayOfWeek)
        {
            ThrowIf.Parameter.IsNull(minutes, nameof(minutes));
            ThrowIf.Parameter.IsNull(hours, nameof(hours));
            ThrowIf.Parameter.IsNull(dayOfMonth, nameof(dayOfMonth));
            ThrowIf.Parameter.IsNull(month, nameof(month));
            ThrowIf.Parameter.IsNull(dayOfWeek, nameof(dayOfWeek));

            _minutes = minutes;
            _hours = hours;
            _dayOfMonth = dayOfMonth;
            _months = month;
            _dayOfWeek = dayOfWeek;
        }

        public DateTimeOffset GetNextOccurrence(DateTimeOffset startingFrom, bool includeCurrent = false)
        {
            var clock = new CronClock(this, startingFrom);
            if (!includeCurrent || !clock.IsValid())
            {
                clock.TravelToNextOccurence();
            }
            return clock.Time;
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
            return $"{_minutes.AsString()} {_hours.AsString()} {_dayOfMonth.AsString()} {_months.AsString()} {_dayOfWeek.AsString()}";
        }

        #region Static instances
        private static readonly Lazy<CronExpression> _yearly = new Lazy<CronExpression>(() => Parse("0 0 1 1 *"));
        private static readonly Lazy<CronExpression> _monthly = new Lazy<CronExpression>(() => Parse("0 0 1 * *"));
        private static readonly Lazy<CronExpression> _weekly = new Lazy<CronExpression>(() => Parse("0 0 * * 0"));
        private static readonly Lazy<CronExpression> _daily = new Lazy<CronExpression>(() => Parse("0 0 * * *"));
        private static readonly Lazy<CronExpression> _hourly = new Lazy<CronExpression>(() => Parse("0 * * * *"));

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
