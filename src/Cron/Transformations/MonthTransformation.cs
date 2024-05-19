using System;
using DG.Sculpt.Cron.FieldInternals;

namespace DG.Sculpt.Cron.Transformations
{
    internal class MonthTransformation : ITimeTransformation
    {
        private readonly IReadOnlyCronField _monthField;

        public MonthTransformation(IReadOnlyCronField monthField)
        {
            _monthField = monthField;
        }

        public static MonthTransformation For(CronSchedule expression)
        {
            return new MonthTransformation(expression.Months);
        }

        public DateTimeOffset MoveBackwardsToLowest(DateTimeOffset time)
        {
            time = time.AddDays(-(time.Day - 1));
            return time.AddMonths(-(time.Month - 1));
        }

        public TransformationResult MoveForwardWhileNotValid(DateTimeOffset time)
        {
            if (_monthField.CanBe(time.Month))
            {
                return new TransformationResult(false, time);
            }
            if (!_monthField.TryGetLowestOfAtLeast(time.Month, out int target))
            {
                target = _monthField.GetLowestValue();
            }
            int neededMonths = target - time.Month;

            if (neededMonths < 0)
            {
                neededMonths += 12;
            }
            return new TransformationResult(true, time.AddMonths(neededMonths));
        }
    }
}
