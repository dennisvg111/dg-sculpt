using System;

namespace DG.Sculpt.Cron.Clock
{
    internal class TransformationResult
    {
        private bool _isChanged;
        private DateTimeOffset _result;

        public bool IsChanged => _isChanged;
        public DateTimeOffset Result => _result;

        public TransformationResult(bool isChanged, DateTimeOffset result)
        {
            _isChanged = isChanged;
            _result = result;
        }
    }
}
