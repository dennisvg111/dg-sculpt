using System;
using DG.Sculpt.Cron.FieldInternals;

namespace DG.Sculpt.Cron.Transformations
{
    internal class MinutesTransformation : ITimeTransformation
    {
        private readonly IReadOnlyCronField _minutesField;

        public MinutesTransformation(IReadOnlyCronField minutesField)
        {
            _minutesField = minutesField;
        }

        public static MinutesTransformation For(CronSchedule expression)
        {
            return new MinutesTransformation(expression.Minutes);
        }

        public DateTimeOffset MoveBackwardsToLowest(DateTimeOffset time)
        {
            int secondsAbove0 = time.Minute * 60 + time.Second;
            return time.AddSeconds(-secondsAbove0);
        }

        public TransformationResult MoveForwardWhileNotValid(DateTimeOffset time)
        {
            if (_minutesField.CanBe(time.Minute))
            {
                return new TransformationResult(false, time);
            }
            if (!_minutesField.TryGetLowestOfAtLeast(time.Minute, out int target))
            {
                target = _minutesField.GetLowestValue();
            }
            return new TransformationResult(true, MoveTo(time, target));
        }

        private DateTimeOffset MoveTo(DateTimeOffset time, int target)
        {
            int secondsNeeded = target * 60 - (time.Minute * 60 + time.Second);
            if (secondsNeeded < 0)
            {
                secondsNeeded += 3600;
            }
            return time.AddSeconds(secondsNeeded);
        }
    }
}
