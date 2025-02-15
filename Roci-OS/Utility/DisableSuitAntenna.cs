using System.Collections.Generic;
using Sandbox.Game.Entities.Character;
using Sandbox.ModAPI;
using VRage.Game.ModAPI;
using NLog;

namespace RociOS.Utility
{
    public static class DisableSuitAntenna
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private static bool isInitialized = false;

        public static void Initialize()
        {
            Log.Info("Initializing DisableSuitAntenna...");
            MyAPIGateway.Session.OnSessionReady += OnSessionReady;
            MyAPIGateway.Multiplayer.RegisterSecureMessageHandler(12345, OnPlayerRespawn);
            Log.Info("Event handlers registered.");
            isInitialized = true;
        }

        private static void OnSessionReady()
        {
            Log.Info("Session is ready. Disabling antenna...");
            DisableAntenna();
        }

        private static void OnPlayerRespawn(ushort handlerId, byte[] message, ulong senderId, bool fromServer)
        {
            Log.Info($"Player respawn detected. HandlerId: {handlerId}, SenderId: {senderId}, FromServer: {fromServer}");
            DisableAntenna();
        }

        public static void DisableAntenna()
        {
            Log.Info("Attempting to disable antenna for the current player...");
            var player = MyAPIGateway.Session.Player;
            if (player != null)
            {
                Log.Info($"Player found: {player.DisplayName}");
                var character = player.Character as MyCharacter;
                if (character != null)
                {
                    Log.Info($"Character found: {character.DisplayName}");
                    DisableSuitAntennaForCharacter(character);
                }
                else
                {
                    Log.Warn("Character is null.");
                }
            }
            else
            {
                Log.Warn("Player is null.");
            }
        }

        private static void DisableSuitAntennaForCharacter(MyCharacter character)
        {
            character.EnableBroadcastingPlayerToggle(false);
            Log.Info("Disabled suit antenna for player: " + character.DisplayName);
        }

        public static void Unload()
        {
            Log.Info("Unloading DisableSuitAntenna...");
            if (!isInitialized)
            {
                return;
            }
            MyAPIGateway.Session.OnSessionReady -= OnSessionReady;
            MyAPIGateway.Multiplayer.UnregisterSecureMessageHandler(12345, OnPlayerRespawn);
            Log.Info("Event handlers unregistered.");
            isInitialized = false;
        }
    }
}
