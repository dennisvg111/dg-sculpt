using System;

namespace DG.Sculpt.Cron.Clock.Transformations
{
    internal class MonthTransformation : ITimeTransformation
    {
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
