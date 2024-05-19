using System;

namespace DG.Sculpt.Cron.Clock.Transformations
{
    internal class HourTransformation : ITimeTransformation
    {
        private readonly IReadOnlyCronField _hourField;

        public HourTransformation(IReadOnlyCronField hourField)
        {
            _hourField = hourField;
        }

        public DateTimeOffset MoveBackwardsToLowest(DateTimeOffset time)
        {
            return time.AddHours(-time.Hour);
        }

        public TransformationResult MoveForwardWhileNotValid(DateTimeOffset time)
        {
            if (_hourField.CanBe(time.Hour))
            {
                return new TransformationResult(false, time);
            }
            if (!_hourField.TryGetLowestOfAtLeast(time.Hour, out int target))
            {
                target = _hourField.GetLowestValue();
            }
            return new TransformationResult(true, MoveTo(time, target));
        }

        private DateTimeOffset MoveTo(DateTimeOffset time, int target)
        {
            int hoursNeeded = target - time.Hour;
            if (hoursNeeded < 0)
            {
                hoursNeeded += 24;
            }
            return time.AddHours(hoursNeeded);
        }
    }
}
