using System;
using System.Collections.Generic;
using System.Text;

namespace netcore.Hosting
{
    /// <summary>
    /// Represents a configured stand alone host.
    /// </summary>
    public interface IStandAloneHost : IDisposable
    {
        /// <summary>
        /// The <see cref="IServiceProvider"/> for the host.
        /// </summary>
        IServiceProvider Services { get; }

        /// <summary>
        /// Starts the stand alone host.
        /// </summary>
        void Start();
    }
}
