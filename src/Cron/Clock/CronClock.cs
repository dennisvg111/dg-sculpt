using DG.Sculpt.Cron.Clock.Transformations;
using System;

namespace DG.Sculpt.Cron.Clock
{
    public class CronClock
    {
        private readonly CronExpression _cronExpression;
        private DateTimeOffset _time;

        private readonly ITimeTransformation[] _transformations;

        public DateTimeOffset Time => _time;

        private int _minutes => _time.Minute;
        private int _hour => _time.Hour;
        private int _dayOfWeek => (int)_time.DayOfWeek;
        private int _dayOfMonth => _time.Day;
        private int _month => _time.Month;

        public CronClock(CronExpression cronExpression, DateTimeOffset time)
        {
            _cronExpression = cronExpression;

            _time = time;

            _transformations = new ITimeTransformation[]
            {
                new MinutesTransformation(_cronExpression.Minutes),
                new HourTransformation(_cronExpression.Hours)
            };
        }

        /// <summary>
        /// Indicates if the current <see cref="Time"/> is valid according to the used <see cref="CronExpression"/>.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return _cronExpression.Minutes.CanBe(_minutes)
                && _cronExpression.Hours.CanBe(_hour)
                && _cronExpression.Months.CanBe(_month)
                && (_cronExpression.DayOfWeek.CanBe(_dayOfWeek) || _cronExpression.DayOfMonth.CanBe(_dayOfMonth));
        }

        public void MoveToNextOccurence()
        {
            _time = _time.AddMinutes(1);
            MoveWhileNotValid();
        }

        public void MoveWhileNotValid()
        {
            for (int i = 0; i < _transformations.Length; i++)
            {
                var result = _transformations[i].MoveForwardWhileNotValid(_time);
                _time = result.Time;
                if (result.IsChanged && i > 0)
                {
                    ResetLowerUnits(i - 1);
                    MoveWhileNotValid();
                    break;
                }
            }
        }

        private void ResetLowerUnits(int i)
        {
            for (; i >= 0; i--)
            {
                _time = _transformations[i].MoveBackwardsToLowest(_time);
            }
        }
    }
}
