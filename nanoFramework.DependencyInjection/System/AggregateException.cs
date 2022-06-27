//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System.Collections;
using System.Diagnostics;

namespace System
{
    /// <summary>Initializes a new instance of the <see cref="AggregateException"/> class with a system-supplied message that describes the error.</summary>
    /// <remarks>
    /// <see cref="AggregateException"/> is used to consolidate multiple failures into a single, throwable
    /// exception object.
    /// </remarks>
    [Serializable]
    [DebuggerDisplay("Count = {InnerExceptionCount}")]
    public class AggregateException : Exception
    {
        private readonly Exception[] _innerExceptions;
        private static readonly string _defaultMessage = "One or more errors occurred.";

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateException"/> class.
        /// </summary>
        public AggregateException()
            : this(_defaultMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateException"/> class with
        /// a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public AggregateException(string message)
            : base(message)
        {
            _innerExceptions = new Exception[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateException"/> class with
        /// a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exceptions that are the cause of the current exception.</param>
        /// <exception cref="ArgumentNullException">One of the <paramref name="innerException"/> is <see langword="null"/>.</exception>
        public AggregateException(string message, params Exception[] innerException)
            : base(message)
        {
            foreach(Exception ex in innerException)
            {
                if (ex == null)
                {
                    throw new ArgumentNullException();
                }
            }
            
            _innerExceptions = innerException;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateException"/> class with
        /// a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">An array list of <see cref="Exception"/> object exceptions that is the cause of the current exception.</param>
        /// <exception cref="ArgumentException">An element of innerExceptions not of Exception type.</exception>
        /// <exception cref="ArgumentNullException">One of the <paramref name="innerException"/> is <see langword="null"/>.</exception>
        public AggregateException(string message, ArrayList innerException)
            : base(message)
        {
            foreach (Exception ex in innerException)
            {
                if (ex == null)
                {
                    throw new ArgumentNullException();
                }
            }

            try
            {          
                _innerExceptions = (Exception[])innerException.ToArray(typeof(Exception));
            }
            catch
            {
                throw new ArgumentException();                
            }
        }

        /// <summary>
        /// Gets an <see cref="ArrayList"/> of the exception instances that caused the current exception.
        /// </summary>
        public ArrayList InnerExceptions
        {
            get
            {
                var execptions = new ArrayList();
                foreach(Exception ex in _innerExceptions)
                {
                    execptions.Add(ex);
                }
                
                return execptions;
            }
        }

        /// <summary>
        /// Gets a message that describes the exception.
        /// </summary>
        public override string Message
        {
            get
            {
                if (_innerExceptions.Length == 0)
                {
                    return base.Message;
                }

                var msg = $"{ base.Message } ";

                var count = _innerExceptions.Length;
                for (int index = 0; index < count; index++)
                {           
                    if (index < count - 1)
                    {
                        msg = string.Concat(msg, $"({ _innerExceptions[index].Message }) ");
                    }
                    else
                    {
                        msg = string.Concat(msg, $"({ _innerExceptions[index].Message })");
                    }
                }

                return msg;
            }
        }

        /// <summary>
        /// Creates and returns a string representation of the current <see cref="AggregateException"/>.
        /// </summary>
        /// <returns>A string representation of the current exception.</returns>
        public override string ToString()
        {
            string msg = base.ToString();

            for (int index = 0; index < _innerExceptions.Length; index++)
            {
                if (index == 0)
                {
                    msg = string.Concat(msg, "\n");
                }

                if (_innerExceptions[index] == InnerException)
                {
                    // Already logged in base.ToString()
                    continue;
                }

                msg = string.Concat(msg, $"---> (Inner Exception #{index}) {_innerExceptions[index]} <---\n");
            }

            return msg;
        }

        internal int InnerExceptionCount => _innerExceptions.Length;
    }
}