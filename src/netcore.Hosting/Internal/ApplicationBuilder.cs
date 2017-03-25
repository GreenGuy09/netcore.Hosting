using System;
using System.Collections.Generic;
using System.Text;

namespace netcore.Hosting.Internal
{
    public class ApplicationBuilder : IApplicationBuilder
    {
        public ApplicationBuilder(IServiceProvider serviceProvider)
        {
            Properties = new Dictionary<string, object>(StringComparer.Ordinal);
            ApplicationServices = serviceProvider;
        }


        private ApplicationBuilder(ApplicationBuilder builder)
        {
            Properties = new CopyOnWriteDictionary<string, object>(builder.Properties, StringComparer.Ordinal);
        }

        public IServiceProvider ApplicationServices
        {
            get
            {
                return GetProperty<IServiceProvider>(Constants.BuilderProperties.ApplicationServices);
            }
            set
            {
                SetProperty<IServiceProvider>(Constants.BuilderProperties.ApplicationServices, value);
            }
        }

        public IDictionary<string, object> Properties { get; }

        private T GetProperty<T>(string key)
        {
            object value;
            return Properties.TryGetValue(key, out value) ? (T)value : default(T);
        }

        private void SetProperty<T>(string key, T value)
        {
            Properties[key] = value;
        }

        public IApplicationBuilder New()
        {
            return new ApplicationBuilder(this);
        }
    }
}
