using System;
using System.Net;
using System.Threading.Tasks;

namespace SpiderEye.Server
{
    /// <summary>
    /// Defines middleware that can be added to the servers request pipeline.
    /// </summary>
    internal interface IMiddleware
    {
        /// <summary>
        /// Request handling method.
        /// </summary>
        /// <param name="context">The context of the request.</param>
        /// <param name="next">A delegate for the next middleware in the pipeline.</param>
        /// <returns>A <see cref="Task"/> that represents the execution of this middleware.</returns>
        Task InvokeAsync(HttpListenerContext context, Func<Task> next);
    }
}
