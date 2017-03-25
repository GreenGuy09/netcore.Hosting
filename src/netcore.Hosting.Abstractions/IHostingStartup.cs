using System;
using System.Collections.Generic;
using System.Text;

namespace netcore.Hosting
{
    /// <summary>
    /// Represents platform specific configuration that will be applied to a <see cref="IStandAloneHostBuilder"/> 
    /// when building an <see cref="IStandAloneHost"/>.
    /// </summary>
    public interface IHostingStartup
    {
        /// <summary>
        /// Configure the <see cref="IStandAloneHostBuilder"/>.
        /// </summary>
        /// <remarks>
        /// Configure is intended to be called before user code, allowing a user to overwrite any changes made.
        /// </remarks>
        /// <param name="builder"></param>
        void Configure(IStandAloneHostBuilder builder);
    }
}
