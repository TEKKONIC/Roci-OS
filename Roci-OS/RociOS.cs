using System;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using Sandbox.ModAPI;
using VRage.Plugins;
using RociOS.Utility;

namespace RociOS
{
    public class RociOS : IPlugin, IDisposable
    {
        public static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public RociOS()
        {
            LogManager.Configuration = new XmlLoggingConfiguration("NLog.config");
            Log.Info("RociOS constructor called.");
        }

        public void Init(object gameInstance)
        {
            Log.Info("RociOS Init method called.");
            Task.Run(async () => await InitializeAsync());
        }

        private async Task InitializeAsync()
        {
            while (MyAPIGateway.Utilities == null || MyAPIGateway.Session == null)
            {
                Log.Warn("Waiting for MyAPIGateway.Utilities and MyAPIGateway.Session to be initialized...");
                await Task.Delay(10000);
            }

            Log.Info("MyAPIGateway.Utilities and MyAPIGateway.Session are initialized.");
            MyAPIGateway.Utilities.ShowMessage("RociOS", "Client-side plugin initialized.");
            DisableSuitAntenna.Initialize();
        }

        public void Update()
        {
            // Implement any update logic here if needed
        }

        public void Dispose()
        {
            Log.Info("RociOS Dispose method called.");
            DisableSuitAntenna.Unload();
        }
    }
}