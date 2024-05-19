using DG.Sculpt.Cron.Transformations;
using System;

namespace DG.Sculpt.Cron
{
    /// <summary>
    /// Contains the logic needed to calculate whenever a <see cref="CronSchedule"/> would trigger.
    /// </summary>
    public class CronClock
    {
        private readonly CronSchedule _cronExpression;
        private DateTimeOffset _time;

        private readonly DayTransformation _dayTransformation;
        private readonly ITimeTransformation[] _transformations;

        /// <summary>
        /// Returns the current time of this <see cref="CronClock"/>.
        /// </summary>
        public DateTimeOffset Time => _time;

        /// <summary>
        /// Initializes a new instance of <see cref="CronClock"/> with the given <see cref="CronSchedule"/> and <paramref name="time"/>.
        /// </summary>
        /// <param name="cronExpression"></param>
        /// <param name="time"></param>
        public CronClock(CronSchedule cronExpression, DateTimeOffset time)
        {
            _cronExpression = cronExpression;

            _time = time;

            _dayTransformation = DayTransformation.For(cronExpression);
            _transformations = new ITimeTransformation[]
            {
                MinutesTransformation.For(cronExpression),
                HourTransformation.For(cronExpression),
                _dayTransformation,
                MonthTransformation.For(cronExpression)
            };
        }

        /// <summary>
        /// Indicates if the current <see cref="Time"/> is valid according to the used <see cref="CronSchedule"/>.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return _cronExpression.Minutes.CanBe(_time.Minute)
                && _cronExpression.Hours.CanBe(_time.Hour)
                && _cronExpression.Months.CanBe(_time.Month)
                && _dayTransformation.DayIsValid(_time);
        }

        /// <summary>
        /// Moves <see cref="Time"/> to the next valid value. Note that this will always be at least one minute after the previous value.
        /// </summary>
        public void MoveToNextOccurence()
        {
            _time = _time.AddMinutes(1);
            MoveWhileNotValid();
        }

        /// <summary>
        /// Moves <see cref="Time"/> while it is not value. If its value is already valid, nothing changes.
        /// </summary>
        public void MoveWhileNotValid()
        {
            _time = TransformUntillValid(_time);
        }

        private DateTimeOffset TransformUntillValid(DateTimeOffset time)
        {
            for (int i = 0; i < _transformations.Length; i++)
            {
                var result = _transformations[i].MoveForwardWhileNotValid(time);
                time = result.Time;
                if (result.IsChanged && i > 0)
                {
                    time = ResetLowerUnits(time, i - 1);
                    return TransformUntillValid(time);
                }
            }
            return time;
        }

        private DateTimeOffset ResetLowerUnits(DateTimeOffset time, int i)
        {
            for (; i >= 0; i--)
            {
                time = _transformations[i].MoveBackwardsToLowest(time);
            }
            return time;
        }
    }
}
