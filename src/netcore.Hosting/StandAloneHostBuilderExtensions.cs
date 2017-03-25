using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using netcore.Hosting.Startup;
using netcore.Hosting.Internal;

namespace netcore.Hosting
{
    public static class StandAloneHostBuilderExtensions
    {
        /// <summary>
        /// Specify the startup method to be used to configure the web application.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IStandAloneHostBuilder"/> to configure.</param>
        /// <param name="configureApp">The delegate that configures the <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IStandAloneHostBuilder"/>.</returns>
        public static IStandAloneHostBuilder Configure(this IStandAloneHostBuilder hostBuilder, Action<IApplicationBuilder> configureApp)
        {
            if (configureApp == null)
            {
                throw new ArgumentNullException(nameof(configureApp));
            }

            var startupAssemblyName = configureApp.GetMethodInfo().DeclaringType.GetTypeInfo().Assembly.GetName().Name;

            return hostBuilder.UseSetting(StandAloneHostDefaults.ApplicationKey, startupAssemblyName)
                              .ConfigureServices(services =>
                              {
                                  services.AddSingleton<IStartup>(sp =>
                                  {
                                      return new DelegateStartup(sp.GetRequiredService<IServiceProviderFactory<IServiceCollection>>(), configureApp);
                                  });
                              });
        }


        /// <summary>
        /// Specify the startup type to be used by the web host.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IStandAloneHostBuilder"/> to configure.</param>
        /// <param name="startupType">The <see cref="Type"/> to be used.</param>
        /// <returns>The <see cref="IStandAloneHostBuilder"/>.</returns>
        public static IStandAloneHostBuilder UseStartup(this IStandAloneHostBuilder hostBuilder, Type startupType)
        {
            var startupAssemblyName = startupType.GetTypeInfo().Assembly.GetName().Name;

            return hostBuilder.UseSetting(StandAloneHostDefaults.ApplicationKey, startupAssemblyName)
                              .ConfigureServices(services =>
                              {
                                  if (typeof(IStartup).GetTypeInfo().IsAssignableFrom(startupType.GetTypeInfo()))
                                  {
                                      services.AddSingleton(typeof(IStartup), startupType);
                                  }
                                  else
                                  {
                                      services.AddSingleton(typeof(IStartup), sp =>
                                      {
                                          var hostingEnvironment = sp.GetRequiredService<IHostingEnvironment>();
                                          return new ConventionBasedStartup(StartupLoader.LoadMethods(sp, startupType, hostingEnvironment.EnvironmentName));
                                      });
                                  }
                              });
        }

        /// <summary>
        /// Specify the startup type to be used by the web host.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IStandAloneHostBuilder"/> to configure.</param>
        /// <typeparam name ="TStartup">The type containing the startup methods for the application.</typeparam>
        /// <returns>The <see cref="IStandAloneHostBuilder"/>.</returns>
        public static IStandAloneHostBuilder UseStartup<TStartup>(this IStandAloneHostBuilder hostBuilder) where TStartup : class
        {
            return hostBuilder.UseStartup(typeof(TStartup));
        }

    }
}
