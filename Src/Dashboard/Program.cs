using System;
using System.Threading;
using Topshelf;

namespace Tellurium.VisualAssertion.Dashboard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            InstallDashboardService();
        }

        private static void InstallDashboardService()
        {
            HostFactory.Run(hostConfiguration =>
            {
                hostConfiguration.Service<WebServer>(wsc =>
                {
                    wsc.ConstructUsing(() => new WebServer());
                    wsc.WhenStarted(server =>
                    {
                        var myThread = new Thread(new ThreadStart(server.Run));
                        myThread.IsBackground = true;  // This line will prevent thread from working after service stop.
                        myThread.Start();
                    });
                    wsc.WhenStopped(ws => ws.Dispose());
                });
                hostConfiguration.RunAsLocalSystem();
                hostConfiguration.SetDescription("This is Tellurium Dashboard");
                hostConfiguration.SetDisplayName("Tellurium Dashboard");
                hostConfiguration.SetServiceName("TelluriumDashboard");
                hostConfiguration.StartAutomatically();
            });
        }
    }
}
