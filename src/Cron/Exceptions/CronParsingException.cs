using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace DG.Sculpt.Cron.Exceptions
{
    /// <summary>
    /// Represents errors that occur while parsing a <see cref="CronExpression"/>
    /// </summary>
    [Serializable]
    public sealed class CronParsingException : Exception
    {
        private readonly string _fieldName;
        private readonly string _reason;

        /// <inheritdoc />
        public override string Message => $"Error parsing {_fieldName} field; {_reason}.";

        /// <summary>
        /// Initializes a new instance of the <see cref="CronParsingException"/> class.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="reason"></param>
        public CronParsingException(string fieldName, string reason) : base()
        {
            _fieldName = fieldName;
            _reason = reason;
        }

        private CronParsingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _fieldName = info.GetString(nameof(_fieldName));
            _reason = info.GetString(nameof(_reason));
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(_fieldName), _fieldName);
            info.AddValue(nameof(_reason), _reason);
        }
    }
}
