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
    public partial class Player:CommandModule
    { 
     private static readonly Dictionary<ulong, DateTime> _updateCommandTimeout = new Dictionary<ulong, DateTime>();

     
    }
}