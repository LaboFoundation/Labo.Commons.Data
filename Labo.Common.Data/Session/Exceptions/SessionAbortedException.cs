namespace Labo.Common.Data.Session.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    using Labo.Common.Exceptions;

    [Serializable]
    public class SessionAbortedException : CoreLevelException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionAbortedException"/> class.
        /// </summary>
        public SessionAbortedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionAbortedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public SessionAbortedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionAbortedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public SessionAbortedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionAbortedException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected SessionAbortedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}