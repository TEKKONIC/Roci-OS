using System.Collections.Generic;
using Sandbox.Game.Entities.Character;
using Sandbox.ModAPI;
using VRage.Game.ModAPI;
using VRage.GameServices;
using NLog;

namespace RociOS.Utility
{
    public static class DisableSuitAntenna
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private static bool isInitialized = false;
        private const ushort MessageId = 65244;

        public static void Initialize()
        {
            Log.Info("Initializing DisableSuitAntenna...");
            MyAPIGateway.Session.OnSessionReady += OnSessionReady;
            MyAPIGateway.Session.OnSessionLoading += OnSessionLoading;
            MyAPIGateway.Multiplayer.RegisterSecureMessageHandler(MessageId, OnPlayerRespawn);
            Log.Info("Event handlers registered.");
            isInitialized = true;
        }

        private static void OnSessionReady()
        {
            Log.Info("Session is ready. Disabling antenna...");
            DisableAntenna();
        }

        private static void OnSessionLoading()
        {
            Log.Info("Session is unloading. Reinitializing...");
            Unload();
            Initialize();
        }

        private static void OnPlayerRespawn(ushort handlerId, byte[] message, ulong senderId, bool fromServer)
        {
            Log.Info($"Player respawn detected. HandlerId: {handlerId}, SenderId: {senderId}, FromServer: {fromServer}");
            if (MyAPIGateway.Multiplayer.IsServer)
            {
                Log.Info("Server detected. Disabling antenna and sending message to client...");
                DisableAntenna();
                MyAPIGateway.Multiplayer.SendMessageTo(MessageId, new byte[0], senderId);
            }
            else
            {
                Log.Info("Client detected. Disabling antenna...");
                DisableAntenna();
            }
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
            //character.EnableBroadcastingPlayerToggle(true); disables the switch for broadcasting
            character.EnableBroadcasting(false);
            Log.Info("Disabled suit antenna for player: " + character.DisplayName);
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
            MyAPIGateway.Multiplayer.UnregisterSecureMessageHandler(MessageId, OnPlayerRespawn);
            Log.Info("Event handlers unregistered.");
            isInitialized = false;
        }
    }
}
