using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace netcore.Hosting.Internal
{
    public class StandAloneHostOptions
    {
        public StandAloneHostOptions()
        {
        }

        public StandAloneHostOptions(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            ApplicationName = configuration[StandAloneHostDefaults.ApplicationKey];
            StartupAssembly = configuration[StandAloneHostDefaults.StartupAssemblyKey];
            DetailedErrors = ParseBool(configuration, StandAloneHostDefaults.DetailedErrorsKey);
            CaptureStartupErrors = ParseBool(configuration, StandAloneHostDefaults.CaptureStartupErrorsKey);
            Environment = configuration[StandAloneHostDefaults.EnvironmentKey];
            ContentRootPath = configuration[StandAloneHostDefaults.ContentRootKey];
            HostingStartupAssemblies = configuration[StandAloneHostDefaults.HostingStartupAssembliesKey]?.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
        }

        public string ApplicationName { get; set; }

        public IReadOnlyList<string> HostingStartupAssemblies { get; set; }

        public bool DetailedErrors { get; set; }

        public bool CaptureStartupErrors { get; set; }

        public string Environment { get; set; }

        public string StartupAssembly { get; set; }

        public string ContentRootPath { get; set; }

        private static bool ParseBool(IConfiguration configuration, string key)
        {
            return string.Equals("true", configuration[key], StringComparison.OrdinalIgnoreCase)
                || string.Equals("1", configuration[key], StringComparison.OrdinalIgnoreCase);
        }
    }
}
