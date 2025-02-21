using HarmonyLib;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage;
using VRage.Game.Components;
using VRage.Game.Entity;
using Sandbox.Game.Multiplayer;
using Sandbox.Game.World;
using VRage.Utils;
using VRage.Game.ModAPI;
using NLog;
using Sandbox.Game.Entities.Character;
using NLog.Fluent;

namespace RociOS
{
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
    public partial class RociSession : MySessionComponentBase
    {
      bool antennaDisabled = false;
      bool characterHasBeenDead = false;

      public override void UpdateBeforeSimulation()
      {
        if (
            MyAPIGateway.Session?.Player?.Controller != null &&
            MyAPIGateway.Session?.Player?.Character != null &&
            !MyAPIGateway.Session.Player.Character.IsDead)
        {
          var character = MyAPIGateway.Session.Player.Character as MyCharacter;
  
          if (!antennaDisabled)
          {
              character.EnableBroadcasting(false);
              character.RequestEnableBroadcasting(false);
              Log.Info("Disabled suit antenna for player: " + character.DisplayName);
              MyAPIGateway.Utilities.ShowNotification("Suit antenna disabled.", 15000, VRage.Game.MyFontEnum.White);
              antennaDisabled = true; 
          }
        } else {
          characterHasBeenDead = true;
          antennaDisabled = false;
        }
      }
    }
}