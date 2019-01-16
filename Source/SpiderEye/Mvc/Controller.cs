namespace SpiderEye.Mvc
{
    /// <summary>
    /// Base class for any controller.
    /// </summary>
    public abstract class Controller
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
    }
}
