using Exiled.API.Interfaces;
using System.ComponentModel;

namespace UltimateAFK
{
    public class Config : IConfig
    {
        [Description("Enable the plugin")]
        public bool IsEnabled { get; set; } = true;

        [Description("Enable debug logs")]
        public bool Debug { get; set; } = false;

        [Description("Time before kick/spectate after warning (grace period)")]
        public float AFKGracePeriod { get; set; } = 30f;

        [Description("Time in seconds before a player is considered AFK")]
        public float AFKTimeout { get; set; } = 120f;

        [Description("Should AFK players be moved to spectator instead of kicked?")]
        public bool ForceSpectatorInsteadOfKick { get; set; } = false;

        [Description("Broadcast shown when player is about to be kicked/spectated for AFK. Use %action% as a placeholder.")]
        public string AFKWarningBroadcast { get; set; } = "<color=yellow><b>You are AFK. Move or you will be %action%!</b></color>";

        [Description("Broadcast shown when player is kicked/spectated for AFK")]
        public string AFKFinalBroadcast { get; set; } = "<b><color=red>You were removed for being AFK!</color></b>";
    }
}
