using System;
using System.Collections.Generic;
using System.Text;

namespace netcore.Hosting.Builder
{
    public interface IApplicationBuilderFactory
    {
        IApplicationBuilder CreateBuilder();
    }
}
