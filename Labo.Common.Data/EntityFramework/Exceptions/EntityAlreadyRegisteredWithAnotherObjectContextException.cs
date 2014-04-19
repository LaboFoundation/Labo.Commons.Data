namespace Labo.Common.Data.EntityFramework.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    using Labo.Common.Exceptions;

    /// <summary>
    /// Entity already registered to another object context exception.
    /// </summary>
    [Serializable]
    public class EntityAlreadyRegisteredWithAnotherObjectContextException : CoreLevelException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAlreadyRegisteredWithAnotherObjectContextException"/> class.
        /// </summary>
        public EntityAlreadyRegisteredWithAnotherObjectContextException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAlreadyRegisteredWithAnotherObjectContextException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public EntityAlreadyRegisteredWithAnotherObjectContextException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAlreadyRegisteredWithAnotherObjectContextException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public EntityAlreadyRegisteredWithAnotherObjectContextException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAlreadyRegisteredWithAnotherObjectContextException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected EntityAlreadyRegisteredWithAnotherObjectContextException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
