using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HarmonyLib;
using NLog;
using NLog.Config;
using NLog.Targets;
using Sandbox.ModAPI;
using VRage.FileSystem;
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
            try
            {
                // Configure NLog programmatically
                var config = new LoggingConfiguration();

                var consoleTarget = new ConsoleTarget("console")
                {
                    Layout = "${longdate} ${level:uppercase=true} ${message} ${exception:format=toString,StackTrace}"
                };

                //log file path
                string logDirectory = MyFileSystem.UserDataPath;
                string logFilePath = Path.Combine(logDirectory, "RociOS.log");

                var fileTarget = new FileTarget("file")
                {
                    FileName = logFilePath,
                    Layout = "${longdate} ${level:uppercase=true} ${message} ${exception:format=toString,StackTrace}"
                };

                config.AddTarget(consoleTarget);
                config.AddTarget(fileTarget);

                config.AddRule(LogLevel.Info, LogLevel.Fatal, consoleTarget);
                config.AddRule(LogLevel.Debug, LogLevel.Fatal, fileTarget);

                LogManager.Configuration = config;

                Log.Info("RociOS constructor called.");
                this.config = RociOSConfig.Load(); 
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to initialize RociOS plugin.");
                throw;
            }
        }

        public void Init(object gameInstance)
        {
            Log.Info("RociOS Plugin Online.");
            Log.Info("RociOS Init method called.");
            Task.Run(async () => await InitializeAsync());

            if (config.EnableAutoFactionChat)
            {
                new Harmony("AutoFactionChat").PatchAll(Assembly.GetExecutingAssembly());
                Log.Info("AutoFactionChat loaded");
            }
            if (config.GrabSingleItem)
            {
                Log.Debug("GrabSingleItem: Patching");
                new Harmony("GrabSingleItem").PatchAll(Assembly.GetExecutingAssembly());
                Log.Info("GrabSingleItem: Patches applied");
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

            if (MyAPIGateway.Utilities != null)
            {
                Log.Info("MyAPIGateway.Utilities is not null. Attempting to show message.");
                MyAPIGateway.Utilities.ShowMessage("RociOS", "Client-side plugin initialized.");
                Log.Info("Message shown successfully.");
            }
            else
            {
                Log.Error("MyAPIGateway.Utilities is null. Cannot show message.");
            }

            if (MyAPIGateway.Multiplayer.MultiplayerActive)
            {
                Log.Info("Multiplayer server detected. Initializing server-specific logic...");
                DisableSuitAntenna.Initialize();
            }
            else if (MyAPIGateway.Session.OnlineMode == VRage.Game.MyOnlineModeEnum.OFFLINE)
            {
                Log.Info("Single-player detected. Initializing single-player specific logic...");
                DisableSuitAntenna.Initialize();
            }
            Log.Info("InitializeAsync method completed.");
        }

        public void Update()
        {
            //Log.Info("Update method called.");
            // Implement any update logic here if needed
            //Log.Debug("Performing update logic...");
            // Example placeholder logic
            //if (config.EnableAutoFactionChat)
            //{
                //Log.Debug("AutoFactionChat is enabled.");
                // Add any update logic related to AutoFactionChat here
            //}
            //else
            //{
                //Log.Debug("AutoFactionChat is disabled.");
            //}
            //Log.Info("Update method completed.");
        }

        public void Dispose()
        {
            Log.Info("RociOS Dispose method called.");
            DisableSuitAntenna.Unload();
        }
    }
}