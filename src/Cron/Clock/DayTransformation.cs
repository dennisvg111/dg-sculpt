using System;

namespace DG.Sculpt.Cron.Clock
{
    internal class DayTransformation : TimeTransformation
    {
        public DayTransformation(TimeSpan modifier, int lowestValue, TimeTransformation overflowTransformation) : base(modifier, lowestValue, (t) => t.Day, overflowTransformation)
        {
        }
    }
}
