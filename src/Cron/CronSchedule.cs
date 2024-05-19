using DG.Common.Exceptions;
using DG.Sculpt.Cron.FieldInternals;
using DG.Sculpt.Utilities;
using System;

namespace DG.Sculpt.Cron
{
    /// <summary>
    /// Represents a schedule based on cron syntax. A cron schedule contains minutes, hours, day-of-month, months, and day-of-week.
    /// </summary>
    public class CronSchedule
    {
        private readonly static CronValueParser[] _parsers = new CronValueParser[] { CronValueParser.Minutes, CronValueParser.Hours, CronValueParser.DayOfMonth, CronValueParser.Months, CronValueParser.DayOfWeek };

        private readonly IReadOnlyCronField _minutes;
        private readonly IReadOnlyCronField _hours;
        private readonly IReadOnlyCronField _dayOfMonth;
        private readonly IReadOnlyCronField _months;
        private readonly IReadOnlyCronField _dayOfWeek;

        /// <summary>
        /// The minutes field of this <see cref="CronSchedule"/>.
        /// </summary>
        public IReadOnlyCronField Minutes => _minutes;

        /// <summary>
        /// The hours field of this <see cref="CronSchedule"/>.
        /// </summary>
        public IReadOnlyCronField Hours => _hours;

        /// <summary>
        /// The day-of-month field of this <see cref="CronSchedule"/>.
        /// </summary>
        public IReadOnlyCronField DayOfMonth => _dayOfMonth;

        /// <summary>
        /// The months field of this <see cref="CronSchedule"/>.
        /// </summary>
        public IReadOnlyCronField Months => _months;

        /// <summary>
        /// The day-of-week field of this <see cref="CronSchedule"/>.
        /// </summary>
        public IReadOnlyCronField DayOfWeek => _dayOfWeek;

        /// <summary>
        /// Initializes a new instance of <see cref="CronSchedule"/> with the given fields.
        /// </summary>
        /// <param name="minutes"></param>
        /// <param name="hours"></param>
        /// <param name="dayOfMonth"></param>
        /// <param name="month"></param>
        /// <param name="dayOfWeek"></param>
        public CronSchedule(IReadOnlyCronField minutes, IReadOnlyCronField hours, IReadOnlyCronField dayOfMonth, IReadOnlyCronField month, IReadOnlyCronField dayOfWeek)
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

        /// <summary>
        /// Converts the given <paramref name="s"/> to a valid <see cref="CronSchedule"/>.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static CronSchedule Parse(string s)
        {
            var result = TryParse(s);
            return result.GetResultOrThrow();
        }

        /// <summary>
        /// Converts the given <paramref name="s"/> to a valid <see cref="CronSchedule"/>. A return value indicates if the conversion succeeded.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool TryParse(string s, out CronSchedule expression)
        {
            var result = TryParse(s);
            return result.TryGetResult(out expression);
        }

        private static ParseResult<CronSchedule> TryParse(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return ParseResult.Throw<CronSchedule>(new ArgumentException("Expression cannot be null or empty.", nameof(s)));
            }
            var fields = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (fields.Length != _parsers.Length)
            {
                return ParseResult.Throw<CronSchedule>(new ArgumentException($"Expression should contain exactly {_parsers.Length} fields.", nameof(s)));
            }

            CronField[] parsed = new CronField[_parsers.Length];
            for (int i = 0; i < _parsers.Length; i++)
            {
                var parseResult = CronField.TryParse(fields[i], _parsers[i]);
                if (!parseResult.TryGetResult(out var field))
                {
                    return parseResult.CopyExceptionResult<CronSchedule>();
                }
                parsed[i] = field;
            }

            return ParseResult.Success(new CronSchedule(parsed[0], parsed[1], parsed[2], parsed[3], parsed[4]));
        }

        /// <summary>
        /// <para>Returns a text representation of this <see cref="CronSchedule"/>.</para>
        /// <para>Note that the result of this function can be parsed using <see cref="TryParse(string, out CronSchedule)"/>.</para>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{_minutes.AsString()} {_hours.AsString()} {_dayOfMonth.AsString()} {_months.AsString()} {_dayOfWeek.AsString()}";
        }

        #region Static instances
        private static readonly Lazy<CronSchedule> _yearly = new Lazy<CronSchedule>(() => Parse("0 0 1 1 *"));
        private static readonly Lazy<CronSchedule> _monthly = new Lazy<CronSchedule>(() => Parse("0 0 1 * *"));
        private static readonly Lazy<CronSchedule> _weekly = new Lazy<CronSchedule>(() => Parse("0 0 * * 0"));
        private static readonly Lazy<CronSchedule> _daily = new Lazy<CronSchedule>(() => Parse("0 0 * * *"));
        private static readonly Lazy<CronSchedule> _hourly = new Lazy<CronSchedule>(() => Parse("0 * * * *"));

        /// <summary>
        /// Run once a year at midnight of 1 January.
        /// </summary>
        public static CronSchedule Yearly => _yearly.Value;

        /// <summary>
        /// Run once a month at midnight of the first day of the month.
        /// </summary>
        public static CronSchedule Monthly => _monthly.Value;

        /// <summary>
        /// Run once a week at midnight on Sunday.
        /// </summary>
        public static CronSchedule Weekly => _weekly.Value;

        /// <summary>
        /// Run once a day at midnight.
        /// </summary>
        public static CronSchedule Daily => _daily.Value;

        /// <summary>
        /// Run once an hour at the beginning of the hour.
        /// </summary>
        public static CronSchedule Hourly => _hourly.Value;
        #endregion
    }
}
