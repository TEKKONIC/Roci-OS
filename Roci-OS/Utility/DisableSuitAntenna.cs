using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox.Game.Entities.Character;
using Sandbox.ModAPI;
using VRage.Game.ModAPI;
using VRage.GameServices;
using VRageMath;
using VRage.Game;
using VRage.Game.Definitions;
using VRage.Game.ModAPI.Interfaces;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Network;
using VRage.Utils;
using NLog;
using RociOS.Config;

namespace RociOS.Utility
{
    public static class DisableSuitAntenna
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private static bool isInitialized = false;
        private const ushort MessageId = 65244;
        private static RociOSConfig config;

        public static void Initialize()
        {
            if (isInitialized)
            {
                Log.Warn("DisableSuitAntenna is already initialized.");
                return;
            }

            Log.Info("Initializing DisableSuitAntenna...");
            config = RociOSConfig.Load();
            MyAPIGateway.Session.OnSessionReady += OnSessionReady;
            MyAPIGateway.Session.OnSessionLoading += OnSessionLoading;
            Log.Info("Event handlers registered.");
            isInitialized = true;
        }

        private static void OnSessionReady()
        {
            Log.Info("OnSessionReady event triggered. Disabling antenna...");
            Task.Run(DisableAntenna);
        }

        private static void OnSessionLoading()
        {
            Log.Info("OnSessionLoading event triggered. Reinitializing...");
            Unload();
            Initialize();
        }

        public static async Task DisableAntenna()
        {
            Log.Info("Attempting to disable antenna for the current player...");
            var player = MyAPIGateway.Session.Player;
            if (player != null)
            {
                Log.Info($"Player found: {player.DisplayName}");
                var character = player.Character as MyCharacter;
                int retries = 0;
                while (character == null && retries < RociOSConfig.MaxRetries)
                {
                    Log.Warn("Character is null. Retrying...");
                    await Task.Delay(RociOSConfig.RetryDelayMilliseconds);
                    character = player.Character as MyCharacter;
                    retries++;
                }

                if (character != null)
                {
                    Log.Info($"Character found: {character.DisplayName}");
                    await DisableSuitAntennaForCharacter(character);
                }
                else
                {
                    Log.Error("Character is still null after retries.");
                }
            }
            else
            {
                Log.Warn("Player is null.");
            }
        }

        private static async Task DisableSuitAntennaForCharacter(MyCharacter character)
        {
            bool antennaDisabled = false;
            MyCharacter previousCharacter = character;

            while (true)
            {
                if (character.IsDead)
                {
                    Log.Warn("Character is dead. Waiting to retry...");
                    await Task.Delay(RociOSConfig.RetryDelayMilliseconds);
                    antennaDisabled = false; 
                    continue;
                }

                if (character != previousCharacter)
                {
                    Log.Info("Character has respawned. Re-disabling antenna...");
                    previousCharacter = character;
                    antennaDisabled = false; 
                }

                if (!antennaDisabled)
                {
                    character.EnableBroadcasting(false);
                    character.RequestEnableBroadcasting(false);
                    Log.Info("Disabled suit antenna for player: " + character.DisplayName);
                    MyAPIGateway.Utilities.ShowNotification("Suit antenna disabled.", 15000, VRage.Game.MyFontEnum.White);
                    antennaDisabled = true; 
                }

                await Task.Delay(RociOSConfig.RetryDelayMilliseconds);
            }
        }

        public static void Unload()
        {
            Log.Info("Unloading DisableSuitAntenna...");
            if (!isInitialized)
            {
                Log.Warn("Already uninitialized. Exiting unload process.");
                return;
            }
            MyAPIGateway.Session.OnSessionReady -= OnSessionReady;
            MyAPIGateway.Session.OnSessionLoading -= OnSessionLoading;
            Log.Info("Event handlers unregistered.");
            isInitialized = false;
        }
    }
}
