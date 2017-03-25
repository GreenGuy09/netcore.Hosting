using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace netcore.Hosting.Startup
{
    public abstract class StartupBase : IStartup
    {
        public abstract void Configure(IApplicationBuilder app);

        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider();
        }
    }

    public abstract class StartupBase<TContainerBuilder> : IStartup
    {
        private readonly IServiceProviderFactory<TContainerBuilder> _factory;

        public StartupBase(IServiceProviderFactory<TContainerBuilder> factory)
        {
            _factory = factory;
        }

        public abstract void Configure(IApplicationBuilder app);

        public virtual void ConfigureServices(IServiceCollection services)
        {

        }

        IServiceProvider IStartup.ConfigureServices(IServiceCollection services)
        {
            ConfigureServices(services);
            var builder = _factory.CreateBuilder(services);
            ConfigureContainer(builder);
            return _factory.CreateServiceProvider(builder);
        }

        public virtual void ConfigureContainer(TContainerBuilder containerBuilder) { }
    }

}
