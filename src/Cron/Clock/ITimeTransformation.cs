using System;

namespace DG.Sculpt.Cron.Clock
{
    internal interface ITimeTransformation
    {
        DateTimeOffset MoveBackwardsToLowest(DateTimeOffset time);
        TransformationResult MoveForwardWhileNotValid(DateTimeOffset time);
    }
}