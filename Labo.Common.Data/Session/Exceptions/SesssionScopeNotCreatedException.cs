namespace Labo.Common.Data.Session.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    using Labo.Common.Exceptions;

    /// <summary>
    /// The session scope not created exception.
    /// </summary>
    [Serializable]
    public class SesssionScopeNotCreatedException : CoreLevelException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SesssionScopeNotCreatedException"/> class.
        /// </summary>
        public SesssionScopeNotCreatedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SesssionScopeNotCreatedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SesssionScopeNotCreatedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SesssionScopeNotCreatedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public SesssionScopeNotCreatedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SesssionScopeNotCreatedException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected SesssionScopeNotCreatedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
