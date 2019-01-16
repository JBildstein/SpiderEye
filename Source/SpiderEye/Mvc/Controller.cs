using System;

namespace SpiderEye.Mvc
{
    /// <summary>
    /// Base class for any controller.
    /// </summary>
    public abstract class Controller : IDisposable
    {
        /// <summary>
        /// Gets the request information.
        /// </summary>
        public RequestInfo Request
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the response information.
        /// </summary>
        public ResponseInfo Response
        {
            get;
            internal set;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
        }
    }
}
