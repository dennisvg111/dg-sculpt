using System;

namespace DG.Sculpt.Cron.Transformations
{
    internal interface ITimeTransformation
    {
        DateTimeOffset MoveBackwardsToLowest(DateTimeOffset time);
        TransformationResult MoveForwardWhileNotValid(DateTimeOffset time);
    }
}