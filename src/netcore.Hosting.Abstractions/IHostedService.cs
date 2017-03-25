using System;
using System.Collections.Generic;
using System.Text;

namespace netcore.Hosting
{
    /// <summary>
    /// Defines methods for objects that are managed by the host.
    /// </summary>
    public interface IHostedService
    {
        /// <summary>
        /// Triggered when the application host has fully started and the server is waiting
        /// for requests.
        /// </summary>
        void Start();

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// Requests may still be in flight. Shutdown will block until this event completes.
        /// </summary>
        void Stop();
    }
}
