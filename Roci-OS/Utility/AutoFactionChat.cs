using HarmonyLib;
using Sandbox.Game.GameSystems;
using Sandbox.ModAPI;
using Sandbox.Game.Gui;
using System.Reflection;
using RociOS;

namespace AutoFactionChat
{
    [HarmonyPatch]
    internal static class MyChatSystemPatch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Constructor(typeof(MyChatSystem));
        }

        private static void Postfix(ref ChatChannel ___m_currentChannel)
        {
            ___m_currentChannel = ChatChannel.Faction;
            RociOS.RociOS.Log.Info("Switched to Faction Channel");
            MyAPIGateway.Utilities.ShowMessage("RociOS", "Switched to Faction Channel.");
            RociOS.RociOS.Log.Info("Message shown successfully.");
        }
    }
}