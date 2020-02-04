using System;
using System.Collections.Generic;
using ConVar;

namespace Oxide.Plugins
{
    [Info("AntiNoReload", "Nosharp", "0.0.1")]
    [Description("Stops people from using cheats called which affect reload times.")]
    public class AntiNoReload : RustPlugin
    {
        Dictionary<ulong, int> LastReloads = new Dictionary<ulong,int>();
        DateTime epochStart = new DateTime(1970, 1, 1);

        int TimeSinceEpoch()
        {
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            return (int)timeSpan.TotalSeconds;
        }
        void SendMessage(BasePlayer player, string message) => player.SendConsoleCommand("chat.add", new object[] { (int)Chat.ChatChannel.Global, 76561198171821322U, message });

        object OnReloadWeapon(BasePlayer player, BaseProjectile projectile)
        {
            if(player.GetActiveItem().info.shortname != "rocket.launcher") return null;
            LastReloads[player.userID] = TimeSinceEpoch();
            return null;
        }

        void OnRocketLaunched(BasePlayer player, BaseEntity entity)
        {
            //if (player.userID != 76561198880912497U && player.userID != 76561198171821322U) return;
            if (!LastReloads.ContainsKey(player.userID)) return;
            int diff = TimeSinceEpoch() - LastReloads[player.userID];
            if (diff < 5)
            {
                
                player.Kick("<color=#F6542F>[Magma Anti-Cheat]</color> Flagged Check B");
                BasePlayer.activePlayerList.ForEach((ply)=>SendMessage(ply, $"<color=#F6542F>[Magma Anti-Cheat]</color> Instant Reload Violation {player.userID}"));
                Server.Command("ban", player.userID, "<color=#F6542F>[Magma Anti-Cheat]</color> Flagged Check B");
            }
            
        }
    }
}
