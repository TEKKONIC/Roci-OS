using System;
using System.IO;
using Newtonsoft.Json;
using NLog;

namespace RociOS.Config
{
    public class RociOSConfig
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private const string ConfigFileName = "RociOSConfig.json";

        public bool EnableAutoFactionChat { get; set; } = true;
        public int InitializationDelay { get; set; } = 15000;
        public bool DisableSuitBroadcasting { get; set; } = true;
        public bool GrabSingleItem { get; set; } = true;
        public const int RetryDelayMilliseconds = 10000; 
        public const int MaxRetries = 5; 

        public static RociOSConfig Load()
        {
            Log.Info("Attempting to load configuration.");
            try
            {
                if (File.Exists(ConfigFileName))
                {
                    Log.Info($"Config file {ConfigFileName} found. Reading file.");
                    var json = File.ReadAllText(ConfigFileName);
                    Log.Info("Config file read successfully. Deserializing JSON.");
                    return JsonConvert.DeserializeObject<RociOSConfig>(json);
                }
                else
                {
                    Log.Warn($"Config file {ConfigFileName} not found. Using default settings.");
                    return new RociOSConfig();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load config file. Using default settings.");
                return new RociOSConfig();
            }
        }

        public void Save()
        {
            Log.Info("Attempting to save configuration.");
            try
            {
                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                Log.Info("Configuration serialized to JSON. Writing to file.");
                File.WriteAllText(ConfigFileName, json);
                Log.Info("Config file saved successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to save config file.");
            }
        }
    }
}