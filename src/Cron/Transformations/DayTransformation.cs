using System;
using DG.Sculpt.Cron.FieldInternals;

namespace DG.Sculpt.Cron.Transformations
{
    internal class DayTransformation : ITimeTransformation
    {
        private readonly IReadOnlyCronField _dayOfMonthField;
        private readonly IReadOnlyCronField _dayOfWeekField;

        public DayTransformation(IReadOnlyCronField dayOfMonthField, IReadOnlyCronField dayOfWeekField)
        {
            _dayOfMonthField = dayOfMonthField;
            _dayOfWeekField = dayOfWeekField;
        }

        public static DayTransformation For(CronSchedule expression)
        {
            return new DayTransformation(expression.DayOfMonth, expression.DayOfWeek);
        }

        public DateTimeOffset MoveBackwardsToLowest(DateTimeOffset time)
        {
            if (time.Day == 1)
            {
                return time;
            }
            return time.AddDays(-(time.Day - 1));
        }

        public TransformationResult MoveForwardWhileNotValid(DateTimeOffset time)
        {
            if (DayIsValid(time))
            {
                return new TransformationResult(false, time);
            }
            if (_dayOfWeekField.IsWildcard)
            {
                if (TryMoveToValidDayOfMonth(time, out DateTimeOffset result))
                {
                    return new TransformationResult(true, result);
                }
            }
            do
            {
                time = time.AddDays(1);
            } while (!DayIsValid(time));
            return new TransformationResult(true, time);
        }

        private bool TryMoveToValidDayOfMonth(DateTimeOffset time, out DateTimeOffset result)
        {
            result = DateTimeOffset.MinValue;
            if (!_dayOfMonthField.TryGetLowestOfAtLeast(time.Day, out int target))
            {
                return false;
            }
            if (DateTime.DaysInMonth(time.Year, time.Month) < target)
            {
                return false;
            }
            var daysNeeded = target - time.Day;
            result = time.AddDays(daysNeeded);
            return true;
        }

        public bool DayIsValid(DateTimeOffset time)
        {
            if (_dayOfWeekField.IsWildcard)
            {
                return _dayOfMonthField.CanBe(time.Day);
            }
            if (_dayOfMonthField.IsWildcard)
            {
                return _dayOfWeekField.CanBe((int)time.DayOfWeek);
            }
            return _dayOfMonthField.CanBe(time.Day) || _dayOfWeekField.CanBe((int)time.DayOfWeek);
        }
    }
}
