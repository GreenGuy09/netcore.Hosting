using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace netcore.Hosting.Internal
{
    public static class HostingEnvironmentExtensions
    {
        public static void Initialize(this IHostingEnvironment hostingEnvironment, string applicationName, string contentRootPath, StandAloneHostOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (string.IsNullOrEmpty(applicationName))
            {
                throw new ArgumentException("A valid non-empty application name must be provided.", nameof(applicationName));
            }
            if (string.IsNullOrEmpty(contentRootPath))
            {
                throw new ArgumentException("A valid non-empty content root must be provided.", nameof(contentRootPath));
            }
            if (!Directory.Exists(contentRootPath))
            {
                throw new ArgumentException($"The content root '{contentRootPath}' does not exist.", nameof(contentRootPath));
            }

            hostingEnvironment.ApplicationName = applicationName;
            hostingEnvironment.ContentRootPath = contentRootPath;

            hostingEnvironment.EnvironmentName =
                options.Environment ??
                hostingEnvironment.EnvironmentName;
        }
    }
}
