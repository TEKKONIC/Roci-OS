using System;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using NLog;
using NLog.Config;
using Sandbox.ModAPI;
using VRage.Plugins;
using RociOS.Utility;
using RociOS.Config;

namespace RociOS
{
    public class RociOS : IPlugin, IDisposable
    {
        public static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private RociOSConfig config;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public RociOS()
        {
            LogManager.Configuration = new XmlLoggingConfiguration("NLog.config");
            Log.Info("RociOS constructor called.");
            config = RociOSConfig.Load();
        }

        public void Init(object gameInstance)
        {
            Log.Info("RociOS Init method called.");
            Task.Run(async () => await InitializeAsync());
            if (config.EnableAutoFactionChat)
            {
                new Harmony("AutoFactionChat").PatchAll(Assembly.GetExecutingAssembly());
                Log.Info("AutoFactionChat loaded");
            }
        }

        private async Task InitializeAsync()
        {
            Log.Info("InitializeAsync method started.");
            while (MyAPIGateway.Utilities == null || MyAPIGateway.Session == null)
            {
                Log.Warn("Waiting for MyAPIGateway.Utilities and MyAPIGateway.Session to be initialized...");
                await Task.Delay(config.InitializationDelay);
            }

            Log.Info("MyAPIGateway.Utilities and MyAPIGateway.Session are initialized.");
            MyAPIGateway.Utilities.ShowMessage("RociOS", "Client-side plugin initialized.");

            if (MyAPIGateway.Multiplayer.IsServer)
            {
                Log.Info("Server detected. Initializing server-Specific logic...");
                // Initialize server-specific logic here 
            }
            else
            {
                Log.Info("Client detected. Initializing client-Specific logic...");
                DisableSuitAntenna.Initialize();
            }
            Log.Info("InitializeAsync method completed.");
        }

        public void Update()
        {
            Log.Info("Update method called.");
            // Implement any update logic here if needed
            Log.Debug("Performing update logic...");
            // Example placeholder logic
            if (config.EnableAutoFactionChat)
            {
                Log.Debug("AutoFactionChat is enabled.");
                // Add any update logic related to AutoFactionChat here
            }
            else
            {
                Log.Debug("AutoFactionChat is disabled.");
            }
            Log.Info("Update method completed.");
        }

        public void Dispose()
        {
            Log.Info("RociOS Dispose method called.");
            DisableSuitAntenna.Unload();
        }
    }
}