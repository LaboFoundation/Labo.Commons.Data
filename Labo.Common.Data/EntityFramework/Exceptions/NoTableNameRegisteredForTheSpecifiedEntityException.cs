namespace Labo.Common.Data.EntityFramework.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class NoTableNameRegisteredForTheSpecifiedEntityException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoTableNameRegisteredForTheSpecifiedEntityException"/> class.
        /// </summary>
        public NoTableNameRegisteredForTheSpecifiedEntityException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoTableNameRegisteredForTheSpecifiedEntityException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public NoTableNameRegisteredForTheSpecifiedEntityException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoTableNameRegisteredForTheSpecifiedEntityException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public NoTableNameRegisteredForTheSpecifiedEntityException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoTableNameRegisteredForTheSpecifiedEntityException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected NoTableNameRegisteredForTheSpecifiedEntityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}