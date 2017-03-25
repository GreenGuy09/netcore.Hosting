using System;
using System.Collections.Generic;
using System.Text;

namespace netcore.Hosting
{
    public static class StandAloneHostDefaults
    {
        public static readonly string ApplicationKey = "applicationName";
        public static readonly string StartupAssemblyKey = "startupAssembly";
        public static readonly string HostingStartupAssembliesKey = "hostingStartupAssemblies";
        public static readonly string CaptureStartupErrorsKey = "captureStartupErrorsKey";
        public static readonly string DetailedErrorsKey = "detailedErrors";
        public static readonly string EnvironmentKey = "environment";
        public static readonly string ContentRootKey = "contentRoot";
    }
}
