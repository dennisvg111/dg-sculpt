using System;

namespace DG.Sculpt.Cron.Clock.Transformations
{
    internal class DayTransformation : ITimeTransformation
    {
        private readonly IReadOnlyCronField _dayOfMonthField;
        private readonly IReadOnlyCronField _dayOfWeekField;

        public DayTransformation(CronExpression cronExpression)
        {
            _dayOfMonthField = cronExpression.DayOfMonth;
            _dayOfWeekField = cronExpression.DayOfWeek;
        }

        public DateTimeOffset MoveBackwardsToLowest(DateTimeOffset time)
        {
            throw new NotImplementedException();
        }

        public TransformationResult MoveForwardWhileNotValid(DateTimeOffset time)
        {
            throw new NotImplementedException();
        }
    }
}
