using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Tellurium.VisualAssertion.Dashboard
{
    public class WebServer:IDisposable
    {
        private IWebHost host;

        public void Run()
        {
            host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://*:5000")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }

        public void Dispose()
        {
            host?.Dispose();
        }
    }
}