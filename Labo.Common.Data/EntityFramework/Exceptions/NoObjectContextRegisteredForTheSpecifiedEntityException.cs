﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoObjectContextRegisteredForTheSpecifiedEntityException.cs" company="Labo">
//   The MIT License (MIT)
//   
//   Copyright (c) 2014 Bora Akgun
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to
//   use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
//   the Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary>
//   Defines the NoObjectContextRegisteredForTheSpecifiedEntityException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Labo.Common.Data.EntityFramework.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class NoObjectContextRegisteredForTheSpecifiedEntityException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoObjectContextRegisteredForTheSpecifiedEntityException"/> class.
        /// </summary>
        public NoObjectContextRegisteredForTheSpecifiedEntityException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoObjectContextRegisteredForTheSpecifiedEntityException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public NoObjectContextRegisteredForTheSpecifiedEntityException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoObjectContextRegisteredForTheSpecifiedEntityException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public NoObjectContextRegisteredForTheSpecifiedEntityException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoObjectContextRegisteredForTheSpecifiedEntityException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected NoObjectContextRegisteredForTheSpecifiedEntityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}