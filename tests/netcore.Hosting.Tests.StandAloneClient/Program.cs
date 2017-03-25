using System;

namespace netcore.Hosting.Tests.StandAloneClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new StandAloneHostBuilder()
                .UseStartup<Startup>()
                .Build();

            builder.Start();
        }
    }
}