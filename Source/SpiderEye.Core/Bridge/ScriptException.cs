using System;

namespace SpiderEye.Bridge
{
    /// <summary>
    /// Represents errors that occur during script execution.
    /// </summary>
    public class ScriptException : Exception
    {
        /// <inheritdoc/>
        public override string StackTrace
        {
            get { return stackTrace ?? base.StackTrace; }
        }

        private readonly string stackTrace;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptException"/> class.
        /// </summary>
        public ScriptException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ScriptException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception,
        /// or a null reference if no inner exception is specified.
        /// </param>
        public ScriptException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="stackTrace">The original stack trace of the error.</param>
        internal ScriptException(string message, string stackTrace)
            : base(message)
        {
            this.stackTrace = stackTrace;
        }
    }
}
