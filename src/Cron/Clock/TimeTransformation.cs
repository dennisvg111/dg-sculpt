using System;

namespace DG.Sculpt.Cron.Clock
{
    internal abstract class TimeTransformation
    {
        private readonly TimeTransformation _overflowTransformation;

        public TimeTransformation(TimeTransformation overflowTransformation)
        {
            _overflowTransformation = overflowTransformation;
        }

        public DateTimeOffset Transform(DateTimeOffset time)
        {
            var result = MoveUntillValid(time);
            if (!result.IsChanged || _overflowTransformation == null)
            {
                return result.Result;
            }
            return _overflowTransformation.Reset(result.Result);
        }

        private DateTimeOffset Reset(DateTimeOffset time)
        {
            time = MoveBackwardsToLowest(time);
            if (_overflowTransformation == null)
            {
                return time;
            }
            return _overflowTransformation.Reset(time);
        }

        public abstract TransformationResult MoveUntillValid(DateTimeOffset time);

        public abstract DateTimeOffset MoveBackwardsToLowest(DateTimeOffset time);
    }
}
