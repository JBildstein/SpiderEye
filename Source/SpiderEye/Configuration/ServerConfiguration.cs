using System;
using System.Collections.Generic;
using System.Reflection;
using SpiderEye.Mvc;

namespace SpiderEye.Configuration
{
    /// <summary>
    /// Server configuration.
    /// </summary>
    public class ServerConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether the internal server is started or not. Default is true.
        /// </summary>
        public bool UseInternalServer { get; set; }

        /// <summary>
        /// Gets or sets the start page url. Default is "index.html".
        /// </summary>
        public string StartPageUrl { get; set; }

        /// <summary>
        /// Gets or sets the host url. Default is null.
        /// If it's not set, it'll use the internal servers address.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the internal servers localhost port. Default is null.
        /// If it's not set, it'll search for a free one.
        /// </summary>
        public int? Port { get; set; }

        /// <summary>
        /// Gets or sets the folder path where the embedded files are. Default is "App".
        /// </summary>
        public string ContentFolder { get; set; }

        /// <summary>
        /// Gets or sets the assembly where the content files are embedded. Default is the entry assembly.
        /// </summary>
        public Assembly ContentAssembly { get; set; }

        /// <summary>
        /// Gets the controller registry.
        /// </summary>
        internal IControllerRegistry Controllers { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerConfiguration"/> class.
        /// </summary>
        public ServerConfiguration()
        {
            UseInternalServer = true;
            StartPageUrl = "index.html";
            ContentFolder = "App";
            Host = null;
            ContentAssembly = Assembly.GetEntryAssembly();

            Controllers = new ControllerRegistry();
        }

        /// <summary>
        /// Registers a controller of a given type.
        /// </summary>
        /// <typeparam name="T">The type of the controller to register.</typeparam>
        public void RegisterController<T>()
            where T : Controller, new()
        {
            Controllers.Register<T>();
        }

        /// <summary>
        /// Registers a controller with a factory method.
        /// </summary>
        /// <typeparam name="T">The type of the controller to register.</typeparam>
        /// <param name="controllerFactory">The factory to create a controller instance.</param>
        public void Register<T>(Func<T> controllerFactory)
            where T : Controller
        {
            Controllers.Register(controllerFactory);
        }
    }
}
