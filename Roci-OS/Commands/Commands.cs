using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandbox.Definitions;
using Sandbox.Game.World;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.ModAPI;

namespace Roci_OS.Commands
{
    [Command("Roci_OS")]
    public partial class Player:CommandModule
    { 
     private static readonly Dictionary<ulong, DateTime> _updateCommandTimeout = new Dictionary<ulong, DateTime>();
     
     [Command("!RociOS Beacon disabler on/off", "Enable or disable the beacon disabler for your suit.")]
     [Permission(MyPromoteLevel.None)]
     public void 


     
    }
}
