using System;
using System.Collections.Generic;
using System.Text;

namespace netcore.Hosting.Internal
{
    public class HostingEnvironment : IHostingEnvironment
    {
        public string EnvironmentName { get; set; } = Hosting.EnvironmentName.Production;

        public string ApplicationName { get; set; }

        public string ContentRootPath { get; set; }
    }
}
