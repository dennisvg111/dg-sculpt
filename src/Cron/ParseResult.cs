using System;

namespace DG.Sculpt.Cron
{
    internal class ParseResult<T>
    {
        private readonly T _value;
        private readonly bool _hasResult;
        private readonly Exception _exception;

        internal ParseResult(T value, bool hasResult, Exception exception)
        {
            _value = value;
            _hasResult = hasResult;
            _exception = exception;
        }

        public T GetResultOrThrow()
        {
            if (_hasResult)
            {
                return _value;
            }
            throw _exception;
        }

        public void ThrowIfNeeded()
        {
            if (!_hasResult)
            {
                throw _exception;
            }
        }

        public static ParseResult<T> Throw(Exception exception)
        {
            return new ParseResult<T>(default(T), false, exception);
        }
    }

    internal static class ParseResult
    {
        public static ParseResult<T> Success<T>(T result)
        {
            return new ParseResult<T>(result, true, null);
        }

        public static ParseResult<T> Throw<T>(Exception exception)
        {
            return new ParseResult<T>(default(T), false, exception);
        }
    }
}
