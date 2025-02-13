using System.IO;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Character;
using Sandbox.Game.Entities.Character.Components;
using Sandbox.ModAPI;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.Game.ModAPI.Ingame;
using VRage.Plugins;

namespace RociOS
{
    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
    public class ClientPlugin : MySessionComponentBase, IPlugin
    {
        private bool _initialized;

        public void Init(object gameInstance)
        {
            if (!_initialized)
            {
                Init();
                _initialized = true;
            }
        }

        private void Init()
        {
            if (MyAPIGateway.Entities != null)
            {
                MyAPIGateway.Entities.OnEntityAdd += OnEntityAdd;
            }
        }

        protected override void UnloadData()
        {
            if (MyAPIGateway.Entities != null)
            {
                MyAPIGateway.Entities.OnEntityAdd -= OnEntityAdd;
            }
        }

        private void OnEntityAdd(IMyEntity entity)
        {
            if (entity is IMyCharacter character)
            {
                character.OnClosing += OnCharacterClosing;
                DisableSuitAntenna(character as Sandbox.Game.Entities.Character.MyCharacter);
            }
        }

        private void OnCharacterClosing(IMyEntity entity)
        {
            if (entity is IMyCharacter character)
            {
                character.OnClosing -= OnCharacterClosing;
            }
        }

        private void DisableSuitAntenna(Sandbox.Game.Entities.Character.MyCharacter character)
        {
            character.EnableBroadcastingPlayerToggle(false);
        }

        public void Update()
        {
            // nothing to do here - we're using UpdateAfterSimulation
        }

        public void Dispose()
        {
            UnloadData();
        }
    }
}