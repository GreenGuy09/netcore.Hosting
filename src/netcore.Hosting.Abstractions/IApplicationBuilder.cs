using System;
using System.Collections.Generic;
using System.Text;

namespace netcore.Hosting
{
    public interface IApplicationBuilder
    {
        //
        // Summary:
        //     Gets or sets the System.IServiceProvider that provides access to the application's
        //     service container.
        IServiceProvider ApplicationServices { get; set; }
        //
        // Summary:
        //     Gets a key/value collection that can be used to share data between middleware.
        IDictionary<string, object> Properties { get; }

        //
        // Summary:
        //     Creates a new Microsoft.AspNetCore.Builder.IApplicationBuilder that shares the
        //     Microsoft.AspNetCore.Builder.IApplicationBuilder.Properties of this Microsoft.AspNetCore.Builder.IApplicationBuilder.
        //
        // Returns:
        //     The new Microsoft.AspNetCore.Builder.IApplicationBuilder.
        IApplicationBuilder New();
    }
}
