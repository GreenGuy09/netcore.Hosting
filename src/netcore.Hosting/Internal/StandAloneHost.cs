using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using netcore.Hosting.Builder;

namespace netcore.Hosting.Internal
{
    public class StandAloneHost : IStandAloneHost
    {
        private readonly IServiceCollection _applicationServiceCollection;
        private IStartup _startup;
        private ApplicationLifetime _applicationLifetime;

        private readonly IServiceProvider _hostingServiceProvider;
        private readonly StandAloneHostOptions _options;
        private readonly IConfiguration _config;
        private readonly AggregateException _hostingStartupErrors;

        private IServiceProvider _applicationServices;
        private ILogger<StandAloneHost> _logger;

        // Used for testing only
        internal StandAloneHostOptions Options => _options;

        public StandAloneHost(
            IServiceCollection appServices,
            IServiceProvider hostingServiceProvider,
            StandAloneHostOptions options,
            IConfiguration config,
            AggregateException hostingStartupErrors)
        {
            if (appServices == null)
            {
                throw new ArgumentNullException(nameof(appServices));
            }

            if (hostingServiceProvider == null)
            {
                throw new ArgumentNullException(nameof(hostingServiceProvider));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            _config = config;
            _hostingStartupErrors = hostingStartupErrors;
            _options = options;
            _applicationServiceCollection = appServices;
            _hostingServiceProvider = hostingServiceProvider;
            _applicationServiceCollection.AddSingleton<IApplicationLifetime, ApplicationLifetime>();
        }

        public IServiceProvider Services
        {
            get
            {
                EnsureApplicationServices();
                return _applicationServices;
            }
        }

        public void Initialize()
        {
            BuildApplication();
        }

        public virtual void Start()
        {
            _logger = _applicationServices.GetRequiredService<ILogger<StandAloneHost>>();

            Initialize();

            _applicationLifetime = _applicationServices.GetRequiredService<IApplicationLifetime>() as ApplicationLifetime;

            // Fire IApplicationLifetime.Started
            _applicationLifetime?.NotifyStarted();

            // Log the fact that we did load hosting startup assemblies.
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                foreach (var assembly in _options.HostingStartupAssemblies)
                {
                    _logger.LogDebug("Loaded hosting startup assembly {assemblyName}", assembly);
                }
            }

            if (_hostingStartupErrors != null)
            {
                foreach (var exception in _hostingStartupErrors.InnerExceptions)
                {
                    _logger.LogError("An Error occured on startup {0}", exception.Message);
                }
            }
        }

        private void EnsureApplicationServices()
        {
            if (_applicationServices == null)
            {
                EnsureStartup();
                _applicationServices = _startup.ConfigureServices(_applicationServiceCollection);
            }
        }

        private void EnsureStartup()
        {
            if (_startup != null)
            {
                return;
            }

            _startup = _hostingServiceProvider.GetRequiredService<IStartup>();
        }

        private void BuildApplication()
        {
            try
            {
                EnsureApplicationServices();

                var builderFactory = _applicationServices.GetRequiredService<IApplicationBuilderFactory>();
                var builder = builderFactory.CreateBuilder();
                builder.ApplicationServices = _applicationServices;

                Action<IApplicationBuilder> configure = _startup.Configure;

                configure(builder);
            }
            catch (Exception ex) when (_options.CaptureStartupErrors)
            {
                // EnsureApplicationServices may have failed due to a missing or throwing Startup class.
                if (_applicationServices == null)
                {
                    _applicationServices = _applicationServiceCollection.BuildServiceProvider();
                }

                // Write errors to standard out so they can be retrieved when not in development mode.
                Console.Out.WriteLine("Application startup exception: " + ex.ToString());
                var logger = _applicationServices.GetRequiredService<ILogger<StandAloneHost>>();
                logger.LogError("An error occured {0}", ex.ToString());

                // Generate an HTML error page.
                var hostingEnv = _applicationServices.GetRequiredService<IHostingEnvironment>();
                var showDetailedErrors = hostingEnv.IsDevelopment() || _options.DetailedErrors;

            }
        }

        public void Dispose()
        {
            // Fire IApplicationLifetime.Stopping
            _applicationLifetime?.StopApplication();
            (_hostingServiceProvider as IDisposable)?.Dispose();
            (_applicationServices as IDisposable)?.Dispose();
            // Fire IApplicationLifetime.Stopped
            _applicationLifetime?.NotifyStopped();
        }
    }
}
